namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Messaging.Contracts;

public class ForgotPasswordModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailSender _emailSender;

	public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
	{
		_userManager = userManager;
		_emailSender = emailSender;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (ModelState.IsValid)
		{
			ApplicationUser? user = await _userManager
				.FindByEmailAsync(Input.Email);

			if (user is null || !(await _userManager.IsEmailConfirmedAsync(user)))
			{
				return RedirectToPage("./ForgotPasswordConfirmation");
			}

			string code = await _userManager.GeneratePasswordResetTokenAsync(user);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			string callbackUrl = Url.Page(
				"/Account/ResetPassword",
				pageHandler: null,
				values: new { area = "Identity", code },
				protocol: Request.Scheme)!;

			await _emailSender.SendEmailAsync(
				Input.Email,
				"Reset Password",
				$"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			return RedirectToPage("./ForgotPasswordConfirmation");
		}

		return Page();
	}
}
