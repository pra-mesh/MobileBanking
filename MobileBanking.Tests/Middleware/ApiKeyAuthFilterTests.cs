using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Moq;
using MobileBanking.Application.AuthService;
using MobileBanking.Middleware;
using Xunit;

namespace MobileBanking.Tests.Middleware;

public class ApiKeyAuthFilterTests
{
    private readonly Mock<IApiKeyValidation> _mockApiKeyValidation;
    private readonly ApiKeyAuthFilter _filter;
    private readonly Mock<HttpContext> _mockHttpContext;
    private readonly Mock<HttpRequest> _mockHttpRequest;
    private readonly AuthorizationFilterContext _context;

    public ApiKeyAuthFilterTests()
    {
        _mockApiKeyValidation = new Mock<IApiKeyValidation>();
        _filter = new ApiKeyAuthFilter(_mockApiKeyValidation.Object);
        
        _mockHttpContext = new Mock<HttpContext>();
        _mockHttpRequest = new Mock<HttpRequest>();
        
        _mockHttpContext.Setup(x => x.Request).Returns(_mockHttpRequest.Object);
        
        var actionContext = new ActionContext
        {
            HttpContext = _mockHttpContext.Object
        };
        
        _context = new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    [Fact]
    public void OnAuthorization_WithMissingApiKey_ShouldReturnBadRequest()
    {
        // Arrange
        var headers = new HeaderDictionary();
        _mockHttpRequest.Setup(x => x.Headers).Returns(headers);

        // Act
        _filter.OnAuthorization(_context);

        // Assert
        _context.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public void OnAuthorization_WithEmptyApiKey_ShouldReturnBadRequest()
    {
        // Arrange
        var headers = new HeaderDictionary
        {
            { "X-API-Key", "" }
        };
        _mockHttpRequest.Setup(x => x.Headers).Returns(headers);

        // Act
        _filter.OnAuthorization(_context);

        // Assert
        _context.Result.Should().BeOfType<BadRequestResult>();
    }

    [Fact]
    public void OnAuthorization_WithInvalidApiKey_ShouldReturnUnauthorized()
    {
        // Arrange
        var apiKey = "invalid-api-key";
        var headers = new HeaderDictionary
        {
            { "X-API-Key", apiKey }
        };
        _mockHttpRequest.Setup(x => x.Headers).Returns(headers);
        _mockApiKeyValidation.Setup(x => x.IsValidAPIKey(apiKey)).Returns(false);

        // Act
        _filter.OnAuthorization(_context);

        // Assert
        _context.Result.Should().BeOfType<UnauthorizedResult>();
    }

    [Fact]
    public void OnAuthorization_WithValidApiKey_ShouldNotSetResult()
    {
        // Arrange
        var apiKey = "valid-api-key";
        var headers = new HeaderDictionary
        {
            { "X-API-Key", apiKey }
        };
        _mockHttpRequest.Setup(x => x.Headers).Returns(headers);
        _mockApiKeyValidation.Setup(x => x.IsValidAPIKey(apiKey)).Returns(true);

        // Act
        _filter.OnAuthorization(_context);

        // Assert
        _context.Result.Should().BeNull();
    }
}