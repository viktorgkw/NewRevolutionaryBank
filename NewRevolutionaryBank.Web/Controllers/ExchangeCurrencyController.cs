namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services.Contracts;

[Authorize]
public class ExchangeCurrencyController : Controller
{
	private const string CurrencyConversionRates = "CCR";

	private readonly IExchangeCurrencyService _currencyService;
	private readonly IMemoryCache _memoryCache;

	public ExchangeCurrencyController(
		IExchangeCurrencyService currencyService,
		IMemoryCache memoryCache)
	{
		_currencyService = currencyService;
		_memoryCache = memoryCache;
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
			}

			return View(rates);
		}
		catch (InvalidOperationException)
		{
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
			}

			return View(rates);
		}
		catch (InvalidOperationException)
		{
			return View(null);
		}
	}
}
