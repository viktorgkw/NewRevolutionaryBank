namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services.Contracts;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[Authorize]
public class ExchangeCurrencyController : Controller
{
	private const string CurrencyConversionRates = "CCR";

	private readonly IExchangeCurrencyService _currencyService;
	private readonly ILogger<ExchangeCurrencyController> _logger;
	private readonly IMemoryCache _memoryCache;

	public ExchangeCurrencyController(
		IExchangeCurrencyService currencyService,
		ILogger<ExchangeCurrencyController> logger,
		IMemoryCache memoryCache)
	{
		_currencyService = currencyService;
		_memoryCache = memoryCache;
		_logger = logger;
	}

	[AllowAnonymous]
	public async Task<IActionResult> All()
	{
		try
		{
			if (!_memoryCache.TryGetValue(CurrencyConversionRates, out ConversionRate? rates))
			{
				rates = await _currencyService.GetRates();

				MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

				_memoryCache.Set(CurrencyConversionRates, rates, cacheOptions);

				_logger.LogInformation(string.Format(
					InformationConstants.CurrencyRatesCached,
					DateTime.UtcNow));
			}

			return View(rates);
		}
		catch (InvalidOperationException)
		{
			_logger.LogCritical(string.Format(
					CriticalConstants.CurrencyRatesDontFetch,
					DateTime.UtcNow));

			return View(null);
		}
	}

	[AllowAnonymous]
	public async Task<IActionResult> CurrencyCalculator()
	{
		try
		{
			if (!_memoryCache.TryGetValue(CurrencyConversionRates, out ConversionRate? rates))
			{
				rates = await _currencyService.GetRates();

				MemoryCacheEntryOptions cacheOptions = new MemoryCacheEntryOptions()
					.SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

				_memoryCache.Set(CurrencyConversionRates, rates, cacheOptions);

				_logger.LogInformation(string.Format(
					InformationConstants.CurrencyRatesCached,
					DateTime.UtcNow));
			}

			return View(rates);
		}
		catch (InvalidOperationException)
		{
			_logger.LogCritical(string.Format(
					CriticalConstants.CurrencyRatesDontFetch,
					DateTime.UtcNow));

			return View(null);
		}
	}
}
