namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Messaging.Contracts;

[AllowAnonymous]
public class ResendEmailConfirmationModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailSender _emailSender;

	public ResendEmailConfirmationModel(
		UserManager<ApplicationUser> userManager,
		IEmailSender emailSender)
	{
		_userManager = userManager;
		_emailSender = emailSender;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	[TempData]
	public string? StatusMessage { get; set; }

	public class InputModel
	{
		[Required]
		[EmailAddress]
		public string Email { get; set; } = null!;
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		ApplicationUser? user = await _userManager.FindByEmailAsync(Input.Email);

		if (user is null)
		{
			StatusMessage = "Verification email sent. Please check your email.";

			return Page();
		}

		string userId = await _userManager.GetUserIdAsync(user);
		string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		string callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			pageHandler: null,
			values: new { userId, code },
			protocol: Request.Scheme)!;

		await _emailSender.SendEmailAsync(
			Input.Email,
			"NRB - Confirm your email",
			$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

		StatusMessage = "Verification email sent. Please check your email.";

		return Page();
	}
}
