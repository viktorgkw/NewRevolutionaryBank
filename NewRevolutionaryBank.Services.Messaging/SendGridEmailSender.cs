namespace NewRevolutionaryBank.Services.Messaging;

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

using NewRevolutionaryBank.Services.Messaging.Contracts;

public class SendGridEmailSender : IEmailSender
{
	private readonly IConfiguration _configuration;

	public SendGridEmailSender(IConfiguration configuration)
	{
		_configuration = configuration;
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

		var message = new MimeMessage();

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

		using var client = new SmtpClient();

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
