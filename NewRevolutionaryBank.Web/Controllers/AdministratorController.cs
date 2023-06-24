namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.ViewModels.BankAccount;

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
		List<BankAccountDisplayViewModel> accounts = await _administratorService
			.GetAllBankAccounts();

		return View(accounts);
	}

	public async Task<IActionResult> BankAccountDetails(Guid id)
	{
		BankAccountDetailsViewModel account = await _administratorService
			.GetBankAccountDetails(id);

		return View(account);
	}

	[HttpGet]
	public async Task<IActionResult> ActivateBankAccount(string id)
	{
		await _administratorService.ActivateBankAccountByIdAsync(id);

        return RedirectToAction("ManageBankAccounts", "Administrator");
	}
}
