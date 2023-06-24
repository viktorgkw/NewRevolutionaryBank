namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Administrator;

public interface IAdministratorService
{
	Task ActivateBankAccountByIdAsync(string id);

	Task<List<BankAccountManageViewModel>> GetAllBankAccounts();

	Task<BankAccountDetailsViewModel> GetBankAccountDetails(Guid id);

	Task<List<UserProfileManageViewModel>> GetAllProfilesAsync();
}
