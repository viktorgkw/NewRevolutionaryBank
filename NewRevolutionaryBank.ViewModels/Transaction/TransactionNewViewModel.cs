namespace NewRevolutionaryBank.ViewModels.Transaction;

using System.ComponentModel.DataAnnotations;

public class TransactionNewViewModel
{
	public decimal Amount { get; set; }

	[Required]
	[StringLength(120, MinimumLength = 5)]
	public string Description { get; set; } = null!;

	[Required]
	public string AccountFrom { get; set; } = null!;

	[Required]
	public string AccountTo { get; set; } = null!;

	[Required]
	public List<TransactionSenderViewModel> SenderAccounts { get; set; } = null!;
}
