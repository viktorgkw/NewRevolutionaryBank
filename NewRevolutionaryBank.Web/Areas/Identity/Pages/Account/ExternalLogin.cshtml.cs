namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

[AllowAnonymous]
public class ExternalLoginModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
