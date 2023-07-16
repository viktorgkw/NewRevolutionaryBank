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
		MyProfileViewModel profile = await _profileService
			.GetProfileDataAsync(User.Identity!.Name!);

		ViewData["Title"] = $"{profile.FirstName}'s profile";

		return View(profile);
	}
}
