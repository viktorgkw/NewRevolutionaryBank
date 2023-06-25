namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class LoginModel : PageModel
{
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly UserManager<ApplicationUser> _userManager;

	public LoginModel(
		SignInManager<ApplicationUser> signInManager,
		UserManager<ApplicationUser> userManager)
	{
		_signInManager = signInManager;
		_userManager = userManager;
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

		await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (ModelState.IsValid)
		{
			ApplicationUser? user = await _userManager.FindByNameAsync(Input.UserName);

			if (user is null)
			{
				ModelState.AddModelError(string.Empty, "Invalid login attempt.");

				return Page();
			}

			if (user.IsDeleted)
			{
				ModelState.AddModelError(string.Empty, "The account you are trying to log into is deleted!");

				return Page();
			}

			Microsoft.AspNetCore.Identity.SignInResult result =
				await _signInManager.PasswordSignInAsync(
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

		ModelState.AddModelError(string.Empty, "Unexpected error occured!");

		return Page();
	}
}
