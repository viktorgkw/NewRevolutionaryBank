namespace NewRevolutionaryBank.Data.Models;

using Microsoft.EntityFrameworkCore;

public class BankSettings
{
	[Comment("Такса за транзакция")]
	public decimal TransactionFee { get; set; }

	[Comment("Такса за обмен на валута")]
	public decimal CurrencyExchangeFee { get; set; }
}
