namespace NewRevolutionaryBank.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

public class BankAccountService : IBankAccountService
{
	private readonly NrbDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IEmailSender _emailSender;

	public BankAccountService(
		NrbDbContext context,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IEmailSender emailSender)
	{
		_context = context;
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
	}

	public async Task CreateAsync(string userName, BankAccountCreateViewModel model)
	{
		ApplicationUser? foundUser = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		if (foundUser.BankAccounts.Count(ba => !ba.IsClosed) >= 5)
		{
			return;
		}

		string IBAN = GenerateIBAN();

		while (_context.BankAccounts.Any(ba => ba.IBAN == IBAN))
		{
			IBAN = GenerateIBAN();
		}


		BankAccount newAccount = new()
		{
			IBAN = IBAN,
			UnifiedCivilNumber = model.UnifiedCivilNumber,
			Address = model.Address
		};

		foundUser.BankAccounts.Add(newAccount);

		await _context.BankAccounts.AddAsync(newAccount);

		bool isGuest = await _userManager.IsInRoleAsync(foundUser, "Guest");

		if (isGuest)
		{
			await _userManager.RemoveFromRoleAsync(foundUser, "Guest");
			await _userManager.AddToRoleAsync(foundUser, "AccountHolder");
			await _signInManager.RefreshSignInAsync(foundUser);
		}

		await _context.SaveChangesAsync();

		await _emailSender.SendEmailAsync(
			foundUser.Email!,
			"NRB - New Bank Account",
			"Your new bank account is ready to use!");
	}

