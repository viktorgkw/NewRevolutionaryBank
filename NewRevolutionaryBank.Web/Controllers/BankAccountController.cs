namespace NewRevolutionaryBank.Web.Controllers;

using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models.Enums;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Transaction;

[Authorize]
public partial class BankAccountController : Controller
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
		try
		{
			await _bankAccountService.CheckRoleAsync(User.Identity!.Name!);

			return View();
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpPost]
	[Authorize(Roles = "Guest,AccountHolder")]
	public async Task<IActionResult> Create(BankAccountCreateViewModel model)
	{
		if (!ModelState.IsValid)
		{
			return View(model);
		}

		try
		{
			await _bankAccountService.Create(User.Identity!.Name!, model);

			return RedirectToAction("MyAccounts", "BankAccount");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> MyAccounts()
	{
		try
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
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpGet]
	public async Task<IActionResult> Details(Guid id)
	{
		try
		{
			BankAccountDetailsViewModel? viewModel = await _bankAccountService
				.GetDetailsByIdAsync(id);

			// TODO: If User is owner
			return View(viewModel);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("MyAccounts", "BankAccount");
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> NewTransaction()
	{
		try
		{
			TransactionNewViewModel model = await _bankAccountService
			.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			return View(model);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpPost]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> NewTransaction(TransactionNewViewModel model)
	{
		try
		{
			TransactionNewViewModel cleanModel = await _bankAccountService
				.PrepareTransactionModelForUserAsync(User.Identity!.Name!);

			if (!ModelState.IsValid)
			{
				return View(cleanModel);
			}

			PaymentResult paymentResult = await _bankAccountService
				.BeginPaymentAsync(model.AccountFrom, model.AccountTo, model.Amount);

			if (paymentResult != PaymentResult.Successful)
			{
				ModelState.AddModelError("",
					string.Join(" ", Regex.Split(paymentResult.ToString(), "(?<!^)(?=[A-Z])")));

				return View(cleanModel);
			}

			TempData["RedirectedFromMethod"] = "NewTransaction";
			return RedirectToAction("TransactionSuccessful", "BankAccount");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("Index", "Home");
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public IActionResult TransactionSuccessful()
	{
		if (TempData["RedirectedFromMethod"] is not null &&
			TempData["RedirectedFromMethod"]!.ToString() == "NewTransaction")
		{
			return View();
		}

		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	public IActionResult Close(Guid id)
	{
		// TODO: Validate is owner

		return View(id);
	}

	[HttpGet]
	public async Task<IActionResult> CloseConfirmation(Guid id)
	{
		// TODO: Validate is owner and if it came from Close

		await _bankAccountService.CloseAccountByIdAsync(id);

		return RedirectToAction("Index", "Home");
	}
}