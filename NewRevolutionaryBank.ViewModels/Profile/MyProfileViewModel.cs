namespace NewRevolutionaryBank.Web.ViewModels.Profile;

public class MyProfileViewModel
{
	public string FirstName { get; set; } = null!;

	public string LastName { get; set; } = null!;

	public string UserName { get; set; } = null!;

	public string Email { get; set; } = null!;

	public string? PhoneNumber { get; set; }

	public DateTime CreatedOn { get; set; }

	public byte[]? Avatar { get; set; }
}
