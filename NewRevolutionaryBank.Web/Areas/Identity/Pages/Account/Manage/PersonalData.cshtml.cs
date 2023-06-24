namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class PersonalDataModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;

	public PersonalDataModel(
		UserManager<ApplicationUser> userManager)
	{
		_userManager = userManager;
	}

	public async Task<IActionResult> OnGet()
	{
		var user = await _userManager.GetUserAsync(User);
		if (user == null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		return Page();
	}
}
