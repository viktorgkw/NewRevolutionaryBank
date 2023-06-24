namespace NewRevolutionaryBank.Services;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels.BankAccount;

public class AdministratorService : IAdministratorService
{
    private readonly ApplicationDbContext _context;

	public AdministratorService(ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task ActivateBankAccountByIdAsync(string id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
			.FirstOrDefaultAsync(ba => ba.Id.ToString() == id);

		if (bankAcc is null || !bankAcc.IsClosed)
		{
			return;
		}

		bankAcc.IsClosed = false;
		bankAcc.ClosedDate = null;

		await _context.SaveChangesAsync();
	}

	public async Task<List<BankAccountDisplayViewModel>> GetAllBankAccounts()
		=> await _context.BankAccounts
		.Select(ba => new BankAccountDisplayViewModel
		{
			Id = ba.Id,
			IBAN = ba.IBAN,
			IsClosed = ba.IsClosed,
			Balance = ba.Balance
		})
		.ToListAsync();

	public async Task<BankAccountDetailsViewModel> GetBankAccountDetails(Guid id)
	{
		BankAccount? bankAcc = await _context.BankAccounts
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
}
