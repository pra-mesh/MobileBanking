using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MobileBanking.Application.Models;
using MobileBanking.Application.Services;
using MobileBanking.Controllers;
using MobileBanking.Models.Request.ISmart;
using MobileBanking.Models.Response.ISmart;
using Xunit;

namespace MobileBanking.Tests.Controllers;

public class ISmartControllerTests
{
    private readonly Mock<IBalanceInquiry> _mockBalanceInquiry;
    private readonly Mock<IStatementServices> _mockStatementServices;
    private readonly Mock<ITransactionService> _mockTransactionService;
    private readonly ISmartController _controller;

    public ISmartControllerTests()
    {
        _mockBalanceInquiry = new Mock<IBalanceInquiry>();
        _mockStatementServices = new Mock<IStatementServices>();
        _mockTransactionService = new Mock<ITransactionService>();
        _controller = new ISmartController(
            _mockBalanceInquiry.Object,
            _mockStatementServices.Object,
            _mockTransactionService.Object);
    }

    [Fact]
    public async Task Inquiry_WithValidRequest_ShouldReturnBalanceInquiryResponse()
    {
        // Arrange
        var request = new BalanceInquiryRequest
        {
            accountNumber = "123456",
            branchId = "01"
        };

        var accountDetail = new AccountDetailModel
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

        _mockBalanceInquiry.Setup(x => x.GetBalance(It.IsAny<BalanceInquiryModel>()))
            .ReturnsAsync(accountDetail);

        // Act
        var result = await _controller.Inquiry(request);

        // Assert
        result.Should().NotBeNull();
        result.availableBalance.Should().Be(1000m);
        result.minimumBalance.Should().Be(100m);
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task FullStatement_WithValidRequest_ShouldReturnFullStatementResponse()
    {
        // Arrange
        var request = new FullStatementRequest
        {
            accountNumber = "123456",
            branchId = "01",
            fromDate = DateTime.Now.AddDays(-30),
            toDate = DateTime.Now
        };

        var fullStatement = new FullStatementModel
        {
            minimumBalance = 100m,
            availableBalance = 1000m,
            statementList = new List<Statement>
            {
                new() { date = DateTime.Now.AddDays(-1), remarks = "Deposit", amount = 500m, type = "Credit", Balance = 1500m },
                new() { date = DateTime.Now.AddDays(-2), remarks = "Withdrawal", amount = 200m, type = "Debit", Balance = 1000m }
            }
        };

        _mockStatementServices.Setup(x => x.FullStatementBalance(It.IsAny<FullStatmentInquiryModel>()))
            .ReturnsAsync(fullStatement);

        // Act
        var result = await _controller.FullStatement(request);

        // Assert
        result.Should().NotBeNull();
        result.minimumBalance.Should().Be(100m);
        result.availableBalance.Should().Be(1000m);
        result.statementList.Should().HaveCount(2);
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task MiniStatement_WithValidRequest_ShouldReturnMiniStatementResponse()
    {
        // Arrange
        var request = new MiniStatementRequest
        {
            accountNumber = "123456",
            branchId = "01",
            count = 5
        };

        var miniStatement = new MiniStatementModel
        {
            minimumBalance = 100m,
            availableBalance = 1000m,
            statementList = new List<MiniStatement>
            {
                new() { date = DateTime.Now.AddDays(-1), remarks = "Deposit", amount = 500m, type = "Credit" },
                new() { date = DateTime.Now.AddDays(-2), remarks = "Withdrawal", amount = 200m, type = "Debit" }
            }
        };

        _mockStatementServices.Setup(x => x.MiniStatementBalance(It.IsAny<MiniStatementInquiryModel>()))
            .ReturnsAsync(miniStatement);

        // Act
        var result = await _controller.MiniStatement(request);

        // Assert
        result.Should().NotBeNull();
        result.minimumBalance.Should().Be(100m);
        result.availableBalance.Should().Be(1000m);
        result.statementList.Should().HaveCount(2);
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task FundTransferResponse_WithValidRequest_ShouldReturnFundTransferResponse()
    {
        // Arrange
        var request = new FundTransferRequest
        {
            srcAccount = "123456",
            destAccount = "654321",
            amount = 1000m,
            description1 = "Test Transfer",
            tranCode = "TXN001",
            tranDate = DateTime.Now
        };

        var fundTransferResult = new FundTransferedModel
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890,
            balance = 4000m,
            transactionBalance = 1000m,
            transactionIdentifier = "TXN001"
        };

        _mockTransactionService.Setup(x => x.FundTransferbyProcWithBalance(It.IsAny<FundTransferModel>()))
            .ReturnsAsync(fundTransferResult);

        // Act
        var result = await _controller.FundTransferResponse(request);

        // Assert
        result.Should().NotBeNull();
        result.balance.Should().Be(4000m);
        result.transactionId.Should().Be("12345");
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task FundTransferStatus_WithValidRequest_ShouldReturnStatusResponse()
    {
        // Arrange
        var request = new FundTransferStatusCheckRequest
        {
            transactionIdentifier = "TXN001"
        };

        var statusResult = new TransactionStatusModel
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        _mockTransactionService.Setup(x => x.FundTransferStatus(It.IsAny<TranactionStatusInquiryModel>()))
            .ReturnsAsync(statusResult);

        // Act
        var result = await _controller.FundTransferStatus(request);

        // Assert
        result.Should().NotBeNull();
        result.transactionId.Should().Be("12345");
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task ReversalIsmart_WithValidRequest_ShouldReturnReversalResponse()
    {
        // Arrange
        var request = new FundtransferReverseRequest
        {
            transactionIdentifier = "TXN001",
            newTransactionIdentifier = "REV001"
        };

        var reversalResult = new ReversalStatusModel
        {
            BVRCNO = "REV001",
            Journalno = 54321,
            TransNoA = 98765,
            Message = "Reversal Successful"
        };

        _mockTransactionService.Setup(x => x.TransactionReversal(It.IsAny<ReversalRequestModel>()))
            .ReturnsAsync(reversalResult);

        // Act
        var result = await _controller.ReversalIsmart(request);

        // Assert
        result.Should().NotBeNull();
        result.transactionId.Should().Be("54321");
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task AccountDetail_WithValidRequest_ShouldReturnAccountsDetailResponse()
    {
        // Arrange
        var request = new AccountDetailByIdRequest
        {
            accountNumber = "123456",
            mobileNumber = "9841234567"
        };

        var accountDetails = new List<AccountDetailFullModel>
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

        _mockBalanceInquiry.Setup(x => x.GetAccountList(It.IsAny<AllDetailsQueryModel>()))
            .ReturnsAsync(accountDetails);

        // Act
        var result = await _controller.AccountDetail(request);

        // Assert
        result.Should().NotBeNull();
        result.accountList.Should().HaveCount(1);
        result.accountList!.First().memberId.Should().Be("M001");
        result.accountList.First().memberName.Should().Be("John Doe");
        result.isoResponseCode.Should().Be("00");
    }

    [Fact]
    public async Task AccountDetail_WithNoAccountNumberOrMobile_ShouldThrowException()
    {
        // Arrange
        var request = new AccountDetailByIdRequest
        {
            accountNumber = "",
            mobileNumber = ""
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.AccountDetail(request));
        exception.Message.Should().Contain("Required either mobile number or account no");
    }

    [Fact]
    public async Task AccountDetail_WithNoAccountsFound_ShouldThrowException()
    {
        // Arrange
        var request = new AccountDetailByIdRequest
        {
            accountNumber = "123456"
        };

        _mockBalanceInquiry.Setup(x => x.GetAccountList(It.IsAny<AllDetailsQueryModel>()))
            .ReturnsAsync(new List<AccountDetailFullModel>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => _controller.AccountDetail(request));
        exception.Message.Should().Contain("No Account found");
    }
}