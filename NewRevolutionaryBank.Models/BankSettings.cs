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

	[Display(Name = "Transaction Fee")]
	[Comment("Такса за транзакция")]
	public decimal TransactionFee { get; set; }

	[Display(Name = "Bank Balance")]
	[Comment("Баланс на банката")]
	public decimal BankBalance { get; set; }

	[Display(Name = "Standard Tax")]
	[Comment("Месечна такса за Standard сметка")]
	public decimal StandardTax { get; set; }

	[Display(Name = "Premium Tax")]
	[Comment("Месечна такса за Premium сметка")]
	public decimal PremiumTax { get; set; }

	[Display(Name = "VIP Tax")]
	[Comment("Месечна такса за VIP сметка")]
	public decimal VipTax { get; set; }
}
