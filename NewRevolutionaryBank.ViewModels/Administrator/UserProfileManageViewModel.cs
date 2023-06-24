namespace NewRevolutionaryBank.Web.ViewModels.Administrator;

public class UserProfileManageViewModel
{
    public Guid Id { get; set; }

	public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public bool IsDeleted { get; set; }

	public DateTime? DeletedOn { get; set; }

	public int BankAccountsCount { get; set; }
}
