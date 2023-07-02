namespace NewRevolutionaryBank.Tests.Services.Mocks;

using NewRevolutionaryBank.Services.Messaging.Contracts;

public class MockEmailSender : IEmailSender
{
	public Task SendEmailAsync(string recipient, string subject, string body)
		=> Task.CompletedTask;
}
