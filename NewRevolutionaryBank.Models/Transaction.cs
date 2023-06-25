namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

public class Transaction
{
	public Transaction() => Id = Guid.NewGuid();

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[Comment("Дата на транзакцията")]
	public DateTime TransactionDate { get; set; }

	[Comment("Сума на транзакцията")]
	public decimal Amount { get; set; }

	[Required]
	[Comment("Описание на транзакцията")]
	[StringLength(120, MinimumLength = 5)]
	public string Description { get; set; } = null!;

	[Required]
	[Comment("Уникален идентификатор на предавателя")]
	[ForeignKey(nameof(AccountFrom))]
	public Guid AccountFromId { get; set; }

	[Required]
	public BankAccount AccountFrom { get; set; } = null!;

	[Required]
	[ForeignKey(nameof(AccountTo))]
	[Comment("Уникален идентификатор на получателя")]
	public Guid AccountToId { get; set; }

	[Required]
	public BankAccount AccountTo { get; set; } = null!;
}
