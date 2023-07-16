namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Profile;

[Authorize]
public class ProfileController : Controller
{
	private readonly IProfileService _profileService;

	public ProfileController(IProfileService profileService) =>
		_profileService = profileService;

	public async Task<IActionResult> MyProfile()
	{
		try
		{
			MyProfileViewModel profile = await _profileService
			.GetProfileDataAsync(User.Identity!.Name!);

			ViewData["Title"] = $"{profile.FirstName}'s profile";

			return View(profile);
		}
		catch (ArgumentNullException)
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "User profile error occurred!",
					description = "There might be a problem with your profile, contact support as soon as possible so we can fix the problem!",
					isNotFound = false
				}
			);
		}
		catch
		{
			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "Unknown error occurred!",
					description = "Contact support the details for help!",
					isNotFound = false
				}
			);
		}
	}
}
