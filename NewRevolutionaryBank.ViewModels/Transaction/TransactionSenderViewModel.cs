namespace NewRevolutionaryBank.ViewModels.Transaction;

public class TransactionSenderViewModel
{
	public Guid Id { get; set; }

	public string IBAN { get; set; } = null!;

	public decimal Balance { get; set; }
}
