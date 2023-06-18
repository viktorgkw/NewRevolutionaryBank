namespace NewRevolutionaryBank.Models;

using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
	public ApplicationUser()
	{
		CreatedOn = DateTime.UtcNow;
		IsDeleted = false;
		Roles = new HashSet<IdentityUserRole<string>>();
		Claims = new HashSet<IdentityUserClaim<string>>();
		Logins = new HashSet<IdentityUserLogin<string>>();
	}

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public DateTime CreatedOn { get; set; }

	public DateTime? ModifiedOn { get; set; }

	public bool IsDeleted { get; set; }

	public DateTime? DeletedOn { get; set; }

	public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }

	public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

	public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

    public BankAccount? BankAccount { get; set; }
}
