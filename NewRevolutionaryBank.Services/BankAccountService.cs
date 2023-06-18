namespace NewRevolutionaryBank.Services;

using System.Threading.Tasks;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels;

public class BankAccountService : IBankAccountService
{
    private readonly ApplicationDbContext _context;

	public BankAccountService(
		ApplicationDbContext context)
	{
		_context = context;
	}

	public async Task Create(BankAccountCreateViewModel model)
	{
		BankAccount newAccount = new()
		{
			UnifiedCivilNumber = model.UnifiedCivilNumber,
			Address = model.Address
		};

		await _context.BankAccounts.AddAsync(newAccount);
		await _context.SaveChangesAsync();
	}
}
