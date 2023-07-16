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
	public async Task<IActionResult> Create()
	{
		await _bankAccountService.CheckUserRole(User);

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
			await _bankAccountService.CheckUserRole(User);

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
			await _bankAccountService.CheckUserRole(User);

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
		await _bankAccountService.CheckUserRole(User);

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
			await _bankAccountService.CheckUserRole(User);

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
			await _bankAccountService.CheckUserRole(User);

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
			await _bankAccountService.CheckUserRole(User);

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
		catch (ArgumentNullException)
		{
			DepositViewModel newModel = await _bankAccountService
					.PrepareDepositViewModel(User.Identity!.Name!);

			return View(newModel);
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