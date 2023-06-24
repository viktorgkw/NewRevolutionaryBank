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

	public AdministratorService(NrbDbContext context)
	{
		_context = context;
	}

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

	public async Task<List<BankAccountManageViewModel>> GetAllBankAccounts() =>
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

	public async Task<BankAccountDetailsViewModel> GetBankAccountDetails(Guid id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.AsNoTracking()
			.FirstOrDefaultAsync(ba => ba.Id == id);

		ArgumentNullException.ThrowIfNull(bankAcc);

		return new BankAccountDetailsViewModel
		{
			Id = bankAcc.Id,
			IBAN = bankAcc.IBAN,
			Balance = bankAcc.Balance,
			Address = bankAcc.Address,
			UnifiedCivilNumber = bankAcc.UnifiedCivilNumber,
			TransactionHistory = bankAcc.TransactionHistory.ToHashSet()
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
}
