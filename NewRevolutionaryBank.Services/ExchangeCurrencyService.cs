namespace NewRevolutionaryBank.Services;

using System.Net.Http;
using System.Net.Http.Json;

using Microsoft.Extensions.Configuration;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services.Contracts;

public class ExchangeCurrencyService : IExchangeCurrencyService
{
	private readonly string api_url;

	public ExchangeCurrencyService(IConfiguration configuration) =>
		api_url = configuration["ExchangeCurrencyApi:URL"]!;

	public async Task<ConversionRate> GetRates()
	{
		using HttpClient client = new();

		ExchangeApiResponse? response = await client
			.GetFromJsonAsync<ExchangeApiResponse>(api_url);

		return response!.conversion_rates;
	}
}
