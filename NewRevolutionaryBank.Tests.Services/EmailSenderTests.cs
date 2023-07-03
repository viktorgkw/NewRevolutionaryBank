namespace NewRevolutionaryBank.Tests.Services;

using Xunit;

using NewRevolutionaryBank.Services.Messaging.Contracts;
using NewRevolutionaryBank.Tests.Services.Mocks;

public class EmailSenderTests
{
	private readonly IEmailSender _emailSender;

	public EmailSenderTests() => _emailSender = new MockEmailSender();

	[Fact]
	public async Task SendEmailAsync_ValidArguments_Success()
	{
		// Arrange
		string toEmail = "recipient@example.com";
		string subject = "Test Subject";
		string htmlContent = "<html><body>Test Content</body></html>";

		// Act
		await _emailSender.SendEmailAsync(toEmail, subject, htmlContent);

		// Assert
	}

	[Fact]
	public async Task SendEmailAsync_MissingSubject_ThrowsArgumentNullException()
	{
		// Arrange
		string toEmail = "recipient@example.com";
		string subject = string.Empty;
		string htmlContent = "<html><body>Test Content</body></html>";

		// Act and Assert
		await Assert.ThrowsAsync<ArgumentNullException>(
			async () => await _emailSender.SendEmailAsync(toEmail, subject, htmlContent));
	}

	[Fact]
	public async Task SendEmailAsync_MissingContent_ThrowsArgumentNullException()
	{
		// Arrange
		string toEmail = "recipient@example.com";
		string subject = "Test Subject";
		string htmlContent = string.Empty;

		// Act and Assert
		await Assert.ThrowsAsync<ArgumentNullException>(
			async () => await _emailSender.SendEmailAsync(toEmail, subject, htmlContent));
	}
}
