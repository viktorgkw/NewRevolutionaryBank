namespace NewRevolutionaryBank.Models;

using System.ComponentModel.DataAnnotations;

public class BankAccount
{
    public BankAccount()
    {
		Id = Guid.NewGuid();
		Balance = 0m;
		IsClosed = false;
        TransactionHistory = new HashSet<Transaction>();
    }

    [Key]
    public Guid Id { get; set; }

	[Required]
	[StringLength(25, MinimumLength = 25)]
	public string IBAN { get; set; } = null!;

	public decimal Balance { get; set; }

    [Required]
	[StringLength(10, MinimumLength = 10)]
	public string UnifiedCivilNumber { get; set; } = null!;

    [Required]
	[StringLength(40, MinimumLength = 8)]
	public string Address { get; set; } = null!;

    public bool IsClosed { get; set; }

    public DateTime? ClosedDate { get; set; }

    public ICollection<Transaction> TransactionHistory { get; set; }
}