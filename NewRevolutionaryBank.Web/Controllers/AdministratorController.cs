namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.BankAccount;
using NewRevolutionaryBank.Web.ViewModels.Administrator;

[Authorize(Roles = "Administrator")]
public class AdministratorController : Controller
{
	private readonly IAdministratorService _administratorService;

	public AdministratorController(IAdministratorService administratorService)
	{
		_administratorService = administratorService;
	}

	[HttpGet]
	public async Task<IActionResult> ManageBankAccounts()
	{
		// TODO: Filters
		List<BankAccountManageViewModel> accounts = await _administratorService
			.GetAllBankAccounts();

		return View(accounts);
	}

	public async Task<IActionResult> BankAccountDetails(Guid id)
	{
		try
		{
			BankAccountDetailsViewModel account = await _administratorService
				.GetBankAccountDetails(id);

			return View(account);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction("ManageBankAccounts", "Administrator");
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
			return RedirectToAction("ManageBankAccounts", "Administrator");
		}
	}

	[HttpGet]
	public async Task<IActionResult> ManageUserProfiles()
	{
		// TODO: Filters
		List<UserProfileManageViewModel> users = await _administratorService
			.GetAllProfilesAsync();

		return View(users);
	}
}
