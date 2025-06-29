using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using MobileBanking.Exceptions;
using MobileBanking.Logger.Services;
using MobileBanking.Models.Response;
using System.Text.Json;
using Xunit;

namespace MobileBanking.Tests.Exceptions;

public class GlobalExceptionHandlerTests
{
    private readonly Mock<ILoggerService> _mockLogger;
    private readonly GlobalExceptionHandler _handler;
    private readonly DefaultHttpContext _httpContext;

    public GlobalExceptionHandlerTests()
    {
        _mockLogger = new Mock<ILoggerService>();
        _handler = new GlobalExceptionHandler(_mockLogger.Object);
        _httpContext = new DefaultHttpContext();
        _httpContext.Response.Body = new MemoryStream();
    }

    [Fact]
    public async Task TryHandleAsync_WithException_ShouldReturnTrueAndWriteResponse()
    {
        // Arrange
        var exception = new Exception("Test exception");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, cancellationToken);

        // Assert
        result.Should().BeTrue();
        _httpContext.Response.StatusCode.Should().Be(200);
        
        // Verify logging
        _mockLogger.Verify(x => x.LogError($"Exception occurred: {exception.Message}", exception), Times.Once);

        // Verify response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        var response = JsonSerializer.Deserialize<BaseResponse<string>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        response.Should().NotBeNull();
        response!.statusCode.Should().Be("96"); // System Error code
        response.data.Should().Contain("System Error Test exception");
    }

    [Fact]
    public async Task TryHandleAsync_WithAccountNotFoundException_ShouldReturnCorrectErrorCode()
    {
        // Arrange
        var exception = new AccountNotFoundException("123456");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        // Verify response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        var response = JsonSerializer.Deserialize<BaseResponse<string>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        response.Should().NotBeNull();
        response!.statusCode.Should().Be("76"); // Invalid Account code
    }

    [Fact]
    public async Task TryHandleAsync_WithInsufficientBalanceException_ShouldReturnCorrectErrorCode()
    {
        // Arrange
        var exception = new InsufficientBalanceException("123456");
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _handler.TryHandleAsync(_httpContext, exception, cancellationToken);

        // Assert
        result.Should().BeTrue();
        
        // Verify response body
        _httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
        var reader = new StreamReader(_httpContext.Response.Body);
        var responseBody = await reader.ReadToEndAsync();
        
        var response = JsonSerializer.Deserialize<BaseResponse<string>>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        response.Should().NotBeNull();
        response!.statusCode.Should().Be("51"); // Insufficient Fund code
    }
}