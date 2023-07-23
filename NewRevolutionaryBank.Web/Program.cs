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
builder.Services.ConfigureIdentity();

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
builder.Services.AddSingleton<IConfiguration>(configuration);

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

// Initialize Roles and Administrator profile
using (IServiceScope initScope = app.Services.CreateScope())
{
	RoleManager<ApplicationRole> roleManager = initScope.ServiceProvider
		.GetRequiredService<RoleManager<ApplicationRole>>();
	UserManager<ApplicationUser> userManager = initScope.ServiceProvider
		.GetRequiredService<UserManager<ApplicationUser>>();
	NrbDbContext context = initScope.ServiceProvider
		.GetRequiredService<NrbDbContext>();

	await DbSeeder.SeedRolesAndAdministratorAsync(
		roleManager,
		userManager,
		context,
		configuration);
}

// Hangfire
app.UseHangfireDashboard();
HangfireJobsExtension.ConfigureJobs();

await app.RunAsync();
