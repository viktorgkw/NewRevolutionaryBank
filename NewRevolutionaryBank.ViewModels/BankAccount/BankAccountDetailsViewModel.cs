namespace NewRevolutionaryBank.ViewModels.BankAccount;

using NewRevolutionaryBank.Models;

public class BankAccountDetailsViewModel
{
    public Guid Id { get; set; }

	public string IBAN { get; set; } = null!;

	public decimal Balance { get; set; }

    public string UnifiedCivilNumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public HashSet<Transaction> TransactionHistory { get; set; } = null!;
}
