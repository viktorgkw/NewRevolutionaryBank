namespace NewRevolutionaryBank.Services;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels;

public class BankAccountService : IBankAccountService
{
	private readonly ApplicationDbContext _context;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public BankAccountService(
		ApplicationDbContext context,
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_context = context;
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public async Task Create(string userName, BankAccountCreateViewModel model)
	{
		ApplicationUser foundUser = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstAsync(user => user.UserName == userName);

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
	}

	public async Task<List<BankAccountDisplayViewModel>> GetAllUserAccounts(string userName)
	{
		ApplicationUser foundUser = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstAsync(user => user.UserName == userName);

		return foundUser.BankAccounts
			.Select(ba => new BankAccountDisplayViewModel
			{
				Id = ba.Id,
				SecureId = ba.Id.ToString()![..5] + new string('*', 10),
				Balance = ba.Balance
			})
			.ToList();
	}

	public async Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(Guid id)
	{
		BankAccount? account = await _context.BankAccounts
			.FirstOrDefaultAsync(acc => acc.Id == id);

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

	public async Task RemoveHolderRole(string userName)
	{
		ApplicationUser? foundUser = await _userManager
			.FindByNameAsync(userName);

		if (foundUser is null)
		{
			return;
		}

        if (foundUser.BankAccounts.Count < 1)
        {
			await _userManager.RemoveFromRoleAsync(foundUser, "AccountHolder");
			await _userManager.AddToRoleAsync(foundUser, "Guest");
			await _signInManager.RefreshSignInAsync(foundUser);
		}
    }
}
