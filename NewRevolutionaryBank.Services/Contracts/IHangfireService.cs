namespace NewRevolutionaryBank.Services.Contracts;

public interface IHangfireService
{
	Task DeleteNotVerifiedAsync();
	Task DeleteThreeYearOldAccountsAsync();
}
