namespace NewRevolutionaryBank.Services.Messaging;

using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

using NewRevolutionaryBank.Services.Messaging.Contracts;

public class SendGridEmailSender : IEmailSender
{
	private readonly SendGridClient _client;
	private readonly IConfiguration _configuration;

	public SendGridEmailSender(IConfiguration configuration)
	{
		_configuration = configuration;

		string apiKey = _configuration["SendGrid:ApiKey"]
			?? throw new ArgumentNullException("apiKey", "API Key must not be null!");

		_client = new(apiKey);
	}

	public async Task SendEmailAsync(
		string toEmail,
		string subject,
		string htmlContent)
	{
		if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(htmlContent))
		{
			throw new ArgumentException("Subject and message should be provided.");
		}

		EmailAddress fromAddress = new(
			_configuration["Seeding:Email"],
			_configuration["Seeding:UserName"]);
		EmailAddress toAddress = new(toEmail);

		SendGridMessage message = MailHelper
			.CreateSingleEmail(fromAddress, toAddress, subject, null, htmlContent);

		await _client.SendEmailAsync(message);
	}
}
