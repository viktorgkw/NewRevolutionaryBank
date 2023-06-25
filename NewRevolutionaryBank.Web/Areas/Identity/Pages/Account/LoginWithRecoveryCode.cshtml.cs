namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginWithRecoveryCodeModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
