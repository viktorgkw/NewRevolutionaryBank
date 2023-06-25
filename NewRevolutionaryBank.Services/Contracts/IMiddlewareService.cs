namespace NewRevolutionaryBank.Services.Contracts;

public interface IMiddlewareService
{
	Task<bool> MustLogoutByUsernameAsync(string userName);
}
