namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models.ExchangeRates;
using NewRevolutionaryBank.Services.Contracts;

[Authorize]
public class ExchangeCurrencyController : Controller
{
	private readonly IExchangeCurrencyService _currencyService;

	public ExchangeCurrencyController(IExchangeCurrencyService currencyService)
		=> _currencyService = currencyService;

	[AllowAnonymous]
	public async Task<IActionResult> All()
	{
		try
		{
			ConversionRate rates = await _currencyService.GetRates();

			return View(rates);
		}
		catch (InvalidOperationException)
		{
			return View(null);
		}
	}
}
