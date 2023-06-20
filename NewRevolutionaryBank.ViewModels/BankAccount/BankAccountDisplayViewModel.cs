namespace NewRevolutionaryBank.ViewModels.BankAccount;

public class BankAccountDisplayViewModel
{
    public Guid Id { get; set; }

    public string SecureId { get; set; } = null!;

    public decimal Balance { get; set; }
}
