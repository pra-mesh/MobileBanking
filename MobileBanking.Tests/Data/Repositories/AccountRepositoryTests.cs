using FluentAssertions;
using Moq;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Models.RequestModels;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services.Connection;
using Xunit;

namespace MobileBanking.Tests.Data.Repositories;

public class AccountRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly AccountRepository _accountRepository;

    public AccountRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _accountRepository = new AccountRepository(_mockSqlDataAccess.Object);
    }

    [Theory]
    [InlineData("123456", "John Doe", 1000.50)]
    [InlineData("654321", "Jane Smith", 2500.75)]
    [InlineData("789012", "Bob Johnson", 500.25)]
    public async Task GetAccountDetails_WithValidAccountNumbers_ShouldReturnAccountDetails(
        string accountNo, string accountHolder, decimal balance)
    {
        // Arrange
        var expectedAccounts = new List<AccountDetailDTO>
        {
            new()
            {
                SavingName = "Savings",
                mainbookno = "030",
                acno = "030.01",
                AccountHolder = accountHolder,
                accountno = accountNo,
                MemName = accountHolder,
                Balance = balance,
                MinBal = 100m,
                InterestRate = 5.5m,
                Gamt = 0,
                Lamt = 0,
                Disabled = false
            }
        };

        _mockSqlDataAccess.Setup(x => x.LoadData<AccountDetailDTO, dynamic>("balancewithfullacno", It.IsAny<object>()))
            .ReturnsAsync(expectedAccounts);

        // Act
        var result = await _accountRepository.GetAccountDetails(accountNo);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().accountno.Should().Be(accountNo);
        result.First().AccountHolder.Should().Be(accountHolder);
        result.First().Balance.Should().Be(balance);
        
        _mockSqlDataAccess.Verify(x => x.LoadData<AccountDetailDTO, dynamic>("balancewithfullacno", 
            It.Is<object>(p => p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("123456", 1)]
    [InlineData("654321", 0)]
    [InlineData("789012", 2)]
    [InlineData("999999", 0)]
    public async Task AccountCount_WithVariousAccountNumbers_ShouldReturnExpectedCount(
        string accountNo, int expectedCount)
    {
        // Arrange
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<int, dynamic>(
            "select count(itemcode) from itms1 where REPLACE(acno,'.','')+ITEMCODE =@accountno", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _accountRepository.AccountCount(accountNo);

        // Assert
        result.Should().Be(expectedCount);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<int, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("123456", 1500.75)]
    [InlineData("654321", 0.00)]
    [InlineData("789012", 25000.50)]
    [InlineData("111111", 100.25)]
    public async Task GetBalance_WithVariousAccountNumbers_ShouldReturnExpectedBalance(
        string accountNo, decimal expectedBalance)
    {
        // Arrange
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<decimal, dynamic>(
            "Select IsNUll((select Balance from ItemBal where REPLACE(acno,'.','')+ITEMCODE =@accountNo),0) as Balance", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _accountRepository.GetBalance(accountNo);

        // Assert
        result.Should().Be(expectedBalance);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<decimal, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("030123456", 2500.50)]
    [InlineData("030654321", 0.00)]
    [InlineData("030789012", 15000.75)]
    public async Task GetDepBalance_WithDepositAccounts_ShouldReturnDepositBalance(
        string accountNo, decimal expectedBalance)
    {
        // Arrange
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<decimal, dynamic>(
            "Select IsNUll((SELECT [dbo].[DepositBalance](@accountno)),0) as Balance", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _accountRepository.GetDepBalance(accountNo);

        // Assert
        result.Should().Be(expectedBalance);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<decimal, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("123456", "01")]
    [InlineData("654321", "02")]
    [InlineData("789012", "00")]
    [InlineData("111111", "03")]
    public async Task GetAccountBranch_WithVariousAccounts_ShouldReturnExpectedBranchId(
        string accountNo, string expectedBranchId)
    {
        // Arrange
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<string, dynamic>(
            "select top 1 branchId from Itms1 where REPLACE(acno,'.','')+Itemcode =@accountNO", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedBranchId);

        // Act
        var result = await _accountRepository.GetAccountBranch(accountNo);

        // Assert
        result.Should().Be(expectedBranchId);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<string, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => p.GetType().GetProperty("accountNO")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("123456", "Savings Account")]
    [InlineData("654321", "Current Account")]
    [InlineData("789012", "Fixed Deposit")]
    [InlineData("111111", "Loan Account")]
    public async Task GetItemName_WithVariousAccounts_ShouldReturnExpectedItemName(
        string accountNo, string expectedItemName)
    {
        // Arrange
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<string, dynamic>(
            It.IsAny<string>(), 
            It.IsAny<object>()))
            .ReturnsAsync(expectedItemName);

        // Act
        var result = await _accountRepository.GetItemName(accountNo);

        // Assert
        result.Should().Be(expectedItemName);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<string, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => p.GetType().GetProperty("fullAccountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Theory]
    [InlineData("123456", 30, 500.25)]
    [InlineData("654321", 60, 1000.50)]
    [InlineData("789012", 90, 0.00)]
    public async Task GetOpeningBalance_WithVariousParameters_ShouldReturnExpectedBalance(
        string accountNo, int daysBack, decimal expectedBalance)
    {
        // Arrange
        var date = DateTime.Now.AddDays(-daysBack);
        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<decimal, dynamic>(
            It.IsAny<string>(), 
            It.IsAny<object>()))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _accountRepository.GetOpeningBalance(accountNo, date);

        // Assert
        result.Should().Be(expectedBalance);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<decimal, dynamic>(
            It.IsAny<string>(), 
            It.Is<object>(p => 
                p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo) &&
                p.GetType().GetProperty("date")!.GetValue(p)!.Equals(date))), Times.Once);
    }

    [Theory]
    [InlineData("M001", "123456", "9841234567", 5)]
    [InlineData("M002", null, "9851234567", 3)]
    [InlineData(null, "654321", null, 1)]
    [InlineData(null, null, "9861234567", 0)]
    public async Task GetAccountCount_WithVariousQueries_ShouldReturnExpectedCount(
        string? memberNo, string? accountNumber, string? mobileNumber, int expectedCount)
    {
        // Arrange
        var accountDetail = new AccountDetailPaged
        {
            MemberNo = memberNo,
            AccountNumber = accountNumber,
            MobileNumber = mobileNumber
        };

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<int, dynamic>(
            It.IsAny<string>(), 
            accountDetail))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _accountRepository.GetAccountCount(accountDetail);

        // Assert
        result.Should().Be(expectedCount);
        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<int, dynamic>(
            It.IsAny<string>(), accountDetail), Times.Once);
    }

    [Theory]
    [InlineData(0, 2, "M001")]
    [InlineData(2, 2, "M002")]
    [InlineData(4, 2, "M003")]
    public async Task AllAccountFullDetails_WithVariousPagination_ShouldReturnPagedResults(
        int offset, int limit, string memberId)
    {
        // Arrange
        var accountDetail = new AccountDetailPaged
        {
            MemberNo = memberId,
            Offset = offset,
            Limit = limit
        };

        var expectedAccounts = new List<AccountFullDetalDTO>
        {
            new()
            {
                MemberId = memberId,
                MemberName = "Test Member",
                Address = "Test Address",
                MobileNumber = "9841234567",
                AccountNumber = "123456",
                AccountType = "Savings",
                SavingType = "Normal",
                LedgerBalance = 1000m,
                AvailableBalance = 900m,
                MinBal = 100m
            }
        };

        _mockSqlDataAccess.Setup(x => x.LoadData<AccountFullDetalDTO, dynamic>(
            "sp_GetDepositAccountDetails", accountDetail))
            .ReturnsAsync(expectedAccounts);

        // Act
        var result = await _accountRepository.AllAccountFullDetails(accountDetail);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().MemberId.Should().Be(memberId);
        
        _mockSqlDataAccess.Verify(x => x.LoadData<AccountFullDetalDTO, dynamic>(
            "sp_GetDepositAccountDetails", accountDetail), Times.Once);
    }

    [Fact]
    public async Task GetAccountDetails_WithEmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var accountNo = "999999";
        var emptyList = new List<AccountDetailDTO>();

        _mockSqlDataAccess.Setup(x => x.LoadData<AccountDetailDTO, dynamic>("balancewithfullacno", It.IsAny<object>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _accountRepository.GetAccountDetails(accountNo);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AllAccountFullDetails_WithEmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var accountDetail = new AccountDetailPaged
        {
            MemberNo = "NONEXISTENT",
            Offset = 0,
            Limit = 10
        };

        var emptyList = new List<AccountFullDetalDTO>();

        _mockSqlDataAccess.Setup(x => x.LoadData<AccountFullDetalDTO, dynamic>(
            "sp_GetDepositAccountDetails", accountDetail))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _accountRepository.AllAccountFullDetails(accountDetail);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}