namespace NewRevolutionaryBank.Data.Seeding;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

using NewRevolutionaryBank.Data.Models;

public static class DbSeeder
{
	public static async Task SeedRolesAndAdministratorAsync(
		RoleManager<ApplicationRole> roleManager,
		UserManager<ApplicationUser> userManager,
		NrbDbContext context,
		IConfiguration configuration)
	{
		await SeedBankSettingsAsync(context);

		await SeedRolesAsync(roleManager);

		await SeedAdministratorAsync(configuration, userManager);
	}

	private static async Task SeedBankSettingsAsync(NrbDbContext context)
	{
		if (!context.BankSettings.Any())
		{
			context.BankSettings.Add(new()
			{
				TransactionFee = 0.00m
			});

			await context.SaveChangesAsync();
		}
	}

	private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
	{
		string[] roles = new[]
		{
			"Administrator",
			"AccountHolder",
			"Guest",
		};

		foreach (string role in roles)
		{
			if (!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new ApplicationRole(role));
			}
		}
	}

	private static async Task SeedAdministratorAsync(
		IConfiguration configuration,
		UserManager<ApplicationUser> userManager)
	{
		string adminUsername = configuration["Seeding:UserName"]!;
		string adminEmail = configuration["Seeding:Email"]!;
		string adminPassword = configuration["Seeding:Password"]!;

		ApplicationUser? adminExists = await userManager.FindByEmailAsync(adminEmail);

		if (adminExists is null)
		{
			ApplicationUser adminUser = new()
			{
				UserName = adminUsername,
				Email = adminEmail,
				EmailConfirmed = true,
				FirstName = adminUsername,
				LastName = adminUsername
			};

			IdentityResult createdResult = await userManager
				.CreateAsync(adminUser, adminPassword);

			if (createdResult.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Administrator");
			}
		}
	}
}
