using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Seeding;
using NewRevolutionaryBank.Services;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Services.Messaging;
using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Web.Handlers;
using NewRevolutionaryBank.Web.Hangfire;
using NewRevolutionaryBank.Web.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string was not found!");

IConfigurationRoot configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.Build();

builder.Services.AddDbContext<NrbDbContext>(options =>
{
	options.UseSqlServer(connectionString);
	options.EnableDetailedErrors();
});

// Add Identity
builder.Services
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

// Cookie
builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.HttpOnly = true;
	options.ExpireTimeSpan = TimeSpan.FromMinutes(
		Convert.ToDouble(configuration["ApplicationCookieConfiguration:ExpireTimeSpan"]));
	options.SlidingExpiration = true;
});

// Hangfire
builder.Services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
builder.Services.AddHangfireServer();

// Configuring Services
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<HangfireJobs>();
builder.Services.AddScoped<IsDeletedHandler>();
builder.Services.AddScoped<IEmailSender, MailKitEmailSender>();
builder.Services.AddScoped<IExchangeCurrencyService, ExchangeCurrencyService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IBankAccountService, BankAccountService>();
builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IRatingService, RatingService>();

builder.Services.AddMvc(options =>
	options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Stripe Configuration
Stripe.StripeConfiguration.ApiKey = configuration["Stripe:ApiKey"];

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Middlewares
app.UseMiddleware<IsDeletedMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

// Initialize Roles and Administrator profile
using (IServiceScope initScope = app.Services.CreateScope())
{
	RoleManager<ApplicationRole> roleManager = initScope.ServiceProvider
		.GetRequiredService<RoleManager<ApplicationRole>>();
	UserManager<ApplicationUser> userManager = initScope.ServiceProvider
		.GetRequiredService<UserManager<ApplicationUser>>();
	NrbDbContext context = initScope.ServiceProvider
		.GetRequiredService<NrbDbContext>();

	DbSeeder
		.SeedRolesAndAdministratorAsync(roleManager, userManager, context, configuration)
		.Wait();
}

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate(
	Guid.NewGuid().ToString(),
	(HangfireJobs service) => service.DeleteNotVerifiedAsync(),
	Cron.Daily,
	new RecurringJobOptions
	{
		TimeZone = TimeZoneInfo.Utc
	});

RecurringJob.AddOrUpdate(
	Guid.NewGuid().ToString(),
	(HangfireJobs service) => service.DeleteThreeYearOldAccountsAsync(),
	Cron.Weekly,
	new RecurringJobOptions
	{
		TimeZone = TimeZoneInfo.Utc
	});

RecurringJob.AddOrUpdate(
	Guid.NewGuid().ToString(),
	(HangfireJobs service) => service.DeleteClosedAccountsAfterYearAsync(),
	Cron.Weekly,
	new RecurringJobOptions
	{
		TimeZone = TimeZoneInfo.Utc
	});

await app.RunAsync();
