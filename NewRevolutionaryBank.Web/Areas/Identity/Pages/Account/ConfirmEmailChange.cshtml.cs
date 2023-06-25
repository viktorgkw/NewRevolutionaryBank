namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;

public class ConfirmEmailChangeModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public ConfirmEmailChangeModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[TempData]
	public string? StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync(string userId, string email, string code)
	{
		if (userId is null || email is null || code is null)
		{
			return RedirectToPage("/Index");
		}

		ApplicationUser? user = await _userManager.FindByIdAsync(userId);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{userId}'.");
		}

		code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
		IdentityResult result = await _userManager.ChangeEmailAsync(user, email, code);

		if (!result.Succeeded)
		{
			StatusMessage = "Error: Changing email was not successful!";

			return Page();
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Thank you for confirming your email change!";

		return Page();
	}
}
