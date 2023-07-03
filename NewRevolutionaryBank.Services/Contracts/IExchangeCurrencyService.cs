namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Data.Models.ExchangeRates;

public interface IExchangeCurrencyService
{
	Task<ConversionRate> GetRates();
}
