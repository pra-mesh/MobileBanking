using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using MobileBanking.Application.AuthService;
using MobileBanking.Logger.Services;
using Xunit;

namespace MobileBanking.Tests.Application.AuthService;

public class ApiKeyValidationTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly ApiKeyValidation _apiKeyValidation;

    public ApiKeyValidationTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILoggerService>();
        _apiKeyValidation = new ApiKeyValidation(_mockConfiguration.Object, _mockLogger.Object);
    }

    [Fact]
    public void IsValidAPIKey_WithValidKey_ShouldReturnTrue()
    {
        // Arrange
        var userApiKey = "valid-api-key";
        var configApiKey = "valid-api-key";
        
        _mockConfiguration.Setup(x => x.GetValue<string>("ApiKey"))
            .Returns(configApiKey);

        // Act
        var result = _apiKeyValidation.IsValidAPIKey(userApiKey);

        // Assert
        result.Should().BeTrue();
        _mockLogger.Verify(x => x.LogInformation("API key validated successfully."), Times.Once);
    }

    [Fact]
    public void IsValidAPIKey_WithInvalidKey_ShouldReturnFalse()
    {
        // Arrange
        var userApiKey = "invalid-api-key";
        var configApiKey = "valid-api-key";
        
        _mockConfiguration.Setup(x => x.GetValue<string>("ApiKey"))
            .Returns(configApiKey);

        // Act
        var result = _apiKeyValidation.IsValidAPIKey(userApiKey);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(x => x.LogWarning("API key validation failed: Invalid Key"), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValidAPIKey_WithNullOrEmptyUserKey_ShouldReturnFalse(string userApiKey)
    {
        // Arrange
        var configApiKey = "valid-api-key";
        
        _mockConfiguration.Setup(x => x.GetValue<string>("ApiKey"))
            .Returns(configApiKey);

        // Act
        var result = _apiKeyValidation.IsValidAPIKey(userApiKey);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(x => x.LogWarning("API key validation failed: Invalid Key"), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void IsValidAPIKey_WithNullOrEmptyConfigKey_ShouldReturnFalse(string configApiKey)
    {
        // Arrange
        var userApiKey = "user-api-key";
        
        _mockConfiguration.Setup(x => x.GetValue<string>("ApiKey"))
            .Returns(configApiKey);

        // Act
        var result = _apiKeyValidation.IsValidAPIKey(userApiKey);

        // Assert
        result.Should().BeFalse();
        _mockLogger.Verify(x => x.LogWarning("API key validation failed: Invalid Key"), Times.Once);
    }
}