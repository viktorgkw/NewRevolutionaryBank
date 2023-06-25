namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class TwoFactorAuthenticationModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
