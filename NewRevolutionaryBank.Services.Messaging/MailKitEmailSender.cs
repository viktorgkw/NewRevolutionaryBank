namespace NewRevolutionaryBank.Services.Messaging;

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

using NewRevolutionaryBank.Services.Messaging.Contracts;

public class MailKitEmailSender : IEmailSender
{
	private readonly IConfiguration _configuration;

	public MailKitEmailSender(IConfiguration configuration) => _configuration = configuration;

	public async Task SendEmailAsync(
		string toEmail,
		string subject,
		string htmlContent)
	{
		if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(htmlContent))
		{
			throw new ArgumentException("Subject and message should be provided.");
		}

		MimeMessage message = new();

		message.From.Add(new MailboxAddress(
			_configuration["EmailSender:SenderName"],
			_configuration["EmailSender:SenderEmail"]));

		message.To.Add(new MailboxAddress(
			toEmail,
			toEmail));

		message.Subject = subject;

		BodyBuilder bodyBuilder = new()
		{
			HtmlBody = htmlContent
		};

		message.Body = bodyBuilder.ToMessageBody();

		using SmtpClient client = new();

		try
		{
			client.Connect(
			"smtp.gmail.com",
			587,
			SecureSocketOptions.StartTls);

			client.Authenticate(
				_configuration["EmailSender:SenderEmail"],
				_configuration["EmailSender:SenderPassword"]);

			await client.SendAsync(message);
		}
		finally
		{
			client.Disconnect(true);
		}
	}
}
