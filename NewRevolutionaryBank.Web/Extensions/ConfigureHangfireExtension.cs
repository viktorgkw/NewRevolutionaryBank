namespace NewRevolutionaryBank.Web.Extensions;

using Hangfire;

public static class ConfigureHangfireExtension
{
	public static void ConfigureHangfire(this WebApplication webApp)
	{
		webApp.UseHangfireDashboard();

		RecurringJob.AddOrUpdate(
            Guid.NewGuid().ToString(),
			(HangfireJobsExtension service) => service.DeleteNotVerifiedAsync(),
            Cron.Daily,
			new RecurringJobOptions
            {
				TimeZone = TimeZoneInfo.Utc
			});

        RecurringJob.AddOrUpdate(
            Guid.NewGuid().ToString(),
			(HangfireJobsExtension service) => service.DeleteThreeYearOldAccountsAsync(),
            Cron.Weekly,
			new RecurringJobOptions
            {
				TimeZone = TimeZoneInfo.Utc
			});

        RecurringJob.AddOrUpdate(
            Guid.NewGuid().ToString(),
			(HangfireJobsExtension service) => service.DeleteClosedAccountsAfterYearAsync(),
            Cron.Weekly,
			new RecurringJobOptions
            {
				TimeZone = TimeZoneInfo.Utc
			});

		RecurringJob.AddOrUpdate(
			Guid.NewGuid().ToString(),
			(HangfireJobsExtension service) => service.MonthlyTaxAsync(),
			Cron.Monthly,
			new RecurringJobOptions
			{
				TimeZone = TimeZoneInfo.Utc
			});
	}
}
