namespace NewRevolutionaryBank.Data.Models;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static NewRevolutionaryBank.Common.ModelsConstants;

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
	[StringLength(
		UserConstants.FirstNameMaxLength,
		MinimumLength = UserConstants.FirstNameMinLength,
		ErrorMessage = "First name should be between {0} and {1}!")]
	public string FirstName { get; set; } = null!;

	[Comment("Фамилия")]
	[StringLength(
		UserConstants.LastNameMaxLength,
		MinimumLength = UserConstants.LastNameMinLength,
		ErrorMessage = "Last name should be between {0} and {1}!")]
	public string LastName { get; set; } = null!;

	[Comment("Дата на създаване")]
	public DateTime CreatedOn { get; set; }

	[Comment("Флаг дали профила е изтрит")]
	public bool IsDeleted { get; set; }

	[Comment("Дата на изтриване")]
	public DateTime? DeletedOn { get; set; }

	[Comment("Аватар на потребителя")]
	public byte[]? Avatar { get; set; }

    public ICollection<BankAccount> BankAccounts { get; set; }
}
