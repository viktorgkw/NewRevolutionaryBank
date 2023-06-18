namespace NewRevolutionaryBank.Services.Messaging.Contracts;

public interface IEmailSender
{
	Task SendEmailAsync(
		string toEmail,
		string subject,
		string htmlContent);
}
