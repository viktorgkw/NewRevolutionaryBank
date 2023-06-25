namespace NewRevolutionaryBank.Web.Areas.Identity.Pages.Account.Manage;

using System.Reflection;
using System.Text.Json;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using NewRevolutionaryBank.Data.Models;

public class DownloadPersonalDataModel : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;

	public DownloadPersonalDataModel(UserManager<ApplicationUser> userManager) =>
		_userManager = userManager;

	public IActionResult OnGet() => NotFound();

	public async Task<IActionResult> OnPostAsync()
	{
		ApplicationUser? user = await _userManager.GetUserAsync(User);

		if (user is null)
		{
			return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
		}

		Dictionary<string, string> personalData = new();

		IEnumerable<PropertyInfo> personalDataProps = typeof(ApplicationUser)
			.GetProperties()
			.Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

		foreach (PropertyInfo propInfo in personalDataProps)
		{
			string propValue = propInfo.GetValue(user)?.ToString() ?? "null";

			if (propValue is not null)
			{
				personalData.Add(propInfo.Name, propValue);
			}
		}

		Response.Headers
			.Add("Content-Disposition", "attachment; filename=PersonalData.json");

		return new FileContentResult(
			JsonSerializer.SerializeToUtf8Bytes(personalData),
			"application/json");
	}
}
