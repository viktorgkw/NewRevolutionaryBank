namespace NewRevolutionaryBank.Web.Extensions;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;

public class HangfireJobsExtension
{
    private readonly NrbDbContext _dbContext;

    public HangfireJobsExtension(NrbDbContext dbContext) => _dbContext = dbContext;

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

    public async Task MonthlyTaxAsync()
    {
        BankSettings bankSettings = await _dbContext.BankSettings
            .FirstAsync();

        await _dbContext.BankAccounts
            .Where(ba => !ba.IsClosed)
            .ExecuteUpdateAsync(props =>
                props.SetProperty(
	                prop => prop.Balance,
	                prop => prop.Balance - bankSettings.MonthlyTax
                ));
    }
}