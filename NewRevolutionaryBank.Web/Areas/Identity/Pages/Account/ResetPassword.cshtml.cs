namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Text;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;

public class ResetPasswordModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;

	public ResetPasswordModel(UserManager<ApplicationUser> userManager) =>
		_userManager = userManager;

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = null!;

		[Required]
		public string Code { get; set; } = null!;

	}

	public IActionResult OnGet(string code)
	{
		if (code is null)
		{
			return BadRequest("A code must be supplied for password reset.");
		}
		else
		{
			Input = new InputModel
			{
				Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
			};

			return Page();
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		ApplicationUser? user = await _userManager
			.FindByEmailAsync(Input.Email);

		if (user is null)
		{
			return RedirectToPage("/Account/Login", new { area = "Identity" });
		}

		IdentityResult result = await _userManager
			.ResetPasswordAsync(user, Input.Code, Input.Password);

		if (result.Succeeded)
		{
			return RedirectToPage("/Account/Login", new { area = "Identity" });
		}

		foreach (IdentityError error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}

		return Page();
	}
}
