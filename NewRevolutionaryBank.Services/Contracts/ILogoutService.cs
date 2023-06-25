namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Data.Models;

public interface ILogoutService
{
	Task<bool> MustLogoutAsync(ApplicationUser? user);

	Task<bool> MustLogoutByUsernameAsync(string userName);
}
