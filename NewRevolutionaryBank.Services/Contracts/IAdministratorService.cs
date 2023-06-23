namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.ViewModels.BankAccount;

public interface IAdministratorService
{
	Task<List<BankAccountDisplayViewModel>> GetAllBankAccounts();

	Task<BankAccountDetailsViewModel> GetBankAccountDetails(Guid id);
}
