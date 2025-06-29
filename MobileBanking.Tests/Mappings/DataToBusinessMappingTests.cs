using FluentAssertions;
using MobileBanking.Application.Mappings;
using MobileBanking.Data.Models.DTOs;
using Xunit;

namespace MobileBanking.Tests.Mappings;

public class DataToBusinessMappingTests
{
    [Fact]
    public void ToAccountDetailModel_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new AccountDetailDTO
        {
            SavingName = "Savings Account",
            mainbookno = "030",
            acno = "030.01",
            AccountHolder = "John Doe",
            accountno = "123456",
            MemName = "John Doe",
            Disabled = false,
            InterestRate = 5.5m,
            MinBal = 100m,
            Balance = 1000m,
            Gamt = 50m,
            Lamt = 25m
        };

        // Act
        var result = DataToBusinessMapping.ToAccountDetailModel(dto);

        // Assert
        result.Should().NotBeNull();
        result.SavingName.Should().Be("Savings Account");
        result.mainbookno.Should().Be("030");
        result.acno.Should().Be("030.01");
        result.AccountHolder.Should().Be("John Doe");
        result.accountno.Should().Be("123456");
        result.MemName.Should().Be("John Doe");
        result.Disabled.Should().BeFalse();
        result.InterestRate.Should().Be(5.5m);
        result.MinBal.Should().Be(100m);
        result.Balance.Should().Be(1000m);
        result.Gamt.Should().Be(50m);
        result.Lamt.Should().Be(25m);
    }

    [Fact]
    public void ToStatementList_ShouldCalculateRunningBalance()
    {
        // Arrange
        var statements = new List<FullStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-3), Description = "Opening", Amount = 1000m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-1), Description = "Withdrawal", Amount = 200m, Type = "Debit" }
        };

        // Act
        var result = DataToBusinessMapping.ToStatementList(statements);

        // Assert
        result.Should().HaveCount(3);
        result[0].Balance.Should().Be(1000m); // Opening balance
        result[1].Balance.Should().Be(1500m); // 1000 + 500
        result[2].Balance.Should().Be(1300m); // 1500 - 200
    }

    [Fact]
    public void ToMiniStatmentList_ShouldMapCorrectly()
    {
        // Arrange
        var statements = new List<MiniStatementDTO>
        {
            new() { Date = DateTime.Now.AddDays(-1), Description = "Deposit", Amount = 500m, Type = "Credit" },
            new() { Date = DateTime.Now.AddDays(-2), Description = "Withdrawal", Amount = 200m, Type = "Debit" }
        };

        // Act
        var result = DataToBusinessMapping.ToMiniStatmentList(statements);

        // Assert
        result.Should().HaveCount(2);
        result[0].date.Should().Be(statements[0].Date);
        result[0].remarks.Should().Be("Deposit");
        result[0].amount.Should().Be(500m);
        result[0].type.Should().Be("Credit");
        
        result[1].date.Should().Be(statements[1].Date);
        result[1].remarks.Should().Be("Withdrawal");
        result[1].amount.Should().Be(200m);
        result[1].type.Should().Be("Debit");
    }

    [Fact]
    public void ToFundTransferedModel_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new TransactionStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        // Act
        var result = DataToBusinessMapping.ToFundTransferedModel(dto);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
    }

    [Fact]
    public void ToReversalStatusModel_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new ReversalStatusDTO
        {
            BVRCNO = "REV001",
            Journalno = 54321,
            TransNoA = 98765,
            Message = "Reversal Successful"
        };

        // Act
        var result = DataToBusinessMapping.ToReversalStatusModel(dto);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("REV001");
        result.Journalno.Should().Be(54321);
        result.TransNoA.Should().Be(98765);
        result.Message.Should().Be("Reversal Successful");
    }

    [Fact]
    public void ToAccountDetailFullModel_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new AccountFullDetalDTO
        {
            MemberId = "M001",
            MemberName = "John Doe",
            Address = "Test Address",
            MobileNumber = "9841234567",
            AccountNumber = "123456",
            BranchCode = "01",
            IsActive = true,
            DateOfBirth = "1990-01-01",
            Gender = "Male",
            AccountType = "Savings",
            SavingType = "Normal",
            LockedAmount = 0m,
            GuarantedAmt = 0m,
            LedgerBalance = 1000m,
            MinBal = 100m,
            AvailableBalance = 900m,
            AccruedInterest = 10m,
            InterestRate = 5.5m,
            IdType = "Citizenship",
            IdNumber = "12345",
            IdIssuePlace = "Kathmandu",
            IssueDate = "2010-01-01"
        };

        // Act
        var result = DataToBusinessMapping.ToAccountDetailFullModel(dto);

        // Assert
        result.Should().NotBeNull();
        result.MemberId.Should().Be("M001");
        result.MemberName.Should().Be("John Doe");
        result.Address.Should().Be("Test Address");
        result.MobileNumber.Should().Be("9841234567");
        result.AccountNumber.Should().Be("123456");
        result.BranchCode.Should().Be("01");
        result.IsActive.Should().BeTrue();
        result.DateOfBirth.Should().Be("1990-01-01");
        result.Gender.Should().Be("Male");
        result.AccountType.Should().Be("Savings");
        result.SavingType.Should().Be("Normal");
        result.LockedAmount.Should().Be(0m);
        result.GuarantedAmt.Should().Be(0m);
        result.LedgerBalance.Should().Be(1000m);
        result.MinBal.Should().Be(100m);
        result.AvailableBalance.Should().Be(900m);
        result.AccruedInterest.Should().Be(10m);
        result.InterestRate.Should().Be(5.5m);
        result.IdType.Should().Be("Citizenship");
        result.IdNumber.Should().Be("12345");
        result.IdIssuePlace.Should().Be("Kathmandu");
        result.IssueDate.Should().Be("2010-01-01");
    }
}