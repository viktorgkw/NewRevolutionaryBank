namespace NewRevolutionaryBank.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
	public ApplicationUser()
	{
		CreatedOn = DateTime.UtcNow;
		IsDeleted = false;
		BankAccounts = new HashSet<BankAccount>();
	}

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public DateTime CreatedOn { get; set; }

	public DateTime? ModifiedOn { get; set; }

	public bool IsDeleted { get; set; }

	public DateTime? DeletedOn { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; }
}
