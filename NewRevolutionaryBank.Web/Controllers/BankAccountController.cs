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

		return RedirectToAction("MyAccounts", "BankAccount");
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> MyAccounts()
	{
		List<BankAccountDisplayViewModel> accounts = await _bankAccountService
			.GetAllUserAccounts(User.Identity!.Name!);

		if (accounts.Count < 1)
		{
			await _bankAccountService.RemoveHolderRole(User.Identity!.Name!);

			return RedirectToAction("Create");
		}

		return View(accounts);
	}

	[HttpGet]
	public async Task<IActionResult> Details(Guid id)
	{
		BankAccountDetailsViewModel? viewModel = await _bankAccountService
			.GetDetailsByIdAsync(id);

		if (viewModel is null)
		{
			// TODO: Add custom error page
			return RedirectToAction("Index", "Home");
		}

		return View(viewModel);
	}
}
