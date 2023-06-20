namespace NewRevolutionaryBank.Web.Controllers;

using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels.BankAccount;
using NewRevolutionaryBank.ViewModels.Transaction;

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
	public async Task<IActionResult> Create()
	{
		await _bankAccountService.CheckRoleAsync(User.Identity!.Name!);

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
		bool isHolder = await _bankAccountService.CheckRoleAsync(User.Identity!.Name!);

		if (!isHolder)
		{
			return RedirectToAction("Create", "BankAccount");
		}

		List<BankAccountDisplayViewModel> accounts = await _bankAccountService
			.GetAllUserAccounts(User.Identity!.Name!);

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

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> NewTransaction()
	{
		TransactionNewViewModel model = await _bankAccountService
			.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

		return View(model);
	}

	[HttpPost]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> NewTransaction(IFormCollection form, TransactionNewViewModel model)
	{
		if (!ModelState.IsValid)
		{
			TransactionNewViewModel cleanModel = await _bankAccountService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			return View(cleanModel);
		}

		PaymentResult paymentResult = await _bankAccountService
			.BeginPaymentAsync(model.AccountFrom, model.AccountTo, model.Amount);

		if (paymentResult != PaymentResult.Successful)
		{
			ModelState.AddModelError("", string.Join(" ", Regex.Split(
				paymentResult.ToString(),
				@"(?<!^)(?=[A-Z])")));

			TransactionNewViewModel cleanModel = await _bankAccountService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			return View(cleanModel);
		}

		TempData["RedirectedFromMethod"] = "NewTransaction";
		return RedirectToAction("TransactionSuccessful", "BankAccount");
	}

	[HttpGet]
	public IActionResult TransactionSuccessful()
	{
		if (TempData["RedirectedFromMethod"] is not null &&
			TempData["RedirectedFromMethod"]!.ToString() == "NewTransaction")
		{
			return View();
		}

		return RedirectToAction("Index", "Home");
	}
}
