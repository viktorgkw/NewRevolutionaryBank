namespace NewRevolutionaryBank.Services;

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

			// TODO: testvai bez dolniq red i polzvai foundUser instead
			ApplicationUser refreshedUser = (await _userManager.FindByIdAsync(foundUser.Id))!;
			await _signInManager.RefreshSignInAsync(refreshedUser);
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
}
