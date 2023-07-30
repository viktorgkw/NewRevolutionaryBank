namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Web.ViewModels.Administrator;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public interface IAdministratorService
{
	// -------------------------------
	//			Bank Accounts
	// -------------------------------

	Task ActivateBankAccountByIdAsync(string id);

	Task DeactivateBankAccountByIdAsync(string id);

	Task<List<BankAccountManageViewModel>> GetAllBankAccountsAsync();

	Task<BankAccountDetailsViewModel> GetBankAccountDetailsByIdAsync(Guid id);

	// -------------------------------
	//			User Profiles
	// -------------------------------

	Task<List<UserProfileManageViewModel>> GetAllProfilesAsync(
		string order,
		string? searchName);

	Task<UserProfileDetailsViewModel> GetUserProfileDetailsByIdAsync(Guid id);

	Task ActivateUserProfileByIdAsync(Guid id);

	Task DeactivateUserProfileByIdAsync(Guid id);

	// -------------------------------
	//			Transactions
	// -------------------------------

	Task<List<TransactionDisplayViewModel>> GetAllTransactionsAsync();

	// --------------------------------
	//			Bank Settings
	// --------------------------------

	Task<BankSettingsDisplayViewModel> GetBankSettingsAsync();

	Task EditTransactionFeeAsync(decimal decimalValue);

	Task EditStandardTaxAsync(decimal decimalValue);

	Task EditPremiumTaxAsync(decimal decimalValue);

	Task EditVipTaxAsync(decimal decimalValue);

	// ------------------------------
	//		Website Statistics
	// ------------------------------

	Task<WebsiteStatisticsViewModel> GetWebsiteStatisticsAsync();
}
