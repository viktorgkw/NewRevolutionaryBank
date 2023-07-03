namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class Disable2FaModel : PageModel
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
