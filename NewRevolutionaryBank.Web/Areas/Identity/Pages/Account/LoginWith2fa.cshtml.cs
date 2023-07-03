namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class LoginWith2FaModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
