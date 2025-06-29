using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using MobileBanking.Middleware;
using MobileBanking.Models.Response;
using System.Text.Json;
using Xunit;

namespace MobileBanking.Tests.Middleware;

public class TenantMiddlewareTests
{
    private readonly Mock<RequestDelegate> _mockNext;
    private readonly TenantMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public TenantMiddlewareTests()
    {
        _mockNext = new Mock<RequestDelegate>();
        _middleware = new TenantMiddleware(_mockNext.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task InvokeAsync_WithValidTenant_ShouldCallNext()
    {
        // Arrange
        var tenantName = "test-tenant";
        _httpContext.Request.Headers.Add("tenant", tenantName);

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Items["TenantName"].Should().Be(tenantName);
        _mockNext.Verify(x => x(_httpContext), Times.Once);
    }

    [Fact]
    public async Task InvokeAsync_WithMissingTenant_ShouldReturn403()
    {
        // Arrange
        // No tenant header added

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(403);
        _mockNext.Verify(x => x(_httpContext), Times.Never);

        // Verify response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        var response = JsonSerializer.Deserialize<BaseResponse<string>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        response.Should().NotBeNull();
        response!.statusCode.Should().Be("96");
        response.data.Should().Be("Missing Tenant");
    }

    [Fact]
    public async Task InvokeAsync_WithEmptyTenant_ShouldReturn403()
    {
        // Arrange
        _httpContext.Request.Headers.Add("tenant", "");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(403);
        _mockNext.Verify(x => x(_httpContext), Times.Never);
    }
}