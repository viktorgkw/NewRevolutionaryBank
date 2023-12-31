﻿namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Griesoft.AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Messaging.Contracts;

[ValidateRecaptcha(
	Action = "Register",
	ValidationFailedAction = ValidationFailedAction.ContinueRequest)]
public class RegisterModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IUserStore<ApplicationUser> _userStore;
	private readonly IUserEmailStore<ApplicationUser> _emailStore;
	private readonly IEmailSender _emailSender;

	public RegisterModel(
		UserManager<ApplicationUser> userManager,
		IUserStore<ApplicationUser> userStore,
		IEmailSender emailSender)
	{
		_userManager = userManager;
		_userStore = userStore;
		_emailStore = GetEmailStore();
		_emailSender = emailSender;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Required]
		[Display(Name = "First Name")]
		public string FirstName { get; set; } = null!;

		[Required]
		[Display(Name = "Last Name")]
		public string LastName { get; set; } = null!;

		[Required]
		[Display(Name = "UserName")]
		public string UserName { get; set; } = null!;

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; } = null!;

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = null!;
	}

	public IActionResult OnGet()
	{
		if (User.Identity?.IsAuthenticated ?? false)
		{
			return RedirectToAction("Index", "Home");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (ModelState.IsValid)
		{
			ApplicationUser user = CreateUser();

			user.FirstName = Input.FirstName;
			user.LastName = Input.LastName;

			await _userStore.SetUserNameAsync(user, Input.UserName, CancellationToken.None);
			await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

			IdentityResult result = await _userManager.CreateAsync(user, Input.Password);

			if (result.Succeeded)
			{
				await _userManager.AddToRoleAsync(user, "Guest");

				string userId = await _userManager.GetUserIdAsync(user);
				string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

				code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
				string callbackUrl = Url.Page(
					"/Account/ConfirmEmail",
					null,
					new { area = "Identity", userId, code },
					Request.Scheme)!;

				await _emailSender.SendEmailAsync(
					Input.Email,
					"NRB - Confirm your email",
					$"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

				return RedirectToPage("RegisterConfirmation");
			}

			foreach (IdentityError error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}
		}

		return Page();
	}

	private ApplicationUser CreateUser()
	{
		try
		{
			return Activator.CreateInstance<ApplicationUser>();
		}
		catch
		{
			throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
				$"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
				$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
		}
	}

	private IUserEmailStore<ApplicationUser> GetEmailStore()
	{
		if (!_userManager.SupportsUserEmail)
		{
			throw new NotSupportedException("The default UI requires a user store with email support.");
		}

		return (IUserEmailStore<ApplicationUser>)_userStore;
	}
}
