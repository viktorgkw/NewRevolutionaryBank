namespace NewRevolutionaryBank.Data.Models.ExchangeRates;

using System.Text.Json.Serialization;

public class ExchangeApiResponse
{
	[JsonPropertyName("result")]
	public string Result { get; set; } = null!;

	[JsonPropertyName("time_last_update_utc")]
	public string TimeLastUpdateUtc { get; set; } = null!;

	[JsonPropertyName("time_next_update_utc")]
	public string TimeNextUpdateUtc { get; set; } = null!;

	[JsonPropertyName("base_code")]
	public string BaseCode { get; set; } = null!;

	[JsonPropertyName("conversion_rates")]
	public ConversionRate ConversionRates { get; set; } = null!;
}

