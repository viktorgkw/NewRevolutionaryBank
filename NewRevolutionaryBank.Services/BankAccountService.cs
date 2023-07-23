namespace NewRevolutionaryBank.Services;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public class BankAccountService : IBankAccountService
{
	private readonly NrbDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IStripeService _stripeService;
	private readonly IEmailSender _emailSender;

	public BankAccountService(
		NrbDbContext context,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IEmailSender emailSender,
		IStripeService stripeService)
	{
		_context = context;
		_userManager = userManager;
		_signInManager = signInManager;
		_emailSender = emailSender;
		_stripeService = stripeService;
	}

	/// <summary>
	/// Creates the bank account if provided data is valid.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
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
			Address = model.Address,
			Tier = model.Tier
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

	/// <param name="userName">Username of bank accounts owner.</param>
	/// <returns>List of the user bank accounts if any.</returns>
	/// <exception cref="ArgumentNullException"></exception>
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
				Balance = ba.Balance,
				Tier = ba.Tier
			})
			.ToList();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(
		Guid id,
		string userName)
	{
		BankAccount? account = await _context.BankAccounts
			.AsNoTracking()
			.Include(acc => acc.Owner)
			.FirstOrDefaultAsync(acc => acc.Id == id);

		ArgumentNullException.ThrowIfNull(account);

		ApplicationUser? foundUser = await _context.Users
			.FirstOrDefaultAsync(u => u.UserName == userName);

		ArgumentNullException.ThrowIfNull(foundUser);

		if (account.Owner.Id != foundUser.Id)
		{
			throw new InvalidOperationException();
		}

		List<Transaction> RecievedTransactions = await _context.Transactions
			.AsNoTracking()
			.Include(t => t.AccountTo)
			.Include(t => t.AccountFrom)
			.Where(t => t.AccountToId == account.Id)
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();

		List<Transaction> SentTransactions = await _context.Transactions
			.AsNoTracking()
			.Include(t => t.AccountTo)
			.Include(t => t.AccountFrom)
			.Where(t => t.AccountFromId == account.Id)
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();

		List<Deposit> Deposits = await _context.Deposits
			.AsNoTracking()
			.Where(d => d.AccountToId == account.Id)
			.OrderByDescending(d => d.DepositedAt)
			.ToListAsync();

		return new BankAccountDetailsViewModel
		{
			Id = account.Id,
			IBAN = account.IBAN,
			Address = account.Address,
			Balance = account.Balance,
			UnifiedCivilNumber = account.UnifiedCivilNumber,
			SentTransactions = SentTransactions.ToHashSet(),
			RecievedTransactions = RecievedTransactions.ToHashSet(),
			Deposits = Deposits.ToHashSet(),
		};
	}

	/// <summary>
	/// Closes a bank account.
	/// </summary>
	/// <param name="id">Id of the bank account.</param>
	/// <exception cref="ArgumentNullException"></exception>
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

	/// <returns>All bank accounts.</returns>
	public Task<List<BankAccountDisplayViewModel>> GetAllBankAccountsAsync()
		=> _context.BankAccounts
			.Select(ba => new BankAccountDisplayViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance
			})
			.ToListAsync();

	/// <returns>Model for depositing amount into bank account.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task<DepositViewModel> PrepareDepositViewModel(string userName)
	{
		ApplicationUser? user = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(u => u.UserName == userName);

		ArgumentNullException.ThrowIfNull(user);

		List<BankAccountDepositViewModel> userAccounts = user.BankAccounts
			.Where(ba => !ba.IsClosed)
			.Select(ba => new BankAccountDepositViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				Balance = ba.Balance
			})
			.ToList();

		return new DepositViewModel
		{
			MyAccounts = userAccounts,
			StripePayment = new()
			{
				Id = Guid.NewGuid().ToString()
			}
		};
	}

	/// <summary>
	/// Deposits amount into bank account.
	/// </summary>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task DepositAsync(DepositViewModel model)
	{
		ArgumentNullException.ThrowIfNull(model);

		ArgumentNullException.ThrowIfNull(model.StripePayment);

		string paymentResult = _stripeService.MakePayment(model.StripePayment);

		if (paymentResult == "succeeded")
		{
			BankAccount? bankAcc = await _context.BankAccounts
						.FirstOrDefaultAsync(ba => ba.Id == model.DepositTo);

			if (bankAcc is null || model.Amount <= 0)
			{
				return;
			}

			bankAcc.Balance += model.Amount;

			await _context.Deposits.AddAsync(new()
			{
				AccountTo = bankAcc,
				AccountToId = bankAcc.Id,
				Amount = model.Amount,
				CVC = model.StripePayment.CVC,
				CardNumber = model.StripePayment.CardNumber,
				ExpYear = model.StripePayment.ExpYear,
				ExpMonth = model.StripePayment.ExpMonth,
				DepositedAt = DateTime.UtcNow
			});

			await _context.SaveChangesAsync();
		}
	}

	/// <param name="id">Id of the bank account.</param>
	/// <param name="userName">Username of the user.</param>
	/// <returns>True or false if the user is owner of a bank account.</returns>
	public Task<bool> IsOwner(Guid id, string userName) =>
		_context.BankAccounts
			.AnyAsync(ba => ba.Id == id && ba.Owner.UserName == userName);

	/// <summary>
	/// Checks if the user role is correct,
	/// because if the user has no bank accounts
	/// his role will be switched back to Guest.
	/// </summary>
	public async Task CheckUserRole(ClaimsPrincipal User)
	{
		if (User.IsInRole("AccountHolder"))
		{
			ApplicationUser? user = await _context.Users
				.Include(u => u.BankAccounts)
				.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

			if (user is null)
			{
				return;
			}

			int userBankAccounts = user.BankAccounts.Count(ba => !ba.IsClosed);

			if (userBankAccounts == 0)
			{
				await _userManager.RemoveFromRoleAsync(user, "AccountHolder");
				await _userManager.AddToRoleAsync(user, "Guest");
				await _signInManager.RefreshSignInAsync(user);
			}
		}
	}

	/// <summary>
	/// Generates unique IBAN for a bank account.
	/// </summary>
	/// <returns>The generated unique IBAN.</returns>
	private static string GenerateIBAN() =>
		$"BG{GenerateRandomIbanPart()}NRB{GenerateRandomIbanPart()}";

	/// <summary>
	/// Generates single part of the IBAN.
	/// </summary>
	/// <returns>The generated part for the IBAN.</returns>
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

	public BankAccountCreateViewModel GetCreateViewModel() =>
		new()
		{
			Tiers = new()
				{
					BankAccountTier.Standard,
					BankAccountTier.Premium,
					BankAccountTier.VIP
				}
		};
}
