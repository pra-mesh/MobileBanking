using FluentAssertions;
using Moq;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services.Connection;
using Xunit;

namespace MobileBanking.Tests.Data.Repositories;

public class StatementRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly StatementRepository _statementRepository;

    public StatementRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _statementRepository = new StatementRepository(_mockSqlDataAccess.Object);
    }

    [Theory]
    [InlineData("123456", 30, 2)]
    [InlineData("654321", 7, 5)]
    [InlineData("789012", 90, 10)]
    [InlineData("111111", 1, 1)]
    public async Task FullStatement_WithVariousDateRanges_ShouldReturnExpectedStatements(
        string accountNo, int daysBack, int expectedCount)
    {
        // Arrange
        var fromDate = DateTime.Now.AddDays(-daysBack);
        var toDate = DateTime.Now;

        var expectedStatements = new List<FullStatementDTO>();
        for (int i = 0; i < expectedCount; i++)
        {
            expectedStatements.Add(new FullStatementDTO
            {
                JournalNo = 12345 + i,
                Date = DateTime.Now.AddDays(-i - 1),
                Miti = $"2081/01/{i + 1:00}",
                Description = $"Transaction {i + 1}",
                Amount = (i + 1) * 100m,
                Type = i % 2 == 0 ? "Credit" : "Debit",
                Balance = (i + 1) * 100m
            });
        }

        _mockSqlDataAccess.Setup(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatements);

        // Act
        var result = await _statementRepository.FullStatement(accountNo, fromDate, toDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedCount);
        
        if (expectedCount > 0)
        {
            result.First().Description.Should().Be("Transaction 1");
            result.First().Amount.Should().Be(100m);
        }

        _mockSqlDataAccess.Verify(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo) &&
                p.GetType().GetProperty("fromDate")!.GetValue(p)!.Equals(fromDate) &&
                p.GetType().GetProperty("toDate")!.GetValue(p)!.Equals(toDate))), Times.Once);
    }

    [Theory]
    [InlineData("123456", 5, 3)]
    [InlineData("654321", 10, 7)]
    [InlineData("789012", 1, 1)]
    [InlineData("111111", 0, 0)]
    public async Task MiniStatement_WithVariousTransactionCounts_ShouldReturnExpectedStatements(
        string accountNo, int noOfTransaction, int expectedCount)
    {
        // Arrange
        var expectedStatements = new List<MiniStatementDTO>();
        for (int i = 0; i < expectedCount; i++)
        {
            expectedStatements.Add(new MiniStatementDTO
            {
                JournalNo = 12345 + i,
                Date = DateTime.Now.AddDays(-i - 1),
                Miti = $"2081/01/{i + 1:00}",
                Description = $"Transaction {i + 1}",
                Amount = (i + 1) * 100m,
                Type = i % 2 == 0 ? "Credit" : "Debit"
            });
        }

        _mockSqlDataAccess.Setup(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatements);

        // Act
        var result = await _statementRepository.MiniStatement(accountNo, noOfTransaction);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedCount);
        
        if (expectedCount > 0)
        {
            result.First().Description.Should().Be("Transaction 1");
            result.First().Amount.Should().Be(100m);
            result.First().Type.Should().Be("Credit");
        }

        _mockSqlDataAccess.Verify(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo) &&
                p.GetType().GetProperty("noOfTransaction")!.GetValue(p)!.Equals(noOfTransaction))), Times.Once);
    }

    [Theory]
    [InlineData("999999", 30)]
    [InlineData("888888", 7)]
    [InlineData("777777", 90)]
    public async Task FullStatement_WithNonExistentAccounts_ShouldReturnEmptyList(
        string accountNo, int daysBack)
    {
        // Arrange
        var fromDate = DateTime.Now.AddDays(-daysBack);
        var toDate = DateTime.Now;
        var emptyList = new List<FullStatementDTO>();

        _mockSqlDataAccess.Setup(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _statementRepository.FullStatement(accountNo, fromDate, toDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Theory]
    [InlineData("999999", 5)]
    [InlineData("888888", 10)]
    [InlineData("777777", 1)]
    public async Task MiniStatement_WithNonExistentAccounts_ShouldReturnEmptyList(
        string accountNo, int noOfTransaction)
    {
        // Arrange
        var emptyList = new List<MiniStatementDTO>();

        _mockSqlDataAccess.Setup(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _statementRepository.MiniStatement(accountNo, noOfTransaction);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task FullStatement_WithSingleTransaction_ShouldReturnSingleStatement()
    {
        // Arrange
        var accountNo = "123456";
        var fromDate = DateTime.Now.AddDays(-1);
        var toDate = DateTime.Now;

        var expectedStatements = new List<FullStatementDTO>
        {
            new()
            {
                JournalNo = 12345,
                Date = DateTime.Now.AddHours(-2),
                Miti = "2081/01/01",
                Description = "Opening Balance",
                Amount = 1000m,
                Type = "Credit",
                Balance = 1000m
            }
        };

        _mockSqlDataAccess.Setup(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatements);

        // Act
        var result = await _statementRepository.FullStatement(accountNo, fromDate, toDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Description.Should().Be("Opening Balance");
        result.First().Balance.Should().Be(1000m);
    }

    [Fact]
    public async Task FullStatement_WithLargeDateRange_ShouldHandleCorrectly()
    {
        // Arrange
        var accountNo = "123456";
        var fromDate = DateTime.Now.AddYears(-1);
        var toDate = DateTime.Now;

        var expectedStatements = new List<FullStatementDTO>
        {
            new()
            {
                JournalNo = 12345,
                Date = DateTime.Now.AddMonths(-6),
                Miti = "2080/06/15",
                Description = "Large Date Range Transaction",
                Amount = 5000m,
                Type = "Credit",
                Balance = 5000m
            }
        };

        _mockSqlDataAccess.Setup(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatements);

        // Act
        var result = await _statementRepository.FullStatement(accountNo, fromDate, toDate);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Amount.Should().Be(5000m);
        
        _mockSqlDataAccess.Verify(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("fromDate")!.GetValue(p)!.Equals(fromDate) &&
                p.GetType().GetProperty("toDate")!.GetValue(p)!.Equals(toDate))), Times.Once);
    }
}