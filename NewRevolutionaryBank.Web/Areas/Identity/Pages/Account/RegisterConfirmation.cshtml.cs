namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

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
public class RegisterConfirmationModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IEmailSender _emailSender;

	public RegisterConfirmationModel(
		UserManager<ApplicationUser> userManager,
		IEmailSender emailSender)
	{
		_userManager = userManager;
		_emailSender = emailSender;
	}

	public string Email { get; set; } = null!;

	public bool DisplayConfirmAccountLink { get; set; }

	public string EmailConfirmationUrl { get; set; } = null!;

	public async Task<IActionResult> OnGetAsync(string email)
	{
		if (email is null)
		{
			return RedirectToPage("/Index");
		}

		ApplicationUser? user = await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			return NotFound($"Unable to load user with email '{email}'.");
		}

		string userId = await _userManager.GetUserIdAsync(user);
		string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		string callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			pageHandler: null,
			values: new { area = "Identity", userId, code },
			protocol: Request.Scheme)!;

		await _emailSender.SendEmailAsync(
					email,
					"NRB - Register confirmation",
					$"Please confirm your registration by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

		return Page();
	}
}
