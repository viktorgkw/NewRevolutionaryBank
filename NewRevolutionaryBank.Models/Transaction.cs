namespace NewRevolutionaryBank.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Transaction
{
	public Transaction()
	{
		Id = Guid.NewGuid();
	}

	[Key]
	public Guid Id { get; set; }

	public DateTime TransactionDate { get; set; }

	public decimal Amount { get; set; }

	[Required]
	[StringLength(120, MinimumLength = 5)]
	public string Description { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(AccountFrom))]
	public string AccountFromId { get; set; } = null!;

	[Required]
	public ApplicationUser AccountFrom { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(AccountTo))]
	public string AccountToId { get; set; } = null!;

	[Required]
	public ApplicationUser AccountTo { get; set; } = null!;
}
