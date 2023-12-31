﻿namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class ChangePasswordModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public ChangePasswordModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	[TempData]
	public string? StatusMessage { get; set; }

	public class InputModel
	{
		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Current password")]
		public string OldPassword { get; set; } = null!;

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "New password")]
		public string NewPassword { get; set; } = null!;

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Confirm new password")]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; } = null!;
	}

	public async Task<IActionResult> OnGetAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		bool hasPassword = await _userManager.HasPasswordAsync(user);

		if (!hasPassword)
		{
			return RedirectToPage("./SetPassword");
		}

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (!ModelState.IsValid)
		{
			return Page();
		}

		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		if (Input.OldPassword == Input.NewPassword)
		{
			StatusMessage = "Error: Your new password must be different from your old one!";

			return RedirectToPage();
		}

		IdentityResult changePasswordResult = await _userManager
			.ChangePasswordAsync(user, Input.OldPassword, Input.NewPassword);

		if (!changePasswordResult.Succeeded)
		{
			foreach (IdentityError error in changePasswordResult.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
			}

			return Page();
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your password has been changed successfully.";

		return RedirectToPage();
	}
}
