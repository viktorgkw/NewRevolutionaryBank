namespace NewRevolutionaryBank.Tests.Models;

using Xunit;

using NewRevolutionaryBank.Data.Models;

public class ApplicationRoleTests
{
	[Fact]
	public void ApplicationRole_Constructor_SetsNameAndCreatedOn()
	{
		// Arrange
		string roleName = "TestRole";

		// Act
		ApplicationRole role = new(roleName);

		// Assert
		Assert.Equal(roleName, role.Name);
		Assert.Equal(DateTime.UtcNow, role.CreatedOn, TimeSpan.FromSeconds(1));
	}
}
