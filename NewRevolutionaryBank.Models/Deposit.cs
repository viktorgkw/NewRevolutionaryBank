namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations.Schema;

public class Deposit
{
	public Deposit() => Id = Guid.NewGuid();

	public Guid Id { get; set; }

	[ForeignKey(nameof(AccountTo))]
    public Guid AccountToId { get; set; }

    public BankAccount AccountTo { get; set; } = null!;

	public decimal Amount { get; set; }

	public string CVC { get; set; } = null!;

	public string CardNumber { get; set; } = null!;

	public string ExpYear { get; set; } = null!;

	public string ExpMonth { get; set; } = null!;

    public DateTime DepositedAt { get; set; }
}
