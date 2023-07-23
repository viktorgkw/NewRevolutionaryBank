namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Messaging.Contracts;

public class EmailModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IEmailSender _emailSender;

	public EmailModel(
		UserManager<ApplicationUser> userManager,
		IEmailSender emailSender,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_emailSender = emailSender;
		_signInManager = signInManager;
	}

	public string Email { get; set; } = null!;

	public bool IsEmailConfirmed { get; set; }

	[TempData]
	public string? StatusMessage { get; set; }

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Required]
		[EmailAddress]
		[Display(Name = "New email")]
		public string NewEmail { get; set; } = null!;
	}

	public async Task<IActionResult> OnGetAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		await LoadAsync(user);

		return Page();
	}

	public async Task<IActionResult> OnPostChangeEmailAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		if (!ModelState.IsValid)
		{
			await LoadAsync(user);

			return Page();
		}

		bool emailUsed = await IsEmailUsed(Input.NewEmail);

		if (emailUsed)
		{
			StatusMessage = "Error: This email is already in use!";
			return RedirectToPage();
		}

		string email = (await _userManager.GetEmailAsync(user))!;

		if (Input.NewEmail != email)
		{
			string userId = (await _userManager.GetUserIdAsync(user))!;
			string code = await _userManager.GenerateChangeEmailTokenAsync(user, Input.NewEmail);
			code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

			string callbackUrl = Url.Page(
				"/Account/ConfirmEmailChange",
				pageHandler: null,
				values: new { area = "Identity", userId, email = Input.NewEmail, code },
				protocol: Request.Scheme)!;

			await _emailSender.SendEmailAsync(
				Input.NewEmail,
				"NRB - Confirm your email",
				$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

			StatusMessage = "Confirmation link sent. Please check your email.";
			return RedirectToPage();
		}

		StatusMessage = "Error: Your new email must be different from your current one!";
		return RedirectToPage();
	}

	public async Task<IActionResult> OnPostSendVerificationEmailAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		if (!ModelState.IsValid)
		{
			await LoadAsync(user);

			return Page();
		}

		bool emailUsed = await IsEmailUsed(Input.NewEmail);

		if (emailUsed)
		{
			StatusMessage = "Error: This email is already in use!";
			return RedirectToPage();
		}

		string userId = await _userManager.GetUserIdAsync(user);
		string email = (await _userManager.GetEmailAsync(user))!;
		string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
		code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

		string callbackUrl = Url.Page(
			"/Account/ConfirmEmail",
			pageHandler: null,
			values: new { area = "Identity", userId, code },
			protocol: Request.Scheme)!;

		await _emailSender.SendEmailAsync(
			email,
			"NRB - Confirm your email",
			$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

		StatusMessage = "Verification email sent. Please check your email.";
		return RedirectToPage();
	}

	private async Task<bool> IsEmailUsed(string newEmail) =>
		await _userManager.FindByEmailAsync(newEmail) is not null;

	private async Task LoadAsync(ApplicationUser user)
	{
		Email = (await _userManager.GetEmailAsync(user))!;

		Input = new InputModel
		{
			NewEmail = "",
		};

		IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
	}
}
