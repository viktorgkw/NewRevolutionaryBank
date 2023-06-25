namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#pragma warning disable S101 // Types should be named in PascalCase
public class Disable2faModel : PageModel
#pragma warning restore S101 // Types should be named in PascalCase
{
	public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
