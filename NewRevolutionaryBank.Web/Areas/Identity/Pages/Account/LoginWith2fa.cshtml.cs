namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#pragma warning disable S101 // Types should be named in PascalCase
public class LoginWith2faModel : PageModel
#pragma warning restore S101 // Types should be named in PascalCase
{
    public IActionResult OnGet() => RedirectToAction("Index", "Home");
}
