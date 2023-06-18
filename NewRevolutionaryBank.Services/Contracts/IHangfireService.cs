namespace NewRevolutionaryBank.Services.Contracts;

public interface IHangfireService
{
	Task DeleteNotVerified();
	Task DeleteThreeYearOldAccounts();
}
