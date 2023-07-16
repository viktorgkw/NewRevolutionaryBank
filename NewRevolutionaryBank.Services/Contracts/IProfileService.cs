namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.Web.ViewModels.Profile;

public interface IProfileService
{
	Task<MyProfileViewModel> GetProfileDataAsync(string userName);
}
