namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;

public class AvatarModel : PageModel
{
	private readonly NrbDbContext _context;
	private readonly SignInManager<ApplicationUser> _signInManager;

	public AvatarModel(
		NrbDbContext context,
		SignInManager<ApplicationUser> signInManager)
	{
		_context = context;
		_signInManager = signInManager;
	}

	[TempData]
	public string? StatusMessage { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		ApplicationUser? user = await _context.Users
			.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

		if (user is null)
		{
			StatusMessage = "Error: Could not load user!";

			return RedirectToPage();
		}

		// TODO: load current avatar if any

		return Page();
	}

	public async Task<IActionResult> OnPostAsync(IFormFile avatar)
	{
		ApplicationUser? user = await _context.Users
			.FirstOrDefaultAsync(u => u.UserName == User.Identity!.Name);

		if (user is null)
		{
			StatusMessage = "Error: Could not load user!";

			return RedirectToPage();
		}

		if (avatar is null)
		{
			StatusMessage = "Error: Avatar file is required!";

			return RedirectToPage();
		}

		if (!HasValidExtension(avatar.FileName))
		{
			StatusMessage = "Error: The provided file extension is not supported!";

			return RedirectToPage();
		}

		using MemoryStream ms = new();
		avatar.CopyTo(ms);

		user.Avatar = ms.ToArray();
		await _context.SaveChangesAsync();

		await _signInManager.RefreshSignInAsync(user);
		StatusMessage = "Your avatar has been updated!";

		return RedirectToPage();
	}

	private static bool HasValidExtension(string fileName) =>
		fileName.ToLower().EndsWith(".png") ||
		fileName.ToLower().EndsWith(".jpg") ||
		fileName.ToLower().EndsWith(".jpeg");
}
