namespace NewRevolutionaryBank.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Administrator;

public class AdministratorService : IAdministratorService
{
	private readonly NrbDbContext _context;

	public AdministratorService(NrbDbContext context) => _context = context;

	// ------------------------------------
	//				Bank Accounts
	// ------------------------------------

	public async Task ActivateBankAccountByIdAsync(string id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id.ToString() == id);

		ArgumentNullException.ThrowIfNull(bankAcc);

		bankAcc.IsClosed = false;
		bankAcc.ClosedDate = null;

		await _context.SaveChangesAsync();
	}

	public async Task DeactivateBankAccountByIdAsync(string id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id.ToString() == id);

		ArgumentNullException.ThrowIfNull(bankAcc);

		bankAcc.IsClosed = true;
		bankAcc.ClosedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync();
	}

	public async Task<List<BankAccountManageViewModel>> GetAllBankAccountsAsync() =>
		await _context.BankAccounts
			.AsNoTracking()
			.Include(ba => ba.Owner)
			.Select(ba => new BankAccountManageViewModel
			{
				Id = ba.Id,
				IBAN = ba.IBAN,
				OwnerUsername = ba.Owner.UserName!,
				IsClosed = ba.IsClosed,
				Balance = ba.Balance
			})
			.ToListAsync();

	public async Task<BankAccountDetailsViewModel> GetBankAccountDetailsAsync(Guid id)
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

	// ------------------------------------
	//				Profiles
	// ------------------------------------

	public async Task<List<UserProfileManageViewModel>> GetAllProfilesAsync() =>
		await _context.Users
			.AsNoTracking()
			.Include(user => user.BankAccounts)
			.Select(user => new UserProfileManageViewModel
			{
				Id = user.Id,
				UserName = user.UserName!,
				FirstName = user.FirstName,
				LastName = user.LastName,
				IsDeleted = user.IsDeleted,
				DeletedOn = user.DeletedOn,
				BankAccountsCount = user.BankAccounts.Count
			})
			.ToListAsync();

	public async Task ActivateUserProfileByIdAsync(Guid id)
	{
		ApplicationUser? user = await _context.Users
			.FirstOrDefaultAsync(u => u.Id == id);

		ArgumentNullException.ThrowIfNull(user);

		user.IsDeleted = false;
		user.DeletedOn = null;

		await _context.SaveChangesAsync();
	}

	public async Task DeactivateUserProfileByIdAsync(Guid id)
	{
		ApplicationUser? user = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(u => u.Id == id);

		ArgumentNullException.ThrowIfNull(user);

		foreach (BankAccount bankAcc in user.BankAccounts)
		{
			bankAcc.IsClosed = true;
		}

		user.IsDeleted = true;
		user.DeletedOn = DateTime.Now;

		await _context.SaveChangesAsync();
	}

	// -------------------------------
	//			Transactions
	// -------------------------------

	public async Task<List<TransactionDisplayViewModel>> GetAllTransactionsAsync() =>
		await _context.Transactions
			.Select(t => new TransactionDisplayViewModel
			{
				Id = t.Id,
				Amount = t.Amount,
				Description = t.Description,
				TransactionDate = t.TransactionDate,
				AccountFromUsername = t.AccountFrom.UserName!,
				AccountToUsername = t.AccountTo.UserName!
			})
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();
}
