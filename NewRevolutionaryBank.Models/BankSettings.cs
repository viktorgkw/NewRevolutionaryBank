namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

[Comment("Банкови настройки")]
public class BankSettings
{
	public BankSettings() => Id = Guid.NewGuid();

	[Key]
	[Comment("Уникален идентификатор")]
	public Guid Id { get; set; }

	[Comment("Такса за транзакция")]
	public decimal TransactionFee { get; set; }
}
