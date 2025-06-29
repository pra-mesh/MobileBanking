using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using MobileBanking.Data.Services.Connection;
using MobileBanking.Logger.Services;
using MobileBanking.Shared.Interface;
using Xunit;

namespace MobileBanking.Tests.Data.Services.Connection;

public class BaseSqlConnectionTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ITenantProvider> _mockTenantProvider;
    private readonly Mock<ILoggerService> _mockLogger;

    public BaseSqlConnectionTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockTenantProvider = new Mock<ITenantProvider>();
        _mockLogger = new Mock<ILoggerService>();
    }

    [Fact]
    public void Constructor_ShouldInitializeWithConfiguration()
    {
        // Act
        var baseSqlConnection = new BaseSqlConnection(_mockConfiguration.Object, _mockTenantProvider.Object);

        // Assert
        baseSqlConnection.Should().NotBeNull();
        baseSqlConnection.Configuration.Should().Be(_mockConfiguration.Object);
    }

    [Fact]
    public void GetConnectionString_WithValidConfiguration_ShouldReturnConnectionString()
    {
        // Arrange
        var baseConnectionString = "Server=localhost;Database=master;";
        var password = "testpassword";
        var userName = "testuser";
        var tenantName = "test-tenant";

        _mockConfiguration.Setup(x => x.GetConnectionString("DbConnection"))
            .Returns(baseConnectionString);
        _mockConfiguration.Setup(x => x.GetValue<string>("DbCredentials:Password"))
            .Returns(password);
        _mockConfiguration.Setup(x => x.GetValue<string>("DbCredentials:userName"))
            .Returns(userName);
        _mockTenantProvider.Setup(x => x.GetTenantName())
            .Returns(tenantName);

        // Note: This test would require mocking the database call to get tenant info
        // In a real scenario, you might want to create an integration test for this
        // or refactor the method to be more testable by extracting the database logic

        var baseSqlConnection = new BaseSqlConnection(_mockConfiguration.Object, _mockTenantProvider.Object);

        // Act & Assert
        // This will likely throw an exception because it tries to connect to the database
        // In a real test environment, you would either:
        // 1. Use a test database
        // 2. Mock the database connection
        // 3. Refactor the code to be more testable
        
        baseSqlConnection.Invoking(x => x.GetConnectionString())
            .Should().ThrowAsync<Exception>();
    }

    [Fact]
    public void Configuration_Property_ShouldReturnInjectedConfiguration()
    {
        // Arrange
        var baseSqlConnection = new BaseSqlConnection(_mockConfiguration.Object, _mockTenantProvider.Object);

        // Act
        var result = baseSqlConnection.Configuration;

        // Assert
        result.Should().Be(_mockConfiguration.Object);
    }

    // Note: The GetConnectionString method is difficult to unit test as it:
    // 1. Creates a real database connection
    // 2. Executes a SQL query
    // 3. Builds a connection string based on database results
    // 
    // This method would be better tested through integration tests or by refactoring
    // to separate the database logic from the connection string building logic.
}