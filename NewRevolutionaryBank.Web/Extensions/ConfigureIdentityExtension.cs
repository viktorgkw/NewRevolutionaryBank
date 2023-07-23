namespace NewRevolutionaryBank.Web.Extensions;

using Microsoft.AspNetCore.Identity;

using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data;

public static class ConfigureIdentityExtension
{
	public static void ConfigureIdentity(
		this IServiceCollection services,
		IConfigurationRoot configuration)
		=> services
			.AddIdentity<ApplicationUser, ApplicationRole>(cfg =>
			{
				// Password settings
				cfg.Password.RequireDigit = Convert.ToBoolean(
					configuration["IdentitySettings:RequireDigit"]);
				cfg.Password.RequireLowercase = Convert.ToBoolean(
					configuration["IdentitySettings:RequireLowercase"]);
				cfg.Password.RequireUppercase = Convert.ToBoolean(
					configuration["IdentitySettings:RequireUppercase"]);
				cfg.Password.RequireNonAlphanumeric = Convert.ToBoolean(
					configuration["IdentitySettings:RequireNonAlphanumeric"]);
				cfg.Password.RequiredLength = Convert.ToInt32(
					configuration["IdentitySettings:RequiredLength"]);
				cfg.Password.RequiredUniqueChars = Convert.ToInt32(
					configuration["IdentitySettings:RequiredUniqueChars"]);

				// Lockout settings
				cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(
					Convert.ToDouble(configuration["IdentitySettings:DefaultLockoutTimeSpan"]));
				cfg.Lockout.MaxFailedAccessAttempts = Convert.ToInt32(
					configuration["IdentitySettings:MaxFailedAccessAttempts"]);
				cfg.Lockout.AllowedForNewUsers = Convert.ToBoolean(
					configuration["IdentitySettings:AllowedForNewUsers"]);

				// User settings
				cfg.User.RequireUniqueEmail = Convert.ToBoolean(
					configuration["IdentitySettings:RequireUniqueEmail"]);
			})
			.AddEntityFrameworkStores<NrbDbContext>()
			.AddDefaultTokenProviders()
			.AddDefaultUI();
}
