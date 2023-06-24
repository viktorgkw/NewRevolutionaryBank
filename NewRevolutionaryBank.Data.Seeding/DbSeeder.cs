namespace NewRevolutionaryBank.Data.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NewRevolutionaryBank.Data.Models;

public static class DbSeeder
{
	public static async Task SeedRolesAndAdministratorAsync(
		RoleManager<ApplicationRole> roleManager,
		UserManager<ApplicationUser> userManager,
		IConfiguration configuration)
	{
		// Seed roles
		string[] roles = new[]
		{
			"Administrator",
			"Manager",
			"AccountHolder",
			"Guest",
		};

		foreach (var role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new ApplicationRole(role));
			}
		}

		// Seed administrator account
		string adminUsername = configuration["Seeding:UserName"]!;
		string adminEmail = configuration["Seeding:Email"]!;
		string adminPassword = configuration["Seeding:Password"]!;

		ApplicationUser? adminExists = await userManager.FindByEmailAsync(adminEmail);

		if (adminExists is null)
		{
			var adminUser = new ApplicationUser
			{
				UserName = adminUsername,
				Email = adminEmail,
				EmailConfirmed = true,
				FirstName = adminUsername,
				LastName = adminUsername
			};

			var createdResult = await userManager.CreateAsync(adminUser, adminPassword);

			if (createdResult.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Administrator");
			}
		}
	}
}
