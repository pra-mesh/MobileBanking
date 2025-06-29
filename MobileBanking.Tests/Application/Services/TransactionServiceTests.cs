using FluentAssertions;
using Moq;
using MobileBanking.Application.Models;
using MobileBanking.Application.Services;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using Xunit;

namespace MobileBanking.Tests.Application.Services;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _mockTransactionRepository;
    private readonly Mock<IAccountValidation> _mockAccountValidation;
    private readonly TransactionService _transactionService;

    public TransactionServiceTests()
    {
        _mockTransactionRepository = new Mock<ITransactionRepository>();
        _mockAccountValidation = new Mock<IAccountValidation>();
        _transactionService = new TransactionService(_mockTransactionRepository.Object, _mockAccountValidation.Object);
    }

    [Fact]
    public async Task FundTransferbyProc_WithValidRequest_ShouldReturnSuccessResult()
    {
        // Arrange
        var request = new FundTransferModel
        {
            srcAccount = "123456",
            destAccount = "654321",
            amount = 1000m,
            description1 = "Test Transfer",
            transCode = "TXN001",
            transDate = DateTime.Now,
            enteredBy = "TestUser"
        };

        var procResult = new TransactionProcStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890,
            Message = "Transaction Successful"
        };

        _mockTransactionRepository.Setup(x => x.TransactionByProc(It.IsAny<TransactionProcDTO>()))
            .ReturnsAsync(procResult);

        // Act
        var result = await _transactionService.FundTransferbyProc(request);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
        result.Message.Should().Be("Transaction Successful");
    }

    [Fact]
    public async Task FundTransferbyProc_WithFailedTransaction_ShouldThrowException()
    {
        // Arrange
        var request = new FundTransferModel
        {
            srcAccount = "123456",
            destAccount = "654321",
            amount = 1000m,
            description1 = "Test Transfer",
            transCode = "TXN001",
            transDate = DateTime.Now,
            enteredBy = "TestUser"
        };

        var procResult = new TransactionProcStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 0, // Failed transaction
            TransNoA = 0,
            Message = "Insufficient balance"
        };

        _mockTransactionRepository.Setup(x => x.TransactionByProc(It.IsAny<TransactionProcDTO>()))
            .ReturnsAsync(procResult);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientBalanceException>(() => 
            _transactionService.FundTransferbyProc(request));
    }

    [Fact]
    public async Task FundTransferbyProcWithBalance_ShouldReturnFundTransferedModel()
    {
        // Arrange
        var request = new FundTransferModel
        {
            srcAccount = "123456",
            destAccount = "654321",
            amount = 1000m,
            description1 = "Test Transfer",
            transCode = "TXN001",
            transDate = DateTime.Now,
            enteredBy = "TestUser"
        };

        var procResult = new TransactionProcStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890,
            Message = "Transaction Successful"
        };

        _mockTransactionRepository.Setup(x => x.TransactionByProc(It.IsAny<TransactionProcDTO>()))
            .ReturnsAsync(procResult);
        _mockAccountValidation.Setup(x => x.GeBalance(request.srcAccount))
            .ReturnsAsync(5000m);

        // Act
        var result = await _transactionService.FundTransferbyProcWithBalance(request);

        // Assert
        result.Should().NotBeNull();
        result.balance.Should().Be(5000m);
        result.transactionBalance.Should().Be(1000m);
        result.transactionIdentifier.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
    }

    [Fact]
    public async Task FundTransferStatus_WithJournalNo_ShouldReturnTransactionStatus()
    {
        // Arrange
        var request = new TranactionStatusInquiryModel
        {
            JournalNo = 12345,
            enteredBY = "TestUser"
        };

        var statusResult = new TransactionStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        _mockTransactionRepository.Setup(x => x.SearchTransactionByJournalNo(request.JournalNo))
            .ReturnsAsync(statusResult);

        // Act
        var result = await _transactionService.FundTransferStatus(request);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
    }

    [Fact]
    public async Task FundTransferStatus_WithBVRCNO_ShouldReturnTransactionStatus()
    {
        // Arrange
        var request = new TranactionStatusInquiryModel
        {
            BVRCNO = "TXN001",
            enteredBY = "TestUser"
        };

        var statusResult = new TransactionStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        _mockTransactionRepository.Setup(x => x.SearchTransactionByBVRCNO(request.BVRCNO))
            .ReturnsAsync(statusResult);

        // Act
        var result = await _transactionService.FundTransferStatus(request);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
    }

    [Fact]
    public async Task TransactionReversal_WithValidRequest_ShouldReturnReversalStatus()
    {
        // Arrange
        var request = new ReversalRequestModel
        {
            JournalNo = 12345,
            enteredBY = "TestUser",
            Description = "Reversal Test",
            Newbvrcno = "REV001"
        };

        var journalNos = new List<string> { "12345" };
        var reversalResult = new ReversalStatusDTO
        {
            BVRCNO = "REV001",
            Journalno = 54321,
            TransNoA = 98765,
            Message = "Reversal Successful"
        };

        _mockTransactionRepository.Setup(x => x.JournalnosBYJournalno(request.JournalNo, request.enteredBY))
            .ReturnsAsync(journalNos);
        _mockTransactionRepository.Setup(x => x.ReverseTransaction(It.IsAny<ReverseTansactionDTO>()))
            .ReturnsAsync(reversalResult);

        // Act
        var result = await _transactionService.TransactionReversal(request);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("REV001");
        result.Journalno.Should().Be(54321);
        result.TransNoA.Should().Be(98765);
        result.Message.Should().Be("Reversal Successful");
    }

    [Fact]
    public async Task TransactionReversal_WithNoTransactionFound_ShouldThrowException()
    {
        // Arrange
        var request = new ReversalRequestModel
        {
            JournalNo = 12345,
            enteredBY = "TestUser"
        };

        _mockTransactionRepository.Setup(x => x.JournalnosBYJournalno(request.JournalNo, request.enteredBY))
            .ReturnsAsync(new List<string>());

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => 
            _transactionService.TransactionReversal(request));
        exception.Message.Should().Contain("Transaction not found");
    }
}