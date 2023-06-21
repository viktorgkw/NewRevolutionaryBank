using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Seeding;
using NewRevolutionaryBank.Models;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging;
using NewRevolutionaryBank.Services.Messaging.Contracts;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string was not found!");

IConfigurationRoot configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.Build();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(connectionString);
	options.EnableDetailedErrors();
});

// Add Identity
builder.Services
	.AddIdentity<ApplicationUser, ApplicationRole>(cfg =>
	{
		// Password settings
		cfg.Password.RequireDigit = true;
		cfg.Password.RequireLowercase = true;
		cfg.Password.RequireUppercase = true;
		cfg.Password.RequireNonAlphanumeric = true;
		cfg.Password.RequiredLength = 8;
		cfg.Password.RequiredUniqueChars = 3;

		// Lockout settings
		cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
		cfg.Lockout.MaxFailedAccessAttempts = 5;
		cfg.Lockout.AllowedForNewUsers = true;

		// User settings
		cfg.User.RequireUniqueEmail = true;
	})
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders()
	.AddDefaultUI();

// Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
	options.SlidingExpiration = true;
});

// Hangfire
builder.Services.AddHangfire(config =>
{
	config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();

builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();
builder.Services.AddScoped<IHangfireService, HangfireService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IAdministratorService, AdministratorService>();

builder.Services.AddMvc(options =>
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Initialize Roles and Administrator profile
using (var initScope = app.Services.CreateScope())
{
	var roleManager = initScope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
	var userManager = initScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

	DbSeeder
		.SeedRolesAndAdministratorAsync(roleManager, userManager, configuration)
		.Wait();
}

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate(
	Guid.NewGuid().ToString(),
	(HangfireService service) => service.DeleteNotVerified(),
	Cron.Daily,
	new RecurringJobOptions
	{
		TimeZone = TimeZoneInfo.Utc
	});

RecurringJob.AddOrUpdate(
	Guid.NewGuid().ToString(),
	(HangfireService service) => service.DeleteThreeYearOldAccounts(),
	Cron.Weekly,
	new RecurringJobOptions
	{
		TimeZone = TimeZoneInfo.Utc
	});

app.Run();

