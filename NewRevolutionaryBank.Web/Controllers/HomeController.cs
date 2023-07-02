namespace NewRevolutionaryBank.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;

[AllowAnonymous]
public class HomeController : Controller
{
	private readonly IEmailSender _emailSender;
	private readonly IRatingService _ratingService;

	public HomeController(
		IEmailSender emailSender,
		IRatingService ratingService)
	{
		_emailSender = emailSender;
		_ratingService = ratingService;
	}

	[HttpGet]
	public async Task<IActionResult> Index()
	{
		List<Rating> ratings = await _ratingService.GetAll();

		return View(ratings);
	}

	[HttpGet]
	public IActionResult Privacy() => View();

	[HttpGet]
	public IActionResult Contacts() => View();

	[HttpGet]
	public IActionResult FindUs() => View();

	[HttpGet]
	public IActionResult Support() => View();

	[HttpPost]
	public async Task<IActionResult> Support(
		string emailFrom,
		string subject,
		string description)
	{
		if (!string.IsNullOrWhiteSpace(emailFrom) &&
			!string.IsNullOrWhiteSpace(subject) &&
			!string.IsNullOrWhiteSpace(description))
		{
			description += $"<br />From {emailFrom}";

			await _emailSender.SendEmailAsync(
				"topautospotbulgaria@gmail.com",
				subject,
				description);
		}

		return View();
	}

	//[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
}