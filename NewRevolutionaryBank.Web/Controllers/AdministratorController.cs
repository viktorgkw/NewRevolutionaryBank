namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;

[Authorize(Roles = "Administrator")]
public class AdministratorController : Controller
{
	private readonly IAdministratorService _adminService;

	public AdministratorController(IAdministratorService adminService)
	{
		_adminService = adminService;
	}

	public IActionResult ManageBankAccounts()
	{
		return View();
	}

	public IActionResult ManageProfiles()
	{
		return View();
	}
}
