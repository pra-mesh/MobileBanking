using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using MobileBanking.Application.MultiTenancy;
using MobileBanking.Logger.Services;
using Xunit;

namespace MobileBanking.Tests.Application.MultiTenancy;

public class TenantProviderTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly TenantProvider _tenantProvider;

    public TenantProviderTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _mockLogger = new Mock<ILoggerService>();
        _mockHttpContext = new Mock<HttpContext>();
        _tenantProvider = new TenantProvider(_mockHttpContextAccessor.Object, _mockLogger.Object);
    }

    [Fact]
    public void GetTenantName_WithValidTenant_ShouldReturnTenantName()
    {
        // Arrange
        var expectedTenant = "test-tenant";
        var items = new Dictionary<object, object?> { { "TenantName", expectedTenant } };
        
        _mockHttpContext.Setup(x => x.Items).Returns(items);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        // Act
        var result = _tenantProvider.GetTenantName();

        // Assert
        result.Should().Be(expectedTenant);
        _mockLogger.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<Exception>()), Times.Never);
    }

    [Fact]
    public void GetTenantName_WithNullHttpContext_ShouldReturnEmptyAndLogError()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext?)null);

        // Act
        var result = _tenantProvider.GetTenantName();

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(x => x.LogError("Tenant not found", It.IsAny<Exception>()), Times.Once);
    }

    [Fact]
    public void GetTenantName_WithMissingTenantInItems_ShouldReturnEmptyAndLogError()
    {
        // Arrange
        var items = new Dictionary<object, object?>();
        
        _mockHttpContext.Setup(x => x.Items).Returns(items);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        // Act
        var result = _tenantProvider.GetTenantName();

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(x => x.LogError("Tenant not found", It.IsAny<Exception>()), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void GetTenantName_WithNullOrEmptyTenant_ShouldReturnEmptyAndLogError(string? tenantValue)
    {
        // Arrange
        var items = new Dictionary<object, object?> { { "TenantName", tenantValue } };
        
        _mockHttpContext.Setup(x => x.Items).Returns(items);
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(_mockHttpContext.Object);

        // Act
        var result = _tenantProvider.GetTenantName();

        // Assert
        result.Should().BeEmpty();
        _mockLogger.Verify(x => x.LogError("Tenant not found", It.IsAny<Exception>()), Times.Once);
    }
}