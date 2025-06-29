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
    [InlineData("")]
    public async Task IsSingleAccount_WithShortAccountNumbers_ShouldThrowAccountNotFoundException(string accountNo)
    {
        // Act & Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Theory]
    [InlineData("123456", 0)]
    [InlineData("654321", -1)]
    public async Task IsSingleAccount_WithZeroOrNegativeAccountCount_ShouldThrowAccountNotFoundException(
        string accountNo, int accountCount)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(accountCount);

        // Act & Assert
        await Assert.ThrowsAsync<AccountNotFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Theory]
    [InlineData("123456", 2)]
    [InlineData("654321", 5)]
    [InlineData("789012", 10)]
    public async Task IsSingleAccount_WithMultipleAccounts_ShouldThrowMultipleAccountsFoundException(
        string accountNo, int accountCount)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(accountCount);

        // Act & Assert
        await Assert.ThrowsAsync<MultipleAccountsFoundException>(() => _accountValidation.IsSingleAccount(accountNo));
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("654321")]
    [InlineData("789012")]
    public async Task IsSingleAccount_WithSingleAccount_ShouldNotThrow(string accountNo)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(1);

        // Act & Assert
        await _accountValidation.Invoking(x => x.IsSingleAccount(accountNo))
            .Should().NotThrowAsync();
    }

    [Theory]
    [InlineData("010123456", true, 1000, 500, false)] // Credit account, debit transaction, insufficient balance
    [InlineData("010123456", false, 1000, 1500, true)] // Credit account, credit transaction, sufficient balance
    [InlineData("110123456", true, 1000, 500, true)] // Debit account, debit transaction, sufficient balance
    [InlineData("020123456", true, 2000, 1500, true)] // Credit account, debit transaction, sufficient balance
    [InlineData("030123456", true, 5000, 1000, true)] // Deposit account, debit transaction, sufficient balance
    public async Task HasSufficientBalance_WithVariousScenarios_ShouldReturnExpectedResult(
        string accountNo, bool isDebit, decimal balance, decimal transactionAmount, bool expectedResult)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.AccountCount(accountNo)).ReturnsAsync(1);
        
        if (accountNo.StartsWith("030"))
        {
            _mockAccountRepository.Setup(x => x.GetDepBalance(accountNo)).ReturnsAsync(balance);
        }
        else
        {
            _mockAccountRepository.Setup(x => x.GetBalance(accountNo)).ReturnsAsync(balance);
        }

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

    [Theory]
    [InlineData("123456", "01")]
    [InlineData("654321", "02")]
    [InlineData("789012", "00")]
    [InlineData("111111", "03")]
    public async Task GetBranch_WithVariousAccounts_ShouldReturnExpectedBranchId(
        string accountNo, string expectedBranch)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.GetAccountBranch(accountNo)).ReturnsAsync(expectedBranch);

        // Act
        var result = await _accountValidation.GetBranch(accountNo);

        // Assert
        result.Should().Be(expectedBranch);
    }

    [Theory]
    [InlineData("123456789", "Savings Account")]
    [InlineData("030123456", "Deposit Account")]
    [InlineData("110654321", "Loan Account")]
    public async Task AccountStructure_WithVariousAccounts_ShouldReturnCorrectStructure(
        string accountNo, string expectedItemName)
    {
        // Arrange
        _mockAccountRepository.Setup(x => x.GetItemName(accountNo)).ReturnsAsync(expectedItemName);

        // Act
        var result = await _accountValidation.AccountStructure(accountNo);

        // Assert
        result.Mano.Should().Be(accountNo.Substring(0, 3));
        result.Acno.Should().Be($"{accountNo.Substring(0, 3)}.{accountNo.Substring(3, 2)}");
        result.ItemCode.Should().Be(accountNo.Substring(5));
        result.ItemName.Should().Be(expectedItemName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void AccountCountValidation_WithNoOrNegativeAccounts_ShouldThrowAccountNotFoundException(int accountCount)
    {
        // Arrange
        var accounts = new List<AccountDetailDTO>();
        for (int i = 0; i < Math.Max(0, accountCount); i++)
        {
            accounts.Add(new AccountDetailDTO 
            { 
                SavingName = "Test", 
                mainbookno = "123", 
                acno = "456", 
                AccountHolder = "Test", 
                accountno = "789", 
                MemName = "Test" 
            });
        }
        var accountNo = "123456";

        // Act & Assert
        Assert.Throws<AccountNotFoundException>(() => 
            _accountValidation.AccountCountValidation(accounts, accountNo));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(5)]
    [InlineData(10)]
    public void AccountCountValidation_WithMultipleAccounts_ShouldThrowMultipleAccountsFoundException(int accountCount)
    {
        // Arrange
        var accounts = new List<AccountDetailDTO>();
        for (int i = 0; i < accountCount; i++)
        {
            accounts.Add(new AccountDetailDTO 
            { 
                SavingName = $"Test{i}", 
                mainbookno = "123", 
                acno = "456", 
                AccountHolder = "Test", 
                accountno = "789", 
                MemName = "Test" 
            });
        }
        var accountNo = "123456";

        // Act & Assert
        Assert.Throws<MultipleAccountsFoundException>(() => 
            _accountValidation.AccountCountValidation(accounts, accountNo));
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("1234")]
    [InlineData("")]
    [InlineData("1")]
    public void InvalidAccount_WithInvalidAccountNumbers_ShouldThrowInvalidAccountException(string accountNo)
    {
        // Act & Assert
        Assert.Throws<InvalidAccountException>(() => _accountValidation.InvalidAccount(accountNo));
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678901234567890")]
    public void InvalidAccount_WithValidAccountNumbers_ShouldNotThrow(string accountNo)
    {
        // Act & Assert
        _accountValidation.Invoking(x => x.InvalidAccount(accountNo))
            .Should().NotThrow();
    }

    [Theory]
    [InlineData("030123456", 5000.50)]
    [InlineData("110654321", 1500.25)]
    [InlineData("020789012", 10000.00)]
    public async Task GeBalance_WithVariousAccountTypes_ShouldReturnCorrectBalance(
        string accountNo, decimal expectedBalance)
    {
        // Arrange
        if (accountNo.StartsWith("030"))
        {
            _mockAccountRepository.Setup(x => x.GetDepBalance(accountNo)).ReturnsAsync(expectedBalance);
        }
        else
        {
            _mockAccountRepository.Setup(x => x.GetBalance(accountNo)).ReturnsAsync(expectedBalance);
        }

        // Act
        var result = await _accountValidation.GeBalance(accountNo);

        // Assert
        result.Should().Be(expectedBalance);
    }
}