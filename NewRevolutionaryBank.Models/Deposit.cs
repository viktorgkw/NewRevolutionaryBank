namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using static NewRevolutionaryBank.Common.ModelsConstants;

[Comment("Банков депозит")]
public class Deposit
{
	public Deposit() => Id = Guid.NewGuid();

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[ForeignKey(nameof(AccountTo))]
	[Comment("Уникален идентификатор на получател")]
	public Guid AccountToId { get; set; }

    public BankAccount AccountTo { get; set; } = null!;

	[Range(DepositConstants.AmountMin, DepositConstants.AmountMax)]
	[Comment("Стойност на депозит")]
	public decimal Amount { get; set; }

	[Comment("CVC на картата с която се прави депозит")]
	public string CVC { get; set; } = null!;

	[Comment("Номер на картата с която се прави депозит")]
	public string CardNumber { get; set; } = null!;

	[Range(DepositConstants.ExpYearMin, DepositConstants.ExpYearMax)]
	[Comment("Година на изтичане на карата с която се прави депозит")]
	public string ExpYear { get; set; } = null!;

	[Range(DepositConstants.ExpMonthMin, DepositConstants.ExpMonthMax)]
	[Comment("Месец на изтичане на карата с която се прави депозит")]
	public string ExpMonth { get; set; } = null!;

	[Comment("Дата на направения депозит")]
	public DateTime DepositedAt { get; set; }
}
