namespace NewRevolutionaryBank.Web.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Home;

using static NewRevolutionaryBank.Common.LoggingMessageConstants;

[AllowAnonymous]
public class HomeController : Controller
{
	private readonly IEmailSender _emailSender;
	private readonly IRatingService _ratingService;
	private readonly ILogger<HomeController> _logger;

	public HomeController(
		IEmailSender emailSender,
		IRatingService ratingService,
		ILogger<HomeController> logger)
	{
		_emailSender = emailSender;
		_ratingService = ratingService;
		_logger = logger;
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
			_logger.LogInformation(string.Format(
				InformationConstants.SupportMessageSent,
				User.Identity?.Name));

			description += $"<br />From {emailFrom}";

			await _emailSender.SendEmailAsync(
				"topautospotbulgaria@gmail.com",
				subject,
				description);
		}

		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error(string title, string description, bool isNotFound)
	{
		_logger.LogError(string.Format(
				ErrorConstants.UserHitAnError,
				User.Identity?.Name,
				description));

		return View(new ErrorViewModel
		{
			Title = title,
			Description = description,
			IsNotFound = isNotFound
		});
	}
}