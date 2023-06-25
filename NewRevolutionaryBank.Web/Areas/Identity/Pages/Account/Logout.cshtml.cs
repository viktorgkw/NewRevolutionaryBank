namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class LogoutModel : PageModel
{
	private readonly SignInManager<ApplicationUser> _signInManager;

	public LogoutModel(SignInManager<ApplicationUser> signInManager) =>
		_signInManager = signInManager;

	public async Task<IActionResult> OnGet()
	{
		if (User.Identity?.IsAuthenticated ?? false)
		{
			await _signInManager.SignOutAsync();
		}

		return RedirectToPage("Index", "Home");
	}
}