	public async Task<List<BankAccountDisplayViewModel>> GetAllUserAccountsAsync(string userName)
	{
		ApplicationUser? foundUser = await _context.Users
			.AsNoTracking()
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		return foundUser.BankAccounts
			.Where(ba => !ba.IsClosed)
			.Select(ba => new BankAccountDisplayViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance
			})
			.ToList();
	}

	public async Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(Guid id)
	{
		BankAccount? account = await _context.BankAccounts
			.AsNoTracking()
			.Include(a => a.TransactionHistory)
				.ThenInclude(th => th.AccountFrom)
			.Include(a => a.TransactionHistory)
				.ThenInclude(th => th.AccountTo)
			.SingleOrDefaultAsync(acc => acc.Id == id);

		ArgumentNullException.ThrowIfNull(account);

		return new BankAccountDetailsViewModel
		{
			Id = account.Id,
			IBAN = account.IBAN,
			Address = account.Address,
			Balance = account.Balance,
			UnifiedCivilNumber = account.UnifiedCivilNumber,
			TransactionHistory = account.TransactionHistory
					.Select(t =>
					{
						if (t.Description.Length >= 25)
						{
							t.Description = string.Join("",
								t.Description.Take(25)) + new string('.', 3);
						}

						return t;
					})
					.OrderByDescending(t => t.TransactionDate)
					.ToHashSet()
		};
	}

	public async Task<TransactionNewViewModel> PrepareTransactionModelForUserAsync(
		string userName)
	{
		ApplicationUser? foundUser = await _context.Users
			.AsNoTracking()
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		List<TransactionSenderViewModel> userAccounts = foundUser!.BankAccounts
			.Where(ba => !ba.IsClosed)
			.Select(ba => new TransactionSenderViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance,
			})
			.ToList();

		return new()
		{
			SenderAccounts = userAccounts
		};
	}

	public async Task<bool> CheckRoleAsync(string userName)
	{
		ApplicationUser? foundUser = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		IList<string> userRoles = await _userManager.GetRolesAsync(foundUser);
		await _userManager.RemoveFromRolesAsync(foundUser, userRoles);

		int accountsCount = foundUser.BankAccounts
			.Count(ba => !ba.IsClosed);

		if (accountsCount < 1)
		{
			await _userManager.AddToRoleAsync(foundUser, "Guest");
		}
		else
		{
			await _userManager.AddToRoleAsync(foundUser, "AccountHolder");
		}

		await _signInManager.RefreshSignInAsync(foundUser);

		return accountsCount > 0;
	}

	public async Task CloseAccountByIdAsync(Guid id)
	{
		BankAccount? account = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.Id == id);

		ArgumentNullException.ThrowIfNull(account);

		if (account.IsClosed)
		{
			return;
		}

		account.IsClosed = true;
		account.ClosedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync();
	}

	public async Task<PaymentResult> BeginPaymentAsync(TransactionNewViewModel model)
	{
		BankAccount? accountFrom = await _context.BankAccounts
			.Include(af => af.TransactionHistory)
			.SingleOrDefaultAsync(acc => acc.Id.ToString() == model.AccountFrom);

		BankAccount? accountTo = await _context.BankAccounts
			.Include(af => af.TransactionHistory)
			.SingleOrDefaultAsync(acc => acc.IBAN == model.AccountTo);

		if (accountFrom is null || accountFrom.IsClosed)
		{
			return PaymentResult.SenderNotFound;
		}

		if (accountTo is null || accountTo.IsClosed)
		{
			return PaymentResult.RecieverNotFound;
		}

		if (accountFrom == accountTo)
		{
			return PaymentResult.NoSelfTransactions;
		}

		ApplicationUser userFrom = await _context.Users
			.FirstAsync(u => u.BankAccounts.Any(ba => ba.Id == accountFrom.Id));

		ApplicationUser userTo = await _context.Users
			.FirstAsync(u => u.BankAccounts.Any(ba => ba.Id == accountTo.Id));

		if (accountFrom.Balance - model.Amount > 1)
		{
			IDbContextTransaction transaction = _context.Database.BeginTransaction();

			try
			{
				accountFrom.Balance -= model.Amount;

				accountTo.Balance += model.Amount;

				Transaction newTransac = await AddTransactionAsync(model, userFrom, userTo);

				accountFrom.TransactionHistory.Add(newTransac);
				accountTo.TransactionHistory.Add(newTransac);

				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				await _emailSender.SendEmailAsync(
					userFrom.Email!,
					"NRB - Successful Transaction",
					$"You successfully sent ${model.Amount} to {userTo.UserName}!");

				await _emailSender.SendEmailAsync(
					userTo.Email!,
					"NRB - Successful Transaction Received",
					$"You just recieved ${model.Amount} from {userTo.UserName}!");

				return PaymentResult.Successful;
			}
			catch
			{
				await transaction.RollbackAsync();
			}
		}

		return PaymentResult.InsufficientFunds;
	}

	private async Task<Transaction> AddTransactionAsync(
		TransactionNewViewModel model,
		ApplicationUser userFrom,
		ApplicationUser userTo)
	{
		Transaction transaction = new()
		{
			Description = model.Description,
			Amount = model.Amount,
			TransactionDate = DateTime.UtcNow,
			AccountFrom = userFrom,
			AccountFromId = userFrom.Id,
			AccountTo = userTo,
			AccountToId = userTo.Id
		};

		await _context.Transactions.AddAsync(transaction);

		await _context.SaveChangesAsync();

		return transaction;
	}

	private static string GenerateIBAN()
	{
		return $"BG{GenerateRandomIbanPart()}NRB{GenerateRandomIbanPart()}";
	}

	private static string GenerateRandomIbanPart()
	{
		int ibanPart = 10;
		string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

		char[] randomString = new char[ibanPart];

		Random random = new();

		for (int i = 0; i < ibanPart; i++)
		{
			randomString[i] = characters[random.Next(characters.Length)];
		}

		return new string(randomString);
	}

	public Task<List<BankAccountDisplayViewModel>> GetAllBankAccountsAsync()
		=> _context.BankAccounts
			.Select(ba => new BankAccountDisplayViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance
			})
			.ToListAsync();
}
