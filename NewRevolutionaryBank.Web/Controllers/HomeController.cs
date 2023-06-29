namespace NewRevolutionaryBank.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Home;

[AllowAnonymous]
public class HomeController : Controller
{
	private readonly IEmailSender _emailSender;

    public HomeController(IEmailSender emailSender)
    {
		_emailSender = emailSender;
	}

    [HttpGet]
	public IActionResult Index() => View();

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

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() =>
		View(new ErrorViewModel
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
		});
}