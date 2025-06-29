using FluentAssertions;
using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
using Xunit;

namespace MobileBanking.Tests.Mappings;

public class BusinessToDataMappingTests
{
    [Fact]
    public void ToTransactionModel_ShouldMapCorrectly()
    {
        // Arrange
        var fundTransfer = new FundTransferModel
        {
            transDate = DateTime.Now,
            description1 = "Test Transfer",
            description2 = "Mobile Banking",
            transCode = "TXN001",
            enteredBy = "TestUser"
        };

        // Act
        var result = BusinessToDataMapping.ToTransactionModel(fundTransfer);

        // Assert
        result.Should().NotBeNull();
        result.TransDate.Should().Be(fundTransfer.transDate);
        result.TrDesc.Should().Be("Test Transfer");
        result.TTID.Should().Be("TXN001");
        result.TransactionType.Should().Be("Mobile banking");
        result.PartyType.Should().Be("Mobile");
        result.PartyDocument.Should().Be("Mobile Banking");
        result.EnteredBy.Should().Be("TestUser");
    }

    [Fact]
    public void ToJournalNoDTO_WithIBT_ShouldMapCorrectly()
    {
        // Arrange
        var fundTransfer = new FundTransferModel
        {
            transDate = DateTime.Now,
            description1 = "Test Transfer",
            srcBranchId = "01",
            enteredBy = "TestUser"
        };

        // Act
        var result = BusinessToDataMapping.ToJournalNoDTO(fundTransfer, true);

        // Assert
        result.Should().NotBeNull();
        result.tdate.Should().Be(fundTransfer.transDate);
        result.description.Should().Be("IBT - Test Transfer");
        result.branchId.Should().Be("01");
        result.user.Should().Be("TestUser");
    }

    [Fact]
    public void ToJournalNoDTO_WithoutIBT_ShouldMapCorrectly()
    {
        // Arrange
        var fundTransfer = new FundTransferModel
        {
            transDate = DateTime.Now,
            description1 = "Test Transfer",
            srcBranchId = "01",
            enteredBy = "TestUser"
        };

        // Act
        var result = BusinessToDataMapping.ToJournalNoDTO(fundTransfer, false);

        // Assert
        result.Should().NotBeNull();
        result.tdate.Should().Be(fundTransfer.transDate);
        result.description.Should().Be("Test Transfer");
        result.branchId.Should().Be("01");
        result.user.Should().Be("TestUser");
    }

    [Fact]
    public void ToTransactionDataDTO_WithDebitTransaction_ShouldMapCorrectly()
    {
        // Arrange
        var fundTransfer = new FundTransferModel
        {
            transDate = DateTime.Now,
            description1 = "Test Transfer",
            description2 = "Mobile Banking",
            description3 = "Additional Info",
            transCode = "TXN001",
            amount = 1000m,
            enteredBy = "TestUser"
        };

        var account = new AccountIdentifier
        {
            Mano = "030",
            Acno = "030.01",
            ItemCode = "123456",
            ItemName = "Savings Account"
        };

        var journalno = 12345;
        var transno = 67890;
        var branchId = "01";

        // Act
        var result = BusinessToDataMapping.ToTransactionDataDTO(
            fundTransfer, account, journalno, transno, true, branchId);

        // Assert
        result.Should().NotBeNull();
        result.Journalno.Should().Be(12345);
        result.BVRCNO.Should().Be("TXN001");
        result.transDate.Should().Be(fundTransfer.transDate);
        result.branchid.Should().Be("01");
        result.mano.Should().Be("030");
        result.acno.Should().Be("030.01");
        result.itemcode.Should().Be("123456");
        result.itemname.Should().Be("Savings Account");
        result.itemlocation.Should().Be("TestUser");
        result.receivedpaidBy.Should().Be("Mobile Banking");
        result.particulars.Should().Be("Mobile Banking");
        result.dr_cr.Should().Be("DR");
        result.Debit.Should().Be(1000m);
        result.Credit.Should().Be(0m);
        result.description.Should().Be("Test Transfer");
        result.Remarks1.Should().Be("Test Transfer");
        result.Remarks2.Should().Be("Mobile Banking");
        result.Remarks3.Should().Be("Mobile Banking");
        result.Remarks4.Should().Be("Additional Info");
        result.TransNoa.Should().Be(67890);
        result.EnteredBy.Should().Be("TestUser");
    }

    [Fact]
    public void ToTransactionDataDTO_WithCreditTransaction_ShouldMapCorrectly()
    {
        // Arrange
        var fundTransfer = new FundTransferModel
        {
            amount = 1000m,
            enteredBy = "TestUser"
        };

        var account = new AccountIdentifier();
        var journalno = 12345;
        var transno = 67890;

        // Act
        var result = BusinessToDataMapping.ToTransactionDataDTO(
            fundTransfer, account, journalno, transno, false, null);

        // Assert
        result.Should().NotBeNull();
        result.dr_cr.Should().Be("CR");
        result.Debit.Should().Be(0m);
        result.Credit.Should().Be(1000m);
    }

    [Fact]
    public void ToReverseTansactionDTO_ShouldMapCorrectly()
    {
        // Arrange
        var request = new ReversalRequestModel
        {
            BVRCNO = "TXN001",
            Newbvrcno = "REV001",
            JournalNo = 12345,
            Description = "Reversal Test",
            enteredBY = "TestUser"
        };

        // Act
        var result = BusinessToDataMapping.ToReverseTansactionDTO(request);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Newbvrcno.Should().Be("REV001");
        result.JournalNo.Should().Be(12345);
        result.Description.Should().Be("Reversal Test");
        result.enteredBY.Should().Be("TestUser");
    }

    [Fact]
    public void ToTransactionProcDTO_ShouldMapCorrectly()
    {
        // Arrange
        var request = new FundTransferModel
        {
            srcAccount = "123456",
            destAccount = "654321",
            description1 = "Test Transfer",
            description2 = "Mobile Banking",
            description3 = "Additional Info",
            transCode = "TXN001",
            transDate = DateTime.Now,
            enteredBy = "TestUser",
            amount = 1000m
        };

        // Act
        var result = BusinessToDataMapping.ToTransactionProcDTO(request);

        // Assert
        result.Should().NotBeNull();
        result.SrcAccount.Should().Be("123456");
        result.DestAccount.Should().Be("654321");
        result.Description1.Should().Be("Test Transfer");
        result.Description2.Should().Be("Mobile Banking");
        result.Description3.Should().Be("Additional Info");
        result.TransCode.Should().Be("TXN001");
        result.TransDate.Should().Be(request.transDate);
        result.EnteredBy.Should().Be("TestUser");
        result.Amount.Should().Be(1000m);
    }

    [Fact]
    public void ToAccountDetailPagedDTO_ShouldMapCorrectly()
    {
        // Arrange
        var request = new AllDetailsQueryModel
        {
            MemberNo = "M001",
            AccountNumber = "123456",
            MobileNumber = "9841234567"
        };

        // Act
        var result = BusinessToDataMapping.ToAccountDetailPagedDTO(request);

        // Assert
        result.Should().NotBeNull();
        result.MemberNo.Should().Be("M001");
        result.AccountNumber.Should().Be("123456");
        result.MobileNumber.Should().Be("9841234567");
    }
}