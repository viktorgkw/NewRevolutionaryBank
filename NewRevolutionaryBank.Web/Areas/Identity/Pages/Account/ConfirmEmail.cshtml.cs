namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;

public class ConfirmEmailModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public ConfirmEmailModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[TempData]
	public string? StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(string userId, string code)
	{
		if (userId is null || code is null)
		{
			return RedirectToPage("/Index");
		}

		ApplicationUser? user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		IdentityResult result = await _userManager.ConfirmEmailAsync(user, code);

		StatusMessage = result.Succeeded
			? "Thank you for confirming your email!"
			: "Error: Confirming your email was not successful!";

		return Page();
	}
}
