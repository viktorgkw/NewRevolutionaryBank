using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Data.Seeding;
using NewRevolutionaryBank.Web.Extensions;
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
builder.Services.ConfigureIdentity(configuration);

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
builder.Services.ServiceConfigurator();
builder.Services.ConfigureOtherServices();
builder.Services.AddSingleton(configuration);

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
app.UseMiddleware<NotFoundMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();
app.MapRazorPages();

// Seeding
await app.SeedBankSettingsAsync();
await app.SeedRolesAsync();
await app.SeedAdministratorAsync();
await app.SeedTestUserAsync();

// Hangfire
app.ConfigureHangfire();

await app.RunAsync();
