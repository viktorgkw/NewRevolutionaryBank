namespace NewRevolutionaryBank.Web.Hangfire;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;

public class HangfireJobs
{
	private readonly NrbDbContext _dbContext;

	public HangfireJobs(NrbDbContext dbContext) => _dbContext = dbContext;

	public async Task DeleteNotVerifiedAsync()
		=> await _dbContext.Users
			.Where(u => !u.EmailConfirmed &&
				u.CreatedOn.Day == DateTime.Now.AddDays(-1).Day)
			.ExecuteDeleteAsync();

	public async Task DeleteThreeYearOldAccountsAsync()
		=> await _dbContext.Users
			.Where(u => u.IsDeleted &&
				u.DeletedOn!.Value.Year == DateTime.Now.AddYears(-3).Year)
			.ExecuteDeleteAsync();

	public async Task DeleteClosedAccountsAfterYearAsync()
		=> await _dbContext.BankAccounts
			.Where(ba => ba.IsClosed && ba.ClosedDate == DateTime.Now.AddYears(-1))
			.ExecuteDeleteAsync();
}