namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Administrator;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;

[Authorize(Roles = "Administrator")]
public class AdministratorController : Controller
{
	private readonly IAdministratorService _administratorService;

	public AdministratorController(IAdministratorService administratorService) =>
		_administratorService = administratorService;

	[HttpGet]
	public async Task<IActionResult> ManageBankAccounts()
	{
		List<BankAccountManageViewModel> accounts = await _administratorService
			.GetAllBankAccountsAsync();

		return View(accounts);
	}

	public async Task<IActionResult> BankAccountDetails(Guid id)
	{
		try
		{
			BankAccountDetailsViewModel account = await _administratorService
				.GetBankAccountDetailsByIdAsync(id);

			return View(account);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public async Task<IActionResult> ActivateBankAccount(string id)
	{
		try
		{
			await _administratorService.ActivateBankAccountByIdAsync(id);

			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public async Task<IActionResult> DeactivateBankAccount(string id)
	{
		try
		{
			await _administratorService.DeactivateBankAccountByIdAsync(id);

			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public async Task<IActionResult> ManageUserProfiles(
		string order,
		string? searchName)
	{
		order ??= "active";

		List<UserProfileManageViewModel> users = await _administratorService
			.GetAllProfilesAsync(order, searchName);

		return View(users);
	}

	[HttpGet]
	public async Task<IActionResult> UserProfileDetails(Guid id)
	{
		try
		{
			UserProfileDetailsViewModel model = await _administratorService
				.GetUserProfileDetailsByIdAsync(id);

			return View(model);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public async Task<IActionResult> ActivateUserProfile(Guid id)
	{
		try
		{
			await _administratorService.ActivateUserProfileByIdAsync(id);

			return RedirectToAction("ManageUserProfiles", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}

	}

	[HttpGet]
	public async Task<IActionResult> DeactivateUserProfile(Guid id)
	{
		try
		{
			await _administratorService.DeactivateUserProfileByIdAsync(id);

			return RedirectToAction("ManageUserProfiles", "Administrator");
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Invalid Id!",
					description = $"Account with Id {id} does not exist!",
					isNotFound = false
				}
			);
		}
	}

	[HttpGet]
	public async Task<IActionResult> TransactionsList()
	{
		List<TransactionDisplayViewModel> trasactions = await _administratorService
			.GetAllTransactionsAsync();

		return View(trasactions);
	}

	[HttpGet]
	public async Task<IActionResult> ManageBankSettings()
	{
		BankSettingsDisplayViewModel model = await _administratorService
			.GetBankSettingsAsync();

		return View(model);
	}

	[HttpGet]
	public IActionResult EditTransactionFee() => View();

	[HttpPost]
	public async Task<IActionResult> EditTransactionFee(decimal decimalValue)
	{
		await _administratorService.EditTransactionFeeAsync(decimalValue);

		return RedirectToAction("ManageBankSettings", "Administrator");
	}

	[HttpGet]
	public IActionResult EditMonthlyTax() => View();

	[HttpPost]
	public async Task<IActionResult> EditMonthlyTax(decimal decimalValue)
	{
		await _administratorService.EditMonthlyTaxAsync(decimalValue);

		return RedirectToAction("ManageBankSettings", "Administrator");
	}

	[HttpGet]
	public async Task<IActionResult> Statistics()
	{
		WebsiteStatisticsViewModel statistics = await _administratorService
			.GetWebsiteStatisticsAsync();

		return View(statistics);
	}
}
