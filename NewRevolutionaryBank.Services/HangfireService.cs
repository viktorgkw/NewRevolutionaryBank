namespace NewRevolutionaryBank.Services;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Services.Contracts;

public class HangfireService : IHangfireService
{
	private readonly ApplicationDbContext _dbContext;

	public HangfireService(ApplicationDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task DeleteNotVerified()
		=> await _dbContext.Users
			.Where(u => !u.EmailConfirmed &&
				u.CreatedOn.Day == DateTime.Now.AddDays(-1).Day)
			.ExecuteDeleteAsync();

	public async Task DeleteThreeYearOldAccounts()
		=> await _dbContext.Users
			.Where(u => u.IsDeleted &&
				u.DeletedOn!.Value.Year == DateTime.Now.AddYears(-3).Year)
			.ExecuteDeleteAsync();
}