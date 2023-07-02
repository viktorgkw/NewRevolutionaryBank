namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;

[Authorize]
public class RatingController : Controller
{
	private readonly IRatingService _ratingService;
	private readonly UserManager<ApplicationUser> _userManager;

	public RatingController(
		IRatingService ratingService,
		UserManager<ApplicationUser> userManager)
	{
		_ratingService = ratingService;
		_userManager = userManager;
	}

	[HttpGet]
	public async Task<IActionResult> RateUs()
	{
		bool hasRated = await _ratingService.HasAlreadyRated(User.Identity!.Name);

		if (hasRated)
		{
			return RedirectToAction("Index", "Home");
		}

		return View();
	}

	[HttpPost]
	public async Task<IActionResult> RateUs(Rating model)
	{
		ApplicationUser? currUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

		if (currUser is null)
		{
			// TODO: Error page
			return RedirectToAction("Index", "Home");
		}

		model.RatedBy = currUser;

		await _ratingService.SendRating(model);

		return RedirectToAction("Index", "Home");
	}
}
