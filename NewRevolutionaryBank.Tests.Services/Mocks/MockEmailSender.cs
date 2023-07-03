namespace NewRevolutionaryBank.Tests.Services.Mocks;

using NewRevolutionaryBank.Services.Messaging.Contracts;

public class MockEmailSender : IEmailSender
{
	public Task SendEmailAsync(string recipient, string subject, string body)
	{
		if (string.IsNullOrEmpty(subject))
		{
			throw new ArgumentNullException(subject);
		}

		if (string.IsNullOrEmpty(body))
		{
			throw new ArgumentNullException(body);
		}

		return Task.CompletedTask;
	}
}
