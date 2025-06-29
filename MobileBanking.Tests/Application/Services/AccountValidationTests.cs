using FluentAssertions;
using Moq;
using MobileBanking.Application.Services;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using Xunit;

namespace MobileBanking.Tests.Application.Services;

public class AccountValidationTests
{
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly AccountValidation _accountValidation;

    public AccountValidationTests()
    {
        _mockAccountRepository = new Mock<IAccountRepository>();
        _accountValidation = new AccountValidation(_mockAccountRepository.Object);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234")]
    [InlineData("123")]
    public async Task IsSingleAccount_WithShortAccountNumber_ShouldThrowAccountNotFoundException(string accountNo)
    {
        // Act & Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Fact]
    public async Task IsSingleAccount_WithZeroAccountCount_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var accountNo = "123456";
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(0);

        // Act & Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Fact]
    public async Task IsSingleAccount_WithMultipleAccounts_ShouldThrowMultipleAccountsFoundException()
    {
        // Arrange
        var accountNo = "123456";
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(2);

        // Act & Assert
        await Assert.ThrowsAsync<MultipleAccountsFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Fact]
    public async Task IsSingleAccount_WithSingleAccount_ShouldNotThrow()
    {
        // Arrange
        var accountNo = "123456";
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(1);

        // Act & Assert
        await _accountValidation.Invoking(x => x.IsSingleAccount(accountNo))
            .Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("010123456", true, 1000, 500, false)] // Credit account, debit transaction, insufficient balance
    [InlineData("010123456", false, 1000, 1500, true)] // Credit account, credit transaction, sufficient balance
    [InlineData("110123456", true, 1000, 500, true)] // Debit account, debit transaction, sufficient balance
    public async Task HasSufficientBalance_ShouldReturnExpectedResult(
        string accountNo, bool isDebit, decimal balance, decimal transactionAmount, bool expectedResult)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(1);
        _mockAccountRepository.Setup(x => x.GetBalance(accountNo)).ReturnsAsync(balance);

        // Act
        if (expectedResult)
        {
            var result = await _accountValidation.HasSufficientBalance(accountNo, isDebit, transactionAmount);
            result.Should().Be(expectedResult);
        }
        else
        {
            await Assert.ThrowsAsync<InsufficientBalanceException>(() => 
                _accountValidation.HasSufficientBalance(accountNo, isDebit, transactionAmount));
        }
    }

    [Fact]
    public async Task GetBranch_ShouldReturnBranchId()
    {
        // Arrange
        var accountNo = "123456";
        var expectedBranch = "01";
        _mockAccountRepository.Setup(x => x.GetAccountBranch(accountNo)).ReturnsAsync(expectedBranch);

        // Act
        var result = await _accountValidation.GetBranch(accountNo);

        // Assert
        result.Should().Be(expectedBranch);
    }

    [Fact]
    public async Task AccountStructure_ShouldReturnCorrectStructure()
    {
        // Arrange
        var accountNo = "123456789";
        var expectedItemName = "Test Item";
        _mockAccountRepository.Setup(x => x.GetItemName(accountNo)).ReturnsAsync(expectedItemName);

        // Act
        var result = await _accountValidation.AccountStructure(accountNo);

        // Assert
        result.Mano.Should().Be("123");
        result.Acno.Should().Be("123.45");
        result.ItemCode.Should().Be("6789");
        result.ItemName.Should().Be(expectedItemName);
    }

    [Fact]
    public void AccountCountValidation_WithNoAccounts_ShouldThrowAccountNotFoundException()
    {
        // Arrange
        var accounts = new List<AccountDetailDTO>();
        var accountNo = "123456";

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() => 
            _accountValidation.AccountCountValidation(accounts, accountNo));
    }

    [Fact]
    public void AccountCountValidation_WithMultipleAccounts_ShouldThrowMultipleAccountsFoundException()
    {
        // Arrange
        var accounts = new List<AccountDetailDTO>
        {
            new() { SavingName = "Test1", mainbookno = "123", acno = "456", AccountHolder = "Test", accountno = "789", MemName = "Test" },
            new() { SavingName = "Test2", mainbookno = "123", acno = "456", AccountHolder = "Test", accountno = "789", MemName = "Test" }
        };
        var accountNo = "123456";

        // Act & Assert
        Assert.Throws<MultipleAccountsFoundException>(() => 
            _accountValidation.AccountCountValidation(accounts, accountNo));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234")]
    [InlineData("")]
    public void InvalidAccount_WithInvalidAccountNumber_ShouldThrowInvalidAccountException(string accountNo)
    {
        // Act & Assert
        Assert.Throws<InvalidAccountException>(() => _accountValidation.InvalidAccount(accountNo));
    }

    [Fact]
    public void InvalidAccount_WithValidAccountNumber_ShouldNotThrow()
    {
        // Arrange
        var accountNo = "123456";

        // Act & Assert
        _accountValidation.Invoking(x => x.InvalidAccount(accountNo))
            .Should().NotThrow();
    }
}