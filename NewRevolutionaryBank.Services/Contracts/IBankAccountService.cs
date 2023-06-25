namespace NewRevolutionaryBank.Services.Contracts;

using System;
using System.Collections.Generic;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

public interface IBankAccountService
{
	Task CreateAsync(
		string userName,
		BankAccountCreateViewModel model);

	Task<bool> CheckRoleAsync(
		string userName);

	Task<List<BankAccountDisplayViewModel>> GetAllUserAccountsAsync(
		string userName);

	Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(
		Guid id);

	Task<TransactionNewViewModel> PrepareTransactionModelForUserAsync(
		string userName);

	Task<PaymentResult> BeginPaymentAsync(TransactionNewViewModel model);

	Task CloseAccountByIdAsync(Guid id);

	Task<List<BankAccountDisplayViewModel>> GetAllBankAccountsAsync();
}
