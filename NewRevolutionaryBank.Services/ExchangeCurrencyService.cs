namespace NewRevolutionaryBank.Services;

using System.Net.Http;
using System.Net.Http.Json;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services.Contracts;

public class ExchangeCurrencyService : IExchangeCurrencyService
{
	private const string api_url =
		"https://v6.exchangerate-api.com/v6/fb5d7016f4fcc80e9c62c331/latest/USD";

	public async Task<ConversionRate> GetRates()
	{
		try
		{
			using HttpClient client = new();

			ExchangeApiResponse? response = await client
				.GetFromJsonAsync<ExchangeApiResponse>(api_url);

			return response!.conversion_rates;
		}
		catch
		{
			throw new InvalidOperationException("Unknown API error!");
		}
	}
}
