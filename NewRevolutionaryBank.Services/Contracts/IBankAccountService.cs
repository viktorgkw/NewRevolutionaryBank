namespace NewRevolutionaryBank.Services.Contracts;

using System;
using System.Collections.Generic;

using NewRevolutionaryBank.Web.ViewModels.BankAccount;

public interface IBankAccountService
{
	Task CreateAsync(
		string userName,
		BankAccountCreateViewModel model);

	Task<List<BankAccountDisplayViewModel>> GetAllUserAccountsAsync(
		string userName);

	Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(
		Guid id);

	Task CloseAccountByIdAsync(Guid id);

	Task<List<BankAccountDisplayViewModel>> GetAllBankAccountsAsync();

	Task<DepositViewModel> PrepareDepositViewModel(string userName);

	Task DepositAsync(DepositViewModel model);
}
