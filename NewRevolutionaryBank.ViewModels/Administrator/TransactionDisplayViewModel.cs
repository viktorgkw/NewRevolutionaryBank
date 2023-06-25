namespace NewRevolutionaryBank.Web.ViewModels.Administrator;

public class TransactionDisplayViewModel
{
	public Guid Id { get; set; }

	public DateTime TransactionDate { get; set; }

	public decimal Amount { get; set; }

	public string Description { get; set; } = null!;

	public string AccountFromUsername { get; set; } = null!;

	public string AccountToUsername { get; set; } = null!;
}
