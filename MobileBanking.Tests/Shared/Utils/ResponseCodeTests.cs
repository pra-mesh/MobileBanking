using FluentAssertions;
using MobileBanking.Shared.Utils;
using Xunit;

namespace MobileBanking.Tests.Shared.Utils;

public class ResponseCodeTests
{
    [Theory]
    [InlineData("Unable To Process", "05")]
    [InlineData("Transaction Not Allowed", "39")]
    [InlineData("Insufficient Fund", "51")]
    [InlineData("Account Dormant", "52")]
    [InlineData("Account Closed", "54")]
    [InlineData("Account Restricted", "62")]
    [InlineData("Invalid Account", "76")]
    [InlineData("System Error", "96")]
    [InlineData("Duplicate Reversal", "98")]
    [InlineData("Format Error", "30")]
    [InlineData("Success", "00")]
    public void GetResponseCode_WithKnownErrorMessages_ShouldReturnCorrectCode(string errorMessage, string expectedCode)
    {
        // Arrange
        var exception = new Exception(errorMessage);

        // Act
        var result = ResponseCode.GetResponseCode(exception);

        // Assert
        result.Should().Be(expectedCode);
    }

    [Fact]
    public void GetResponseCode_WithUnknownErrorMessage_ShouldReturnSystemErrorCode()
    {
        // Arrange
        var exception = new Exception("Unknown error message");

        // Act
        var result = ResponseCode.GetResponseCode(exception);

        // Assert
        result.Should().Be("96"); // System Error code
    }

    [Fact]
    public void GetResponseCode_WithPartialMatch_ShouldReturnMatchingCode()
    {
        // Arrange
        var exception = new Exception("This is an Invalid Account error");

        // Act
        var result = ResponseCode.GetResponseCode(exception);

        // Assert
        result.Should().Be("76"); // Invalid Account code
    }

    [Fact]
    public void GetResponseCode_WithMultipleMatches_ShouldReturnFirstMatch()
    {
        // Arrange
        var exception = new Exception("System Error: Unable To Process");

        // Act
        var result = ResponseCode.GetResponseCode(exception);

        // Assert
        result.Should().Be("05"); // Unable To Process comes first in the dictionary iteration
    }
}