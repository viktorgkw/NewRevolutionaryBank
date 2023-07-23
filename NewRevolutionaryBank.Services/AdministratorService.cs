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

	/// <summary>
	/// Activates bank account by id if it's not null.
	/// </summary>
	/// <param name="id">Id of the bank account.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task ActivateBankAccountByIdAsync(string id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id.ToString() == id);

		ArgumentNullException.ThrowIfNull(bankAcc);

		bankAcc.IsClosed = false;
		bankAcc.ClosedDate = null;

		await _context.SaveChangesAsync();
	}

	/// <summary>
	/// Deactivates bank account by id if it's not null.
	/// </summary>
	/// <param name="id">Id of the bank account.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task DeactivateBankAccountByIdAsync(string id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id.ToString() == id);

		ArgumentNullException.ThrowIfNull(bankAcc);

		bankAcc.IsClosed = true;
		bankAcc.ClosedDate = DateTime.UtcNow;

		await _context.SaveChangesAsync();
	}

	/// <returns>List of all the bank accounts.</returns>
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

	/// <param name="id">Id of the bank account.</param>
	/// <returns>Bank account details.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task<BankAccountDetailsViewModel> GetBankAccountDetailsByIdAsync(Guid id)
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

	/// <summary>
	/// This methods returns all the profiles by given order and search name if given.
	/// </summary>
	/// <param name="order">Order of the profiles.</param>
	/// <param name="searchName">Optional string that could be contained in a username.</param>
	/// <returns>All user profiles.</returns>
	public async Task<List<UserProfileManageViewModel>> GetAllProfilesAsync(
		string order,
		string? searchName)
	{
		searchName = searchName?.ToLower() ?? string.Empty;

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

		List<UserProfileManageViewModel> result = await usersQuery
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

		return result;
	}

	/// <param name="id">The id of the user profile.</param>
	/// <returns>The found user profile.</returns>
	/// <exception cref="ArgumentNullException"></exception>
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

	/// <summary>
	/// Activates the user profile by id or throws exception if not found.
	/// </summary>
	/// <param name="id">The user profile id.</param>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task ActivateUserProfileByIdAsync(Guid id)
	{
		ApplicationUser? user = await _context.Users
			.FirstOrDefaultAsync(u => u.Id == id);

		ArgumentNullException.ThrowIfNull(user);

		user.IsDeleted = false;
		user.DeletedOn = null;

		await _context.SaveChangesAsync();
	}

	/// <summary>
	/// Deactivates the user profile or throws exception if not found.
	/// </summary>
	/// <param name="id">Id of the user profile.</param>
	/// <exception cref="ArgumentNullException"></exception>
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

	/// <returns>All the transactions if any.</returns>
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

	/// <returns>All the bank settings.</returns>
	public async Task<BankSettingsDisplayViewModel> GetBankSettingsAsync()
	{
		BankSettings settings = await _context.BankSettings.FirstAsync();

		return new BankSettingsDisplayViewModel
		{
			TransactionFee = settings.TransactionFee,
			MonthlyTax = settings.MonthlyTax,
			BankBalance = settings.BankBalance
		};
	}

	/// <summary>
	/// Edits the transaction fee with a given value.
	/// </summary>
	/// <param name="decimalValue">New fee value.</param>
	public async Task EditTransactionFeeAsync(decimal decimalValue)
	{
		BankSettings settings = await _context.BankSettings.FirstAsync();

		settings.TransactionFee = decimalValue;

		await _context.SaveChangesAsync();
	}

	/// <returns>View model with statistics for the website.</returns>
	public async Task<WebsiteStatisticsViewModel> GetWebsiteStatisticsAsync()
	{
		int newUsers = await _context.Users.AnyAsync()
			? await _context.Users.CountAsync(u => u.CreatedOn.Day == DateTime.Now.Day)
			: 0;

		decimal averageDepositPrice = await _context.Deposits.AnyAsync()
			? await _context.Deposits.AverageAsync(d => d.Amount)
			: 0.00m;

		decimal averageTransactionPrice = await _context.Transactions.AnyAsync()
			? await _context.Transactions.AverageAsync(t => t.Amount)
			: 0.00m;

		double averageWebsiteReviewRate = await _context.Ratings.AnyAsync()
			? await _context.Ratings.AverageAsync(r => r.RateValue)
			: 0.00d;

		return new()
		{
			TotalRegisteredUsers = await _context.Users.CountAsync(),
			NewUsers = newUsers,
			TotalBankAccounts = await _context.BankAccounts.CountAsync(),
			TotalDeposits = await _context.Deposits.CountAsync(),
			AverageDepositPrice = averageDepositPrice,
			TotalTransactions = await _context.Transactions.CountAsync(),
			AverageTransactionPrice = averageTransactionPrice,
			TotalReviews = await _context.Ratings.CountAsync(),
			AverageWebsiteReviewRate = averageWebsiteReviewRate
		};
	}

	/// <summary>
	/// Edits the monthly tax with a given value.
	/// </summary>
	/// <param name="decimalValue">New tax value.</param>
	public async Task EditMonthlyTaxAsync(decimal decimalValue)
	{
		BankSettings settings = await _context.BankSettings.FirstAsync();

		settings.MonthlyTax = decimalValue;

		await _context.SaveChangesAsync();
	}
}
