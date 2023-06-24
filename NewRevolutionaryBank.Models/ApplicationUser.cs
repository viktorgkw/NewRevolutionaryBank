namespace NewRevolutionaryBank.Data.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Comment("Потребителски акаунт")]
public class ApplicationUser : IdentityUser<Guid>
{
	public ApplicationUser()
	{
		CreatedOn = DateTime.UtcNow;
		IsDeleted = false;
		BankAccounts = new HashSet<BankAccount>();
	}

	[Comment("Собствено име")]
	public string? FirstName { get; set; }

	[Comment("Фамилия")]
	public string? LastName { get; set; }

	[Comment("Дата на създаване")]
	public DateTime CreatedOn { get; set; }

	[Comment("Дата на последна промяна")]
	public DateTime? ModifiedOn { get; set; }

	[Comment("Флаг дали профила е изтрит")]
	public bool IsDeleted { get; set; }

	[Comment("Дата на изтриване")]
	public DateTime? DeletedOn { get; set; }

	public ICollection<BankAccount> BankAccounts { get; set; }
}
