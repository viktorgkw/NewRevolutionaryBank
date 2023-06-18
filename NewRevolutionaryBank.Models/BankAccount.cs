namespace NewRevolutionaryBank.Models;

using System.ComponentModel.DataAnnotations;

public class BankAccount
{
    public BankAccount()
    {
		Id = Guid.NewGuid();
		Balance = 0m;
        TransactionHistory = new HashSet<Transaction>();
    }

    [Key]
    public Guid Id { get; set; }
	
	public decimal Balance { get; set; }

    [Required]
	[StringLength(10, MinimumLength = 10)]
	public string UnifiedCivilNumber { get; set; } = null!;

    [Required]
	[StringLength(40, MinimumLength = 8)]
	public string Address { get; set; } = null!;

	public ICollection<Transaction> TransactionHistory { get; set; }
}