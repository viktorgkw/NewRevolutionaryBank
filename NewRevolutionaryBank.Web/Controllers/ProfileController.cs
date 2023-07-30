namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Profile;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[Authorize]
public class ProfileController : Controller
{
	private readonly IProfileService _profileService;
	private readonly ILogger<ProfileController> _logger;

	public ProfileController(
		IProfileService profileService,
		ILogger<ProfileController> logger)
	{
		_profileService = profileService;
		_logger = logger;
	}

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
			_logger.LogError(string.Format(
				ErrorConstants.ProfilePageError,
				User.Identity?.Name,
				DateTime.UtcNow));

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
			_logger.LogCritical(string.Format(
				CriticalConstants.CriticalErrorInMethod,
				nameof(MyProfile),
				nameof(ProfileController),
				DateTime.UtcNow));

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
