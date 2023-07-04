namespace NewRevolutionaryBank.Tests.Services;

using System.Reflection;

using Microsoft.Extensions.Configuration;
using Xunit;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services;
using Newtonsoft.Json.Linq;

public class ExchangeCurrencyServiceTests
{
	private readonly IConfigurationRoot _configuration;

	public ExchangeCurrencyServiceTests()
	{
		string appSettingsPath = Path.Combine(
			Directory.GetCurrentDirectory(),
			"..", "..", "..", "..",
			"NewRevolutionaryBank.Web",
			"appsettings.json");

		string appSettingsJson = File.ReadAllText(appSettingsPath);

		JObject appSettings = JObject.Parse(appSettingsJson);

		string api_url = appSettings
			.Property("ExchangeCurrencyApi")!.First!.First!.First!
			.ToString();

		Dictionary<string, string> keyValuePairs = new()
		{
			{ "ExchangeCurrencyApi:URL", api_url }
		};

		_configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(keyValuePairs!)
			.Build();
	}

	[Fact]
	public async Task GetRates_ValidResponse_ReturnsConversionRate()
	{
		// Arrange
		ExchangeCurrencyService exchangeCurrencyService = new(_configuration);

		// Act
		ConversionRate result = await exchangeCurrencyService.GetRates();
		Type type = result.GetType();
		PropertyInfo[] currencies = type.GetProperties();

		// Assert
		Assert.NotNull(result);

		foreach (PropertyInfo currency in currencies)
		{
			Assert.NotNull(currency.GetValue(result));
		}
	}

	[Fact]
	public async Task GetRates_ThrowsException_WhenApiUrlIsInvalid()
	{
		// Arrange
		IConfiguration config = new ConfigurationBuilder().Build();

		ExchangeCurrencyService exchangeCurrencyService = new(config);

		// Act & Assert
		await Assert.ThrowsAsync<InvalidOperationException>(exchangeCurrencyService.GetRates);
	}
}
