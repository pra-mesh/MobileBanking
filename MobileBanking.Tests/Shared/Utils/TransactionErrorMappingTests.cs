using FluentAssertions;
using MobileBanking.Shared.Utils;
using Xunit;

namespace MobileBanking.Tests.Shared.Utils;

public class TransactionErrorMappingTests
{
    [Fact]
    public void GetError_WithDestinationMultipleError_ShouldThrowMultipleAccountsFoundException()
    {
        // Arrange
        var message = "destination account has multiple entries";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<MultipleAccountsFoundException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(destAccount);
    }

    [Fact]
    public void GetError_WithDestinationNotFoundError_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var message = "destination account Not found";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<AccountNotFoundException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(destAccount);
    }

    [Fact]
    public void GetError_WithDestinationInsufficientError_ShouldThrowInsufficientBalanceException()
    {
        // Arrange
        var message = "destination account has insufficient balance";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<InsufficientBalanceException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(destAccount);
    }

    [Fact]
    public void GetError_WithSourceMultipleError_ShouldThrowMultipleAccountsFoundException()
    {
        // Arrange
        var message = "source account has multiple entries";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<MultipleAccountsFoundException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(srcAccount);
    }

    [Fact]
    public void GetError_WithSourceNotFoundError_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var message = "source account Not found";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<AccountNotFoundException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(srcAccount);
    }

    [Fact]
    public void GetError_WithSourceInsufficientError_ShouldThrowInsufficientBalanceException()
    {
        // Arrange
        var message = "source account has insufficient balance";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<InsufficientBalanceException>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Contain(srcAccount);
    }

    [Fact]
    public void GetError_WithUnknownError_ShouldThrowGenericException()
    {
        // Arrange
        var message = "unknown error occurred";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => 
            TransactionErrorMapping.GetError(message, destAccount, srcAccount));
        
        exception.Message.Should().Be(message);
    }

    [Fact]
    public void GetError_WithCaseInsensitiveMatch_ShouldThrowCorrectException()
    {
        // Arrange
        var message = "DESTINATION account MULTIPLE entries found";
        var destAccount = "123456";
        var srcAccount = "654321";

        // Act & Assert
        var exception = Assert.Throws<MultipleAccountsFoundException>(() => 
            TransactionErrorMapping.GetError(message.ToLower(), destAccount, srcAccount));
        
        exception.Message.Should().Contain(destAccount);
    }
}