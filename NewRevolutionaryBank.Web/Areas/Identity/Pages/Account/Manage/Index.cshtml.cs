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

	public async Task<IActionResult> OnGetAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		await LoadAsync(user);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		if (!ModelState.IsValid)
		{
			await LoadAsync(user);

			return Page();
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
}
