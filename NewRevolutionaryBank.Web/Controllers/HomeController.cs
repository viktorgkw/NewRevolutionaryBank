namespace NewRevolutionaryBank.Web.Controllers;

using System.Diagnostics;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using NewRevolutionaryBank.Web.ViewModels.Home;

[AllowAnonymous]
public class HomeController : Controller
{
	public IActionResult Index() => View();

	public IActionResult Privacy() => View();

	public IActionResult Contacts() => View();

	public IActionResult FindUs() => View();

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error() =>
		View(new ErrorViewModel
		{
			RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
		});
}