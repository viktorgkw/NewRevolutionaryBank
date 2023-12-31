﻿namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[Authorize]
public partial class BankAccountController : Controller
{
	private readonly IBankAccountService _bankAccountService;
	private readonly ILogger<BankAccountController> _logger;

	public BankAccountController(
		IBankAccountService bankAccountService,
		ILogger<BankAccountController> logger)
	{
		_bankAccountService = bankAccountService;
		_logger = logger;
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult BankAccountTiers() => View();

	[HttpGet]
	[Authorize(Roles = "Guest,AccountHolder")]
	public async Task<IActionResult> Create()
	{
		await _bankAccountService.CheckUserRole(User);

		BankAccountCreateViewModel viewModel = _bankAccountService.GetCreateViewModel();

		return View(viewModel);
	}

	[HttpPost]
	[Authorize(Roles = "Guest,AccountHolder")]
	public async Task<IActionResult> Create(BankAccountCreateViewModel model)
	{
		ModelState.Remove("Tiers");

		if (!ModelState.IsValid)
		{
			return View(_bankAccountService.GetCreateViewModel());
		}

		try
		{
			await _bankAccountService.CreateAsync(User.Identity!.Name!, model);

			return RedirectToAction("MyAccounts", "BankAccount");
		}
		catch (ArgumentNullException)
		{
			_logger.LogError(string.Format(
				ErrorConstants.UserTriedToCreateBankAccountWithInvalidData,
				User.Identity?.Name,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Problem occurred while creating a bank account!",
					description = "User profile does not exist!",
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
			_logger.LogError(string.Format(
				ErrorConstants.UserTriedToAccessHisBankAccountUnsuccessfully,
				User.Identity?.Name,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Problem occurred while retrieving bank accounts!",
					description = "User profile does not exist!",
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
			_logger.LogWarning(string.Format(
				WarningConstants.UserTriedToAccessUnexistingBankAccDetails,
				User.Identity?.Name,
				id,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Bank Account!",
					description = $"Bank account with id {id} is not found!",
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
			_logger.LogError(string.Format(
				ErrorConstants.NotOwnerTriedToCloseBankAccount,
				User.Identity?.Name,
				id,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid bank account data!",
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
					title = "Invalid Bank Account!",
					description = $"Bank account with id {id} is not found!",
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
		catch (ArgumentNullException)
		{
			_logger.LogError(string.Format(
				ErrorConstants.UnauthorizedUserTriedToDeposit,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Problem occurred while trying to deposit!",
					description = "User profile does not exist!",
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
			_logger.LogCritical(string.Format(
				CriticalConstants.CriticalErrorInMethod,
				nameof(Deposit),
				nameof(BankAccountController),
				DateTime.UtcNow));

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