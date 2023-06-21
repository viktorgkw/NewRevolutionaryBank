namespace NewRevolutionaryBank.Services.Contracts;

using System;
using System.Collections.Generic;

using NewRevolutionaryBank.Models.Enums;
using NewRevolutionaryBank.ViewModels.BankAccount;
using NewRevolutionaryBank.ViewModels.Transaction;

public interface IBankAccountService
{
	Task Create(
		string userName,
		BankAccountCreateViewModel model);

	Task<bool> CheckRoleAsync(
		string userName);

	Task<List<BankAccountDisplayViewModel>> GetAllUserAccounts(
		string userName);

	Task<BankAccountDetailsViewModel?> GetDetailsByIdAsync(
		Guid id);

	Task<TransactionNewViewModel> PrepareTransactionModelForUserAsync(
		string userName);

	Task<PaymentResult> BeginPaymentAsync(
		string accountFromId,
		string accountToId,
		decimal amount);

	Task CloseAccountByIdAsync(Guid id);
}
