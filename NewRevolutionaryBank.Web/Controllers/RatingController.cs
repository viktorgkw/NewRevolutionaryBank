namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[Authorize]
public class RatingController : Controller
{
	private readonly IRatingService _ratingService;
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly ILogger<RatingController> _logger;

	public RatingController(
		IRatingService ratingService,
		UserManager<ApplicationUser> userManager,
		ILogger<RatingController> logger)
	{
		_ratingService = ratingService;
		_userManager = userManager;
		_logger = logger;
	}

	[HttpGet]
	public async Task<IActionResult> RateUs()
	{
		bool hasRated = await _ratingService.HasAlreadyRated(User.Identity!.Name);

		if (hasRated)
		{
			_logger.LogInformation(string.Format(
				InformationConstants.UserTriesToRateMoreThanOnce,
				User.Identity?.Name,
				DateTime.UtcNow));

			return RedirectToAction(
				"Error",
				"Home",
				new
				{
					title = "You have already rated!",
					description = "If you have not and there is a problem, please contact our support!",
					isNotFound = false
				}
			);
		}

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> RateUs(Rating model)
	{
		ApplicationUser? currUser = await _userManager
			.FindByNameAsync(User.Identity!.Name!);

		if (currUser is null)
		{
			_logger.LogInformation(string.Format(
				InformationConstants.UnauthorizedUserTriedToRate,
				User.Identity?.Name,
				DateTime.UtcNow));

			return Unauthorized();
		}

		model.RatedBy = currUser;

		await _ratingService.SendRating(model);

		_logger.LogInformation(string.Format(
				InformationConstants.UserRated,
				User.Identity?.Name,
				DateTime.UtcNow));

		return RedirectToAction("Index", "Home");
	}
}
