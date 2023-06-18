namespace NewRevolutionaryBank.Services.Contracts;

using NewRevolutionaryBank.ViewModels;

public interface IBankAccountService
{
	Task Create(BankAccountCreateViewModel model);
}
