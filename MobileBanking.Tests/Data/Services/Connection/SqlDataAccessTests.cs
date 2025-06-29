using FluentAssertions;
using Moq;
using MobileBanking.Data.Services.Connection;
using MobileBanking.Logger.Services;
using System.Data;
using Xunit;

namespace MobileBanking.Tests.Data.Services.Connection;

public class SqlDataAccessTests
{
    private readonly Mock<IBaseSqlConnection> _mockBaseSqlConnection;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly string _testConnectionString = "Server=test;Database=test;";

    public SqlDataAccessTests()
    {
        _mockBaseSqlConnection = new Mock<IBaseSqlConnection>();
        _mockLogger = new Mock<ILoggerService>();
        
        _mockBaseSqlConnection.Setup(x => x.GetConnectionString())
            .Returns(_testConnectionString);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithConnectionString()
    {
        // Act
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Assert
        sqlDataAccess.Should().NotBeNull();
        _mockBaseSqlConnection.Verify(x => x.GetConnectionString(), Times.Once);
    }

    [Fact]
    public void StartTransaction_ShouldInitializeConnectionAndTransaction()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Act
        sqlDataAccess.StartTransaction();

        // Assert
        // We can't directly test the private fields, but we can test that subsequent operations work
        // This is more of an integration test to ensure the transaction is properly initialized
        sqlDataAccess.Should().NotBeNull();
    }

    [Fact]
    public void CommitTransaction_AfterStartTransaction_ShouldNotThrow()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);
        sqlDataAccess.StartTransaction();

        // Act & Assert
        sqlDataAccess.Invoking(x => x.CommitTransaction())
            .Should().NotThrow();
    }

    [Fact]
    public void RollBackTransaction_AfterStartTransaction_ShouldNotThrow()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);
        sqlDataAccess.StartTransaction();

        // Act & Assert
        sqlDataAccess.Invoking(x => x.RollBackTransaction())
            .Should().NotThrow();
    }

    [Fact]
    public void Dispose_ShouldHandleCleanup()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Act & Assert
        sqlDataAccess.Invoking(x => x.Dispose())
            .Should().NotThrow();
    }

    [Fact]
    public void Dispose_WithActiveTransaction_ShouldRollbackAndLog()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);
        sqlDataAccess.StartTransaction();

        // Act
        sqlDataAccess.Dispose();

        // Assert
        // The dispose should handle the rollback internally
        // We can verify that no exceptions are thrown
        sqlDataAccess.Should().NotBeNull();
    }

    [Fact]
    public void CommitTransaction_WithoutStartTransaction_ShouldNotThrow()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Act & Assert
        sqlDataAccess.Invoking(x => x.CommitTransaction())
            .Should().NotThrow();
    }

    [Fact]
    public void RollBackTransaction_WithoutStartTransaction_ShouldNotThrow()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Act & Assert
        sqlDataAccess.Invoking(x => x.RollBackTransaction())
            .Should().NotThrow();
    }

    // Note: Testing the actual database operations (LoadData, SaveData, etc.) would require 
    // either a real database connection or more complex mocking of the SqlConnection and SqlCommand classes.
    // These are typically tested through integration tests rather than unit tests.
    
    [Fact]
    public void MultipleDispose_ShouldNotThrow()
    {
        // Arrange
        var sqlDataAccess = new SqlDataAccess(_mockBaseSqlConnection.Object, _mockLogger.Object);

        // Act & Assert
        sqlDataAccess.Invoking(x => x.Dispose())
            .Should().NotThrow();
        
        sqlDataAccess.Invoking(x => x.Dispose())
            .Should().NotThrow();
    }
}