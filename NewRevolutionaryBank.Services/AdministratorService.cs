namespace NewRevolutionaryBank.Services;

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Administrator;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public class AdministratorService : IAdministratorService
{
	private const string AdministratorUserName = "Administrator";

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
			.SingleOrDefaultAsync(acc => acc.Id == id);

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

	// ------------------------------------
	//				Profiles
	// ------------------------------------

	public async Task<List<UserProfileManageViewModel>> GetAllProfilesAsync(
		string order,
		string? searchName)
	{
		IQueryable<ApplicationUser> usersQuery = _context.Users
			.AsNoTracking()
			.Include(user => user.BankAccounts)
			.Where(user => user.UserName != AdministratorUserName);

		usersQuery = usersQuery = order switch
		{
			"active" => usersQuery.Where(u => !u.IsDeleted),
			"deleted" => usersQuery.Where(u => u.IsDeleted),
			_ => usersQuery,
		};

		return await usersQuery
			.Where(u => u.UserName!.ToLower().Contains(searchName ?? ""))
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
	}

	public async Task<UserProfileDetailsViewModel> GetUserProfileDetailsByIdAsync(Guid id)
	{
		ApplicationUser? user = await _context.Users
			.Include(u => u.BankAccounts)
			.FirstOrDefaultAsync(u => u.Id == id);

		ArgumentNullException.ThrowIfNull(user);

		return new UserProfileDetailsViewModel
		{
			Id = user.Id,
			Email = user.Email!,
			UserName = user.UserName!,
			FirstName = user.FirstName,
			LastName = user.LastName,
			PhoneNumber = user.PhoneNumber,
			CreatedOn = user.CreatedOn,
			IsDeleted = user.IsDeleted,
			DeletedOn = user.DeletedOn,
			BankAccounts = user.BankAccounts.ToList()
		};
	}

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
				AccountFromUsername = t.AccountFrom.IBAN,
				AccountToUsername = t.AccountTo.IBAN
			})
			.OrderByDescending(t => t.TransactionDate)
			.ToListAsync();

	// -------------------------------
	//			Bank Settings
	// -------------------------------

	public async Task<BankSettingsDisplayViewModel> GetBankSettingsAsync()
	{
		BankSettings settings = await _context.BankSettings.FirstAsync();

		return new BankSettingsDisplayViewModel
		{
			TransactionFee = settings.TransactionFee
		};
	}

	public async Task EditTransactionFeeAsync(decimal decimalValue)
	{
		BankSettings settings = await _context.BankSettings.FirstAsync();

		settings.TransactionFee = decimalValue;

		await _context.SaveChangesAsync();
	}
}
