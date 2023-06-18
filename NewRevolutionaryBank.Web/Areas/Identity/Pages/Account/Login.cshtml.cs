namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Models;

public class LoginModel : PageModel
{
	private readonly SignInManager<ApplicationUser> _signInManager;

	public LoginModel(SignInManager<ApplicationUser> signInManager)
	{
		_signInManager = signInManager;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	[TempData]
	public string? ErrorMessage { get; set; }

	public class InputModel
	{
		[Required]
		public string UserName { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;

		[Display(Name = "Remember me?")]
		public bool RememberMe { get; set; }
	}

	public async Task<IActionResult> OnGetAsync()
	{
		if (User.Identity?.IsAuthenticated ?? false)
		{
			return RedirectToAction("Index", "Home");
		}

		if (!string.IsNullOrEmpty(ErrorMessage))
		{
			ModelState.AddModelError(string.Empty, ErrorMessage);
		}

		// Clear the existing external cookie to ensure a clean login process
		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (ModelState.IsValid)
		{
			var result = await _signInManager.PasswordSignInAsync(
				Input.UserName,
				Input.Password,
				Input.RememberMe,
				lockoutOnFailure: true);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
			}
			else if (result.IsLockedOut)
			{
				return RedirectToPage("./Lockout");
			}
			else
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");

				return Page();
			}
		}

		return Page();
	}
}
