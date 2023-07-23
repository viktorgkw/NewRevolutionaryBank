namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class PersonalDataModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public PersonalDataModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public async Task<IActionResult> OnGet()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		return Page();
	}
}
