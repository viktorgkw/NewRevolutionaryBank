namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

[Authorize]
public partial class BankAccountController : Controller
{
	private readonly IBankAccountService _bankAccountService;

	public BankAccountController(IBankAccountService bankAccountService) =>
		_bankAccountService = bankAccountService;

	[HttpGet]
	[Authorize(Roles = "Guest,AccountHolder")]
	public IActionResult Create() => View();

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
			await _bankAccountService.CreateAsync(User.Identity!.Name!, model);

			return RedirectToAction("MyAccounts", "BankAccount");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> MyAccounts()
	{
		try
		{
			List<BankAccountDisplayViewModel> accounts = await _bankAccountService
				.GetAllUserAccountsAsync(User.Identity!.Name!);

			return View(accounts);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
		catch (InvalidOperationException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> Details(Guid id)
	{
		try
		{
			BankAccountDetailsViewModel? viewModel = await _bankAccountService
				.GetDetailsByIdAsync(id, User.Identity!.Name!);

			return View(viewModel);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> Close(Guid id)
	{
		bool isOwner = await _bankAccountService.IsOwner(id, User.Identity!.Name!);

		if (!isOwner)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}

		return View(id);
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> CloseConfirmation(Guid id)
	{
		try
		{
			await _bankAccountService.CloseAccountByIdAsync(id);

			return RedirectToAction("Index", "Home");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> Deposit()
	{
		try
		{
			DepositViewModel model = await _bankAccountService
				.PrepareDepositViewModel(User.Identity!.Name!);

			return View(model);
		}
		catch (Exception)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}

	[HttpPost]
	[Authorize(Roles = "AccountHolder")]
	public async Task<IActionResult> Deposit(DepositViewModel model)
	{
		try
		{
			ModelState.Remove("MyAccounts");
			ModelState.Remove("StripePayment.Id");
			ModelState.Remove("StripePayment.Currency");
			ModelState.Remove("StripePayment.Description");

			if (!ModelState.IsValid)
			{
				DepositViewModel newModel = await _bankAccountService
					.PrepareDepositViewModel(User.Identity!.Name!);

				return View(newModel);
			}

			await _bankAccountService.DepositAsync(model);

			return RedirectToAction("MyAccounts", "BankAccount");
		}
		catch (Exception)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}
}