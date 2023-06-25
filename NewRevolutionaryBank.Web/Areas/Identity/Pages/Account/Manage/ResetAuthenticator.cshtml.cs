namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ResetAuthenticatorModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
