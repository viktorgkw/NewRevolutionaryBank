﻿namespace NewRevolutionaryBank.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
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
			.FirstOrDefaultAsync(acc => acc.Id == id);

		ArgumentNullException.ThrowIfNull(account);

		List<Transaction> RecievedTransactions = await _context.Transactions
			.AsNoTracking()
			.Include(t => t.AccountTo)
			.Include(t => t.AccountFrom)
			.Where(t => t.AccountTo == account)
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();

		List<Transaction> SentTransactions = await _context.Transactions
			.AsNoTracking()
			.Include(t => t.AccountTo)
			.Include(t => t.AccountFrom)
			.Where(t => t.AccountFrom == account)
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();

		return new BankAccountDetailsViewModel
		{
			Id = account.Id,
			IBAN = account.IBAN,
			Address = account.Address,
			Balance = account.Balance,
			UnifiedCivilNumber = account.UnifiedCivilNumber,
			SentTransactions = SentTransactions.ToHashSet(),
			RecievedTransactions = RecievedTransactions.ToHashSet()
		};
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

	private static string GenerateIBAN() =>
		$"BG{GenerateRandomIbanPart()}NRB{GenerateRandomIbanPart()}";

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

			int userBankAccounts = user.BankAccounts
			.Count(ba => !ba.IsClosed);

			if (userBankAccounts == 0)
			{
				IList<string> roles = await _userManager.GetRolesAsync(user);

				await _userManager.RemoveFromRolesAsync(user, roles);

				await _userManager.AddToRoleAsync(user, "Guest");

				await _signInManager.RefreshSignInAsync(user);
			}
		}
	}

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
				Id = Guid.NewGuid().ToString(),
				Currency = "usd",
				Description = "Deposit into bank account"
			}
		};
	}

	public async Task DepositAsync(DepositViewModel model)
	{
		try
		{
			_stripeService.MakePaymentAsync(model.StripePayment);
		}
		catch
		{
			// It will always explode because it's not configured for real environment.
		}

		BankAccount? bankAcc = await _context.BankAccounts
						.FirstOrDefaultAsync(ba => ba.Id == model.DepositTo);

		if (bankAcc is null)
		{
			return;
		}

		bankAcc.Balance += model.Amount;

		await _context.SaveChangesAsync();
	}
}
