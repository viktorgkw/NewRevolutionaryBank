namespace NewRevolutionaryBank.Data.Models.ExchangeRates;

public class ExchangeApiResponse
{
	public string result { get; set; } = null!;

	public string time_last_update_utc { get; set; } = null!;

	public string time_next_update_utc { get; set; } = null!;

	public string base_code { get; set; } = null!;

	public ConversionRate conversion_rates { get; set; } = null!;
}
