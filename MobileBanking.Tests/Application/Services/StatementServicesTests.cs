using FluentAssertions;
using Moq;
using MobileBanking.Application.Models;
using MobileBanking.Application.Services;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using Xunit;

namespace MobileBanking.Tests.Application.Services;

public class StatementServicesTests
{
    private readonly Mock<IStatementRepository> _mockStatementRepository;
    private readonly Mock<IAccountValidation> _mockAccountValidation;
    private readonly Mock<IAccountRepository> _mockAccountRepository;
    private readonly StatementServices _statementServices;

    public StatementServicesTests()
    {
        _mockStatementRepository = new Mock<IStatementRepository>();
        _mockAccountValidation = new Mock<IAccountValidation>();
        _mockAccountRepository = new Mock<IAccountRepository>();
        _statementServices = new StatementServices(
            _mockStatementRepository.Object,
            _mockAccountValidation.Object,
            _mockAccountRepository.Object);
    }

    [Fact]
    public async Task MiniStatementBalance_WithValidRequest_ShouldReturnMiniStatement()
    {
        // Arrange
        var request = new MiniStatementInquiryModel
        {
            accountNumber = "123456",
            branchId = "01",
            count = 5
        };

        var accountDetail = new AccountDetailDTO
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
        };

        var statements = new List<MiniStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-1), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Withdrawal", Amount = 200m, Type = "Debit" }
        };

        _mockAccountRepository.Setup(x => x.GetAccountDetails(request.accountNumber))
            .ReturnsAsync(new List<AccountDetailDTO> { accountDetail });
        _mockStatementRepository.Setup(x => x.MiniStatement(request.accountNumber, request.count))
            .ReturnsAsync(statements);

        // Act
        var result = await _statementServices.MiniStatementBalance(request);

        // Assert
        result.Should().NotBeNull();
        result.minimumBalance.Should().Be(100m);
        result.availableBalance.Should().Be(1000m);
        result.statementList.Should().HaveCount(2);
        result.statementList!.First().amount.Should().Be(500m);
        result.statementList.First().type.Should().Be("Credit");
    }

    [Fact]
    public async Task FullStatementBalance_WithValidRequest_ShouldReturnFullStatement()
    {
        // Arrange
        var request = new FullStatmentInquiryModel
        {
            accountNumber = "123456",
            branchId = "01",
            fromDate = DateTime.Now.AddDays(-30),
            toDate = DateTime.Now
        };

        var accountDetail = new AccountDetailDTO
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
        };

        var statements = new List<FullStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-1), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Withdrawal", Amount = 200m, Type = "Debit" }
        };

        _mockAccountRepository.Setup(x => x.GetAccountDetails(request.accountNumber))
            .ReturnsAsync(new List<AccountDetailDTO> { accountDetail });
        _mockStatementRepository.Setup(x => x.FullStatement(request.accountNumber, request.fromDate, request.toDate))
            .ReturnsAsync(statements);

        // Act
        var result = await _statementServices.FullStatementBalance(request);

        // Assert
        result.Should().NotBeNull();
        result.minimumBalance.Should().Be(100m);
        result.availableBalance.Should().Be(1000m);
        result.statementList.Should().HaveCount(2);
        
        // Check running balance calculation
        var firstStatement = result.statementList!.First();
        firstStatement.Balance.Should().Be(500m); // First credit transaction
        
        var secondStatement = result.statementList.Last();
        secondStatement.Balance.Should().Be(300m); // 500 - 200 = 300
    }

    [Fact]
    public async Task MiniStatement_WithValidRequest_ShouldReturnStatementList()
    {
        // Arrange
        var request = new MiniStatementInquiryModel
        {
            accountNumber = "123456",
            count = 3
        };

        var statements = new List<MiniStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-1), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Withdrawal", Amount = 200m, Type = "Debit" },
            new() { Date = DateTime.Now.AddDays(-3), Description = "Transfer", Amount = 100m, Type = "Debit" }
        };

        _mockStatementRepository.Setup(x => x.MiniStatement(request.accountNumber, request.count))
            .ReturnsAsync(statements);

        // Act
        var result = await _statementServices.MiniStatement(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.First().amount.Should().Be(500m);
        result.First().type.Should().Be("Credit");
    }

    [Fact]
    public async Task FullStatements_WithValidRequest_ShouldReturnStatementListWithBalance()
    {
        // Arrange
        var request = new FullStatmentInquiryModel
        {
            accountNumber = "123456",
            fromDate = DateTime.Now.AddDays(-7),
            toDate = DateTime.Now
        };

        var statements = new List<FullStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-3), Description = "Opening Balance", Amount = 1000m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-1), Description = "Withdrawal", Amount = 200m, Type = "Debit" }
        };

        _mockStatementRepository.Setup(x => x.FullStatement(request.accountNumber, request.fromDate, request.toDate))
            .ReturnsAsync(statements);

        // Act
        var result = await _statementServices.FullStatements(request);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        
        // Verify running balance calculation
        result[0].Balance.Should().Be(1000m); // Opening balance
        result[1].Balance.Should().Be(1500m); // 1000 + 500
        result[2].Balance.Should().Be(1300m); // 1500 - 200
    }
}