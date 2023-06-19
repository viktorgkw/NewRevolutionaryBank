namespace NewRevolutionaryBank.Services.Contracts;

using System;
using System.Collections.Generic;

using NewRevolutionaryBank.ViewModels;

public interface IBankAccountService
{
	Task Create(string userName, BankAccountCreateViewModel model);

	Task<List<BankAccountDisplayViewModel>> GetAllUserAccounts(string userName);

	Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(Guid id);

	Task RemoveHolderRole(string userName);
}
