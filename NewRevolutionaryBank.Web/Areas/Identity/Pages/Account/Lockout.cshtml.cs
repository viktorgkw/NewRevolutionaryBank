namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class LockoutModel : PageModel
{
	public IActionResult OnGet()
	{
		if (User.Identity?.IsAuthenticated ?? false)
		{
			return RedirectToAction("Index", "Home");
		}

		return Page();
	}
}
