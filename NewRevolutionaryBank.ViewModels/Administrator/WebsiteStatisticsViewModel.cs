namespace NewRevolutionaryBank.Web.ViewModels.Administrator;

using System.ComponentModel.DataAnnotations;

public class WebsiteStatisticsViewModel
{
    [Display(Name = "Total Registered Users")]
    public int TotalRegisteredUsers { get; set; }

	[Display(Name = "New Users")]
	public int NewUsers { get; set; }

	[Display(Name = "Total Bank Accounts")]
	public int TotalBankAccounts { get; set; }

	[Display(Name = "Total Deposits")]
	public int TotalDeposits { get; set; }

	[Display(Name = "Average DepositPrice")]
	public decimal AverageDepositPrice { get; set; }

	[Display(Name = "Total Transactions")]
	public int TotalTransactions { get; set; }

	[Display(Name = "Average Transaction Price")]
	public decimal AverageTransactionPrice { get; set; }

	[Display(Name = "Total Reviews")]
	public int TotalReviews { get; set; }

	[Display(Name = "Average Website Review Rate")]
	public double AverageWebsiteReviewRate { get; set; }
}
