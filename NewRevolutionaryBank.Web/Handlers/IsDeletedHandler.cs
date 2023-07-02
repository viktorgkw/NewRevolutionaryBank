namespace NewRevolutionaryBank.Web.Handlers;

using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using NewRevolutionaryBank.Data;
using NewRevolutionaryBank.Data.Models;

public class IsDeletedHandler
{
    private readonly NrbDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IsDeletedHandler(
        NrbDbContext context,
        SignInManager<ApplicationUser> signInManager)
    {
        _context = context;
        _signInManager = signInManager;
    }

    public async Task<bool> MustLogoutByUsernameAsync(string userName)
    {
        ApplicationUser? foundUser = await _context.Users
            .FirstOrDefaultAsync(user => user.UserName == userName);

        if (foundUser is null)
        {
            return false;
        }

        if (foundUser.IsDeleted)
        {
            await _signInManager.SignOutAsync();
            return true;
        }

        return false;
    }
}
