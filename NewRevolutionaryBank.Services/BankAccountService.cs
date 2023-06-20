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

		bool isGuest = await _userManager.IsInRoleAsync(foundUser, "Guest");

		if (isGuest)
		{
			await _userManager.RemoveFromRoleAsync(foundUser, "Guest");
			await _userManager.AddToRoleAsync(foundUser, "AccountHolder");
			await _signInManager.RefreshSignInAsync(foundUser);
		}

		BankAccount newAccount = new()
		{
			UnifiedCivilNumber = model.UnifiedCivilNumber,
			Address = model.Address
		};

		foundUser.BankAccounts.Add(newAccount);

		await _context.BankAccounts.AddAsync(newAccount);
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
			.Select(ba => new BankAccountDisplayViewModel
			{
				Id = ba.Id,
				Balance = ba.Balance
			})
			.ToList();
	}

	public async Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(Guid id)
	{
		BankAccount? account = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.Id == id);

		return account is null
			? null
			: new BankAccountDetailsViewModel
			{
				Id = account.Id,
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
			.Select(ba => new TransactionSenderViewModel
			{
				Id = ba.Id,
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

		if (foundUser.BankAccounts.Count < 1)
		{
			await _userManager.AddToRoleAsync(foundUser, "Guest");
		}
		else
		{
			await _userManager.AddToRoleAsync(foundUser, "AccountHolder");
		}

		await _signInManager.RefreshSignInAsync(foundUser);

		return foundUser.BankAccounts.Count > 0;
	}

	public async Task<PaymentResult> BeginPaymentAsync(
		string accountFromId,
		string accountToId,
		decimal amount)
	{
		BankAccount? accountFrom = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.Id.ToString() == accountFromId);

		BankAccount? accountTo = await _context.BankAccounts
			.SingleOrDefaultAsync(acc => acc.Id.ToString() == accountToId);

		if (accountFrom is null)
		{
			return PaymentResult.SenderNotFound;
		}

		if (accountTo is null)
		{
			return PaymentResult.RecieverNotFound;
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
}
