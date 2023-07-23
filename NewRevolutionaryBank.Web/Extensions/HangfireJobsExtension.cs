namespace NewRevolutionaryBank.Web.Extensions;

using Hangfire;

using NewRevolutionaryBank.Web.HangfireJobs;

public static class HangfireJobsExtension
{
	public static void ConfigureJobs()
	{
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
	}
}
