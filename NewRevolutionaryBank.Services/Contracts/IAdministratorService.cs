namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Administrator;

public interface IAdministratorService
{
	// -------------------------------
	//			Bank Accounts
	// -------------------------------

	Task ActivateBankAccountByIdAsync(string id);

	Task DeactivateBankAccountByIdAsync(string id);

	Task<List<BankAccountManageViewModel>> GetAllBankAccountsAsync();

	Task<BankAccountDetailsViewModel> GetBankAccountDetailsAsync(Guid id);

	// -------------------------------
	//			User Profiles
	// -------------------------------

	Task<List<UserProfileManageViewModel>> GetAllProfilesAsync();

	Task ActivateUserProfileByIdAsync(Guid id);

	Task DeactivateUserProfileByIdAsync(Guid id);

	// -------------------------------
	//			Transactions
	// -------------------------------

	Task<List<TransactionDisplayViewModel>> GetAllTransactionsAsync();
}
