namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

public class DeletePersonalDataModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	private readonly IAdministratorService _administratorService;

	public DeletePersonalDataModel(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager,
		IAdministratorService administratorService)
	{
		_userManager = userManager;
		_signInManager = signInManager;
		_administratorService = administratorService;
	}

	[BindProperty]
	public InputModel Input { get; set; } = null!;

	public class InputModel
	{
		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
	}

	public bool RequirePassword { get; set; }

	public async Task<IActionResult> OnGet()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		RequirePassword = await _userManager.HasPasswordAsync(user);

		return Page();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		RequirePassword = await _userManager.HasPasswordAsync(user);

		if (RequirePassword && !await _userManager.CheckPasswordAsync(user, Input.Password))
		{
			ModelState.AddModelError(string.Empty, "Incorrect password!");
			return Page();
		}

		await _administratorService.DeactivateUserProfileByIdAsync(user.Id);

		await _signInManager.SignOutAsync();

		return RedirectToAction("Index", "Home");
	}
}
