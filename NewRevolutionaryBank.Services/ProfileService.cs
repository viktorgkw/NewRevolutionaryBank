namespace NewRevolutionaryBank.Services;

using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;
using NewRevolutionaryBank.Services.Contracts;
using NewRevolutionaryBank.Web.ViewModels.Profile;

public class ProfileService : IProfileService
{
    private readonly NrbDbContext _context;

	public ProfileService(NrbDbContext context) => _context = context;

	/// <returns>View model of the user profile.</returns>
	/// <exception cref="ArgumentNullException"></exception>
	public async Task<MyProfileViewModel> GetProfileDataAsync(string userName)
	{
		ApplicationUser? user = await _context.Users
			.FirstOrDefaultAsync(u => u.UserName == userName);

		ArgumentNullException.ThrowIfNull(user);

		return new MyProfileViewModel
		{
			UserName = user.UserName!,
			FirstName = user.FirstName,
			LastName = user.LastName,
			Email = user.Email!,
			Avatar = user.Avatar,
			CreatedOn = user.CreatedOn,
			PhoneNumber = user.PhoneNumber
		};
	}
}
