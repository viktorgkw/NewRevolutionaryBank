namespace NewRevolutionaryBank.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Models;
using NewRevolutionaryBank.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.ViewModels.BankAccount;
using NewRevolutionaryBank.ViewModels.Transaction;

public class BankAccountService : IBankAccountService
{
	private readonly ApplicationDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IEmailSender _emailSender;

	public BankAccountService(
		ApplicationDbContext context,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IEmailSender emailSender)
	{
		_context = context;
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
	}

	public async Task Create(string userName, BankAccountCreateViewModel model)
	{
		ApplicationUser? foundUser = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(user => user.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		if (foundUser.BankAccounts.Count >= 5)
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

	public async Task<List<BankAccountDisplayViewModel>> GetAllUserAccounts(string userName)
	{
		ApplicationUser? foundUser = await _context.Users
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
			.SingleOrDefaultAsync(acc => acc.Id == id);

		if (account is null || account.IsClosed)
		{
			return null;
		}

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

		var userRoles = await _userManager.GetRolesAsync(foundUser);
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

	public async Task<PaymentResult> BeginPaymentAsync(
		string accountFromId,
		string accountToIban,
		decimal amount)
	{
		BankAccount? accountFrom = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.Id.ToString() == accountFromId);

		BankAccount? accountTo = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.IBAN == accountToIban);

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

		if (accountFrom.Balance - amount > 1)
		{
			var transaction = _context.Database.BeginTransaction();

			try
			{
				accountFrom.Balance -= amount;

				accountTo.Balance += amount;

				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				await _emailSender.SendEmailAsync(
					userFrom.Email!,
					"NRB - Successful Transaction",
					$"You successfully sent ${amount} to {userTo.UserName}!");

				await _emailSender.SendEmailAsync(
					userTo.Email!,
					"NRB - Successful Transaction Received",
					$"You just recieved ${amount} from {userTo.UserName}!");

				return PaymentResult.Successful;
			}
			catch
			{
				await transaction.RollbackAsync();
			}
		}

		return PaymentResult.InsufficientFunds;
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
}
