namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data.Models.Enums;
using static NewRevolutionaryBank.Common.ModelsConstants;

[Comment("Банкова сметка")]
public class BankAccount
{
	public BankAccount()
	{
		Id = Guid.NewGuid();
		Balance = 0m;
		IsClosed = false;
		Tier = BankAccountTier.Standard;
	}

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[Required]
	[Comment("ИБАН на сметката")]
	[StringLength(
		BankAccountConstants.IBAN_Length,
		MinimumLength = BankAccountConstants.IBAN_Length)]
	public string IBAN { get; set; } = null!;

	[Comment("Салдо на сметката")]
	public decimal Balance { get; set; }

	[Comment("Ниво на сметката")]
	public BankAccountTier Tier { get; set; }

	[Required]
	[ForeignKey(nameof(Owner))]
	[Comment("Уникален идентификатор на собственика на сметката")]
	public Guid OwnerId { get; set; }

	[Required]
	public ApplicationUser Owner { get; set; } = null!;

	[Required]
	[Comment("ЕГН на потребителя")]
	[StringLength(
		BankAccountConstants.UCN_Length,
		MinimumLength = BankAccountConstants.UCN_Length)]
	public string UnifiedCivilNumber { get; set; } = null!;

	[Required]
	[Comment("Адрес на потребителя")]
	[StringLength(
		BankAccountConstants.AddressMaxLength,
		MinimumLength = BankAccountConstants.AddressMinLength)]
	public string Address { get; set; } = null!;

	[Comment("Флаг дали сметката е закрита")]
	public bool IsClosed { get; set; }

	[Comment("Дата на закриване на сметка")]
	public DateTime? ClosedDate { get; set; }
}