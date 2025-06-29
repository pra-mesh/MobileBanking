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

    [Fact]
    public async Task FullStatement_WithValidParameters_ShouldReturnFullStatements()
    {
        // Arrange
        var accountNo = "123456";
        var fromDate = DateTime.Now.AddDays(-30);
        var toDate = DateTime.Now;

        var expectedStatements = new List<FullStatementDTO>
        {
            new()
            {
                JournalNo = 12345,
                Date = DateTime.Now.AddDays(-1),
                Miti = "2081/01/01",
                Description = "Deposit",
                Amount = 1000m,
                Type = "Credit",
                Balance = 1000m
            },
            new()
            {
                JournalNo = 12346,
                Date = DateTime.Now.AddDays(-2),
                Miti = "2080/12/30",
                Description = "Withdrawal",
                Amount = 500m,
                Type = "Debit",
                Balance = 500m
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
        result.Should().HaveCount(2);
        result.First().Description.Should().Be("Deposit");
        result.First().Amount.Should().Be(1000m);
        result.First().Type.Should().Be("Credit");
        
        result.Last().Description.Should().Be("Withdrawal");
        result.Last().Amount.Should().Be(500m);
        result.Last().Type.Should().Be("Debit");

        _mockSqlDataAccess.Verify(x => x.LoadData<FullStatementDTO, dynamic>(
            "SP_MBFullStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo) &&
                p.GetType().GetProperty("fromDate")!.GetValue(p)!.Equals(fromDate) &&
                p.GetType().GetProperty("toDate")!.GetValue(p)!.Equals(toDate))), Times.Once);
    }

    [Fact]
    public async Task MiniStatement_WithValidParameters_ShouldReturnMiniStatements()
    {
        // Arrange
        var accountNo = "123456";
        var noOfTransaction = 5;

        var expectedStatements = new List<MiniStatementDTO>
        {
            new()
            {
                JournalNo = 12345,
                Date = DateTime.Now.AddDays(-1),
                Miti = "2081/01/01",
                Description = "Deposit",
                Amount = 1000m,
                Type = "Credit"
            },
            new()
            {
                JournalNo = 12346,
                Date = DateTime.Now.AddDays(-2),
                Miti = "2080/12/30",
                Description = "Withdrawal",
                Amount = 500m,
                Type = "Debit"
            },
            new()
            {
                JournalNo = 12347,
                Date = DateTime.Now.AddDays(-3),
                Miti = "2080/12/29",
                Description = "Transfer",
                Amount = 200m,
                Type = "Debit"
            }
        };

        _mockSqlDataAccess.Setup(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatements);

        // Act
        var result = await _statementRepository.MiniStatement(accountNo, noOfTransaction);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().Description.Should().Be("Deposit");
        result.First().Amount.Should().Be(1000m);
        result.First().Type.Should().Be("Credit");
        
        result.Skip(1).First().Description.Should().Be("Withdrawal");
        result.Skip(1).First().Amount.Should().Be(500m);
        result.Skip(1).First().Type.Should().Be("Debit");

        _mockSqlDataAccess.Verify(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("accountNo")!.GetValue(p)!.Equals(accountNo) &&
                p.GetType().GetProperty("noOfTransaction")!.GetValue(p)!.Equals(noOfTransaction))), Times.Once);
    }

    [Fact]
    public async Task FullStatement_WithNoTransactions_ShouldReturnEmptyList()
    {
        // Arrange
        var accountNo = "999999";
        var fromDate = DateTime.Now.AddDays(-30);
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

    [Fact]
    public async Task MiniStatement_WithNoTransactions_ShouldReturnEmptyList()
    {
        // Arrange
        var accountNo = "999999";
        var noOfTransaction = 5;
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
    public async Task MiniStatement_WithZeroTransactionCount_ShouldCallWithZero()
    {
        // Arrange
        var accountNo = "123456";
        var noOfTransaction = 0;
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
        
        _mockSqlDataAccess.Verify(x => x.LoadData<MiniStatementDTO, dynamic>(
            "SP_MBMiniStatment", 
            It.Is<object>(p => 
                p.GetType().GetProperty("noOfTransaction")!.GetValue(p)!.Equals(0))), Times.Once);
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