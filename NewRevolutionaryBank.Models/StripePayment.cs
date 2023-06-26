namespace NewRevolutionaryBank.Data.Models;

public class StripePayment
{
	public string Id { get; set; } = null!;

	public string CardNumber { get; set; } = null!;

	public string ExpYear { get; set; } = null!;

	public string ExpMonth { get; set; } = null!;

	public string CVC { get; set; } = null!;
}
