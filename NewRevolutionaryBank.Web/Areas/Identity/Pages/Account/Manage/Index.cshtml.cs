namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class IndexModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public IndexModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}

	public string Username { get; set; } = null!;

	[TempData]
	public string? StatusMessage { get; set; }

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Phone]
		[Display(Name = "Phone number")]
		public string? PhoneNumber { get; set; }
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

	public async Task<IActionResult> OnPostAsync()
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

		if (string.IsNullOrEmpty(Input.PhoneNumber) ||
			string.IsNullOrWhiteSpace(Input.PhoneNumber))
		{
			StatusMessage = "Error: The phone number cannot be white space!";

			return RedirectToPage();
		}

		string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);

		if (Input.PhoneNumber != phoneNumber)
		{
			IdentityResult setPhoneResult = await _userManager
				.SetPhoneNumberAsync(user, Input.PhoneNumber);

			if (!setPhoneResult.Succeeded)
			{
				StatusMessage = "Unexpected error when trying to set phone number.";

				return RedirectToPage();
			}
		}

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your profile has been updated";

		return RedirectToPage();
	}

	private async Task LoadAsync(ApplicationUser user)
	{
		string userName = (await _userManager.GetUserNameAsync(user))!;
		string? phoneNumber = await _userManager.GetPhoneNumberAsync(user);

		Username = userName;

		Input = new InputModel
		{
			PhoneNumber = phoneNumber
		};
	}
}
