namespace NewRevolutionaryBank.Web.ViewModels.Administrator;

using NewRevolutionaryBank.Data.Models;

public class UserProfileDetailsViewModel
{
	public Guid Id { get; set; }

	public string Email { get; set; } = null!;

	public string UserName { get; set; } = null!;

	public string? PhoneNumber { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public DateTime CreatedOn { get; set; }

	public bool IsDeleted { get; set; }

	public DateTime? DeletedOn { get; set; }

	public List<BankAccount> BankAccounts { get; set; } = null!;
}
