namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

[Comment("Банква сметка")]
public class BankAccount
{
	public BankAccount()
	{
		Id = Guid.NewGuid();
		Balance = 0m;
		IsClosed = false;
	}

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[Required]
	[Comment("ИБАН на сметката")]
	[StringLength(25, MinimumLength = 25)]
	public string IBAN { get; set; } = null!;

	[Comment("Салдо на сметката")]
	public decimal Balance { get; set; }

	[Required]
	[ForeignKey(nameof(Owner))]
	[Comment("Уникален идентификатор на собственика на сметката")]
	public Guid OwnerId { get; set; }

	[Required]
	public ApplicationUser Owner { get; set; } = null!;

	[Required]
	[Comment("ЕГН на потребителя")]
	[StringLength(10, MinimumLength = 10)]
	public string UnifiedCivilNumber { get; set; } = null!;

	[Required]
	[Comment("Адрес на потребителя")]
	[StringLength(40, MinimumLength = 8)]
	public string Address { get; set; } = null!;

	[Comment("Флаг дали сметката е закрита")]
	public bool IsClosed { get; set; }

	[Comment("Дата на закриване на сметка")]
	public DateTime? ClosedDate { get; set; }
}