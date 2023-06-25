namespace NewRevolutionaryBank.Data.Models;

public class StripePayment
{
	public string Id { get; set; } = null!;

	public string CustomerName { get; set; } = null!;

	public string CardNumber { get; set; } = null!;

	public string ExpYear { get; set; } = null!;

	public string ExpMonth { get; set; } = null!;

	public string CVC { get; set; } = null!;

	public long ChargeAmount { get; set; }

	public string Currency { get; set; } = null!;

	public string Description { get; set; } = null!;
}
