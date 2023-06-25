namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

public interface ITransactionService
{
	Task<TransactionNewViewModel> PrepareTransactionModelForUserAsync(
		string userName);

	Task<PaymentResult> BeginPaymentAsync(TransactionNewViewModel model);
}
