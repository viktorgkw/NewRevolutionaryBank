namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels;

[Authorize]
public class BankAccountController : Controller
{
	private readonly IBankAccountService _bankAccountService;

	public BankAccountController(
		IBankAccountService bankAccountService)
	{
		_bankAccountService = bankAccountService;
	}

	public IActionResult Index()
	{
		// TODO: with account tiers
		return View();
	}

	[HttpGet]
	[Authorize(Roles = "Guest,AccountHolder")]
	public IActionResult Create()
	{
		return View();
	}

	[HttpPost]
	[Authorize(Roles = "Guest,AccountHolder")]
	public async Task<IActionResult> Create(BankAccountCreateViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		await _bankAccountService.Create(User.Identity!.Name!, model);

		return RedirectToAction("MyAccount", "BankAccount");
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public IActionResult MyAccount()
	{
		return View();
	}
}
