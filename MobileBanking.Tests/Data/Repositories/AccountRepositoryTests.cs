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

    [Fact]
    public async Task GetAccountDetails_WithValidAccountNumber_ShouldReturnAccountDetails()
    {
        // Arrange
        var accountNo = "123456";
        var expectedAccounts = new List<AccountDetailDTO>
        {
            new()
            {
                SavingName = "Savings",
                mainbookno = "030",
                acno = "030.01",
                AccountHolder = "John Doe",
                accountno = "123456",
                MemName = "John Doe",
                Balance = 1000m,
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
        result.First().Balance.Should().Be(1000m);
        
        _mockSqlDataAccess.Verify(x => x.LoadData<AccountDetailDTO, dynamic>("balancewithfullacno", 
            It.Is<object>(p => p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo))), Times.Once);
    }

    [Fact]
    public async Task AccountCount_WithValidAccountNumber_ShouldReturnCount()
    {
        // Arrange
        var accountNo = "123456";
        var expectedCount = 1;

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

    [Fact]
    public async Task GetBalance_WithValidAccountNumber_ShouldReturnBalance()
    {
        // Arrange
        var accountNo = "123456";
        var expectedBalance = 1500.75m;

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

    [Fact]
    public async Task GetDepBalance_WithValidAccountNumber_ShouldReturnDepositBalance()
    {
        // Arrange
        var accountNo = "030123456";
        var expectedBalance = 2500.50m;

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

    [Fact]
    public async Task GetAccountBranch_WithValidAccountNumber_ShouldReturnBranchId()
    {
        // Arrange
        var accountNo = "123456";
        var expectedBranchId = "01";

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

    [Fact]
    public async Task GetItemName_WithValidAccountNumber_ShouldReturnItemName()
    {
        // Arrange
        var accountNo = "123456";
        var expectedItemName = "Savings Account";

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

    [Fact]
    public async Task GetOpeningBalance_WithValidParameters_ShouldReturnOpeningBalance()
    {
        // Arrange
        var accountNo = "123456";
        var date = DateTime.Now.AddDays(-30);
        var expectedBalance = 500.25m;

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

    [Fact]
    public async Task GetAccountCount_WithValidQuery_ShouldReturnCount()
    {
        // Arrange
        var accountDetail = new AccountDetailPaged
        {
            MemberNo = "M001",
            AccountNumber = "123456",
            MobileNumber = "9841234567"
        };
        var expectedCount = 5;

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

    [Fact]
    public async Task AllAccountFullDetails_WithValidQuery_ShouldReturnAccountDetails()
    {
        // Arrange
        var accountDetail = new AccountDetailPaged
        {
            MemberNo = "M001",
            Offset = 0,
            Limit = 2
        };

        var expectedAccounts = new List<AccountFullDetalDTO>
        {
            new()
            {
                MemberId = "M001",
                MemberName = "John Doe",
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
        result.First().MemberId.Should().Be("M001");
        result.First().MemberName.Should().Be("John Doe");
        
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
    public async Task AccountCount_WithNonExistentAccount_ShouldReturnZero()
    {
        // Arrange
        var accountNo = "999999";

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<int, dynamic>(
            It.IsAny<string>(), It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _accountRepository.AccountCount(accountNo);

        // Assert
        result.Should().Be(0);
    }
}