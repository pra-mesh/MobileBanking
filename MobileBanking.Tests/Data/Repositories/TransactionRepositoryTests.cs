using Dapper;
using FluentAssertions;
using Moq;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Repositories;
using MobileBanking.Data.Services.Connection;
using System.Data;
using Xunit;

namespace MobileBanking.Tests.Data.Repositories;

public class TransactionRepositoryTests
{
    private readonly Mock<ISqlDataAccess> _mockSqlDataAccess;
    private readonly TransactionRepository _transactionRepository;

    public TransactionRepositoryTests()
    {
        _mockSqlDataAccess = new Mock<ISqlDataAccess>();
        _transactionRepository = new TransactionRepository(_mockSqlDataAccess.Object);
    }

    [Fact]
    public async Task GenerateJournalNoAsync_WithValidJournal_ShouldReturnJournalNumber()
    {
        // Arrange
        var journal = new JournalNoDTO
        {
            tdate = DateTime.Now,
            description = "Test Transaction",
            branchId = "01",
            user = "TestUser"
        };

        var expectedJournalNo = 12345;

        _mockSqlDataAccess.Setup(x => x.SaveDataTransactionProcedure("sp_GetJournalno", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate the output parameter being set
                param.Add("@newjno", expectedJournalNo, DbType.Int32, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.GenerateJournalNoAsync(journal);

        // Assert
        result.Should().Be(expectedJournalNo);
        _mockSqlDataAccess.Verify(x => x.SaveDataTransactionProcedure("sp_GetJournalno", It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public async Task GetTransNoAsync_WithValidTransaction_ShouldReturnTransactionNumber()
    {
        // Arrange
        var transactionData = new TransactionDTO
        {
            TransDate = DateTime.Now,
            TrDesc = "Test Transaction",
            TTID = "TXN001",
            EnteredBy = "TestUser"
        };

        var expectedTransNo = 67890;

        _mockSqlDataAccess.Setup(x => x.SaveDataScalarTransaction<dynamic>(It.IsAny<string>(), transactionData))
            .ReturnsAsync(expectedTransNo);

        // Act
        var result = await _transactionRepository.GetTransNoAsync(transactionData);

        // Assert
        result.Should().Be(expectedTransNo);
        _mockSqlDataAccess.Verify(x => x.SaveDataScalarTransaction<dynamic>(It.IsAny<string>(), transactionData), Times.Once);
    }

    [Fact]
    public async Task InsertTransactionAsync_WithValidData_ShouldInsertTransaction()
    {
        // Arrange
        var transactionData = new TransactionDataDTO
        {
            Journalno = 12345,
            BVRCNO = "TXN001",
            transDate = DateTime.Now,
            branchid = "01",
            mano = "030",
            acno = "030.01",
            itemcode = "123456",
            itemname = "Test Account",
            dr_cr = "DR",
            Debit = 1000m,
            Credit = 0m,
            EnteredBy = "TestUser"
        };

        _mockSqlDataAccess.Setup(x => x.SaveDataTransactionQuery<dynamic>(It.IsAny<string>(), transactionData))
            .Returns(Task.CompletedTask);

        // Act
        await _transactionRepository.InsertTransactionAsync(transactionData);

        // Assert
        _mockSqlDataAccess.Verify(x => x.SaveDataTransactionQuery<dynamic>(
            It.Is<string>(sql => sql.Contains("insert into maintransbook")), 
            transactionData), Times.Once);
    }

    [Fact]
    public async Task SearchTransactionByJournalNo_WithValidJournalNo_ShouldReturnTransactionStatus()
    {
        // Arrange
        var journalNo = 12345;
        var expectedStatus = new TransactionStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            "select top 1 BVRCNO, journalno, TransNoA from Maintransbook where journalno=@journalno",
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _transactionRepository.SearchTransactionByJournalNo(journalNo);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);

        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("journalNO")!.GetValue(p)!.Equals(journalNo))), Times.Once);
    }

    [Fact]
    public async Task SearchTransactionByBVRCNO_WithValidBVRCNO_ShouldReturnTransactionStatus()
    {
        // Arrange
        var bvrcno = "TXN001";
        var expectedStatus = new TransactionStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890
        };

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            "select top 1 BVRCNO, journalno, TransNoA from Maintransbook where BVRCNO=@BVRCNO order by journalno",
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _transactionRepository.SearchTransactionByBVRCNO(bvrcno);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);

        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("BVRCNO")!.GetValue(p)!.Equals(bvrcno))), Times.Once);
    }

    [Fact]
    public async Task JournalnosByBVRCNO_WithValidBVRCNO_ShouldReturnJournalNumbers()
    {
        // Arrange
        var bvrcno = "TXN001";
        var enteredBy = "TestUser";
        var expectedJournalNos = new List<string> { "12345", "12346" };

        _mockSqlDataAccess.Setup(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .ReturnsAsync(expectedJournalNos);

        // Act
        var result = await _transactionRepository.JournalnosByBVRCNO(bvrcno, enteredBy);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().Contain("12345");
        result.Should().Contain("12346");

        _mockSqlDataAccess.Verify(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => 
                p.GetType().GetProperty("BVRCNO")!.GetValue(p)!.Equals(bvrcno) &&
                p.GetType().GetProperty("enteredBy")!.GetValue(p)!.Equals(enteredBy))), Times.Once);
    }

    [Fact]
    public async Task JournalnosBYJournalno_WithValidJournalNo_ShouldReturnJournalNumbers()
    {
        // Arrange
        var journalno = 12345;
        var enteredBy = "TestUser";
        var expectedJournalNos = new List<string> { "12345" };

        _mockSqlDataAccess.Setup(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .ReturnsAsync(expectedJournalNos);

        // Act
        var result = await _transactionRepository.JournalnosBYJournalno(journalno, enteredBy);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.Should().Contain("12345");

        _mockSqlDataAccess.Verify(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => 
                p.GetType().GetProperty("Journalno")!.GetValue(p)!.Equals(journalno) &&
                p.GetType().GetProperty("enteredBy")!.GetValue(p)!.Equals(enteredBy))), Times.Once);
    }

    [Fact]
    public async Task ReverseTransaction_WithValidRequest_ShouldReturnReversalStatus()
    {
        // Arrange
        var reversalRequest = new ReverseTansactionDTO
        {
            BVRCNO = "TXN001",
            JournalNo = 12345,
            enteredBY = "TestUser",
            Description = "Reversal Test",
            Newbvrcno = "REV001"
        };

        var expectedResult = new ReversalStatusDTO
        {
            BVRCNO = "REV001",
            Journalno = 54321,
            TransNoA = 98765,
            Message = "Reversal Successful"
        };

        _mockSqlDataAccess.Setup(x => x.SaveData("sp_TransactionReversal", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate the output parameters being set
                param.Add("@newJournalno", 54321, DbType.Int32, ParameterDirection.Output);
                param.Add("@newTransno", 98765, DbType.Int32, ParameterDirection.Output);
                param.Add("@Message", "Reversal Successful", DbType.String, size: 100, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.ReverseTransaction(reversalRequest);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("REV001");
        result.Journalno.Should().Be(54321);
        result.TransNoA.Should().Be(98765);
        result.Message.Should().Be("Reversal Successful");

        _mockSqlDataAccess.Verify(x => x.SaveData("sp_TransactionReversal", It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public async Task TransactionByProc_WithValidRequest_ShouldReturnTransactionStatus()
    {
        // Arrange
        var transactionRequest = new TransactionProcDTO
        {
            SrcAccount = "123456",
            DestAccount = "654321",
            Description1 = "Test Transfer",
            Description2 = "Mobile Banking",
            TransCode = "TXN001",
            TransDate = DateTime.Now,
            EnteredBy = "TestUser",
            Amount = 1000m
        };

        var expectedResult = new TransactionProcStatusDTO
        {
            BVRCNO = "TXN001",
            Journalno = 12345,
            TransNoA = 67890,
            Message = "Transaction Successful"
        };

        _mockSqlDataAccess.Setup(x => x.SaveData("sp_MobileTransaction", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate the output parameters being set
                param.Add("@Journalno", 12345, DbType.Int32, ParameterDirection.Output);
                param.Add("@Transno", 67890, DbType.Int32, ParameterDirection.Output);
                param.Add("@Message", "Transaction Successful", DbType.String, size: 100, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.TransactionByProc(transactionRequest);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be("TXN001");
        result.Journalno.Should().Be(12345);
        result.TransNoA.Should().Be(67890);
        result.Message.Should().Be("Transaction Successful");

        _mockSqlDataAccess.Verify(x => x.SaveData("sp_MobileTransaction", It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Fact]
    public async Task GenerateJournalNoAsync_WithZeroOutput_ShouldReturnZero()
    {
        // Arrange
        var journal = new JournalNoDTO
        {
            tdate = DateTime.Now,
            description = "Test Transaction",
            branchId = "01",
            user = "TestUser"
        };

        _mockSqlDataAccess.Setup(x => x.SaveDataTransactionProcedure("sp_GetJournalno", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate zero output (failure case)
                param.Add("@newjno", 0, DbType.Int32, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.GenerateJournalNoAsync(journal);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task GetTransNoAsync_WithFailure_ShouldReturnZero()
    {
        // Arrange
        var transactionData = new TransactionDTO
        {
            TransDate = DateTime.Now,
            TrDesc = "Test Transaction",
            TTID = "TXN001",
            EnteredBy = "TestUser"
        };

        _mockSqlDataAccess.Setup(x => x.SaveDataScalarTransaction<dynamic>(It.IsAny<string>(), transactionData))
            .ReturnsAsync(0);

        // Act
        var result = await _transactionRepository.GetTransNoAsync(transactionData);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task JournalnosByBVRCNO_WithNoResults_ShouldReturnEmptyList()
    {
        // Arrange
        var bvrcno = "NONEXISTENT";
        var enteredBy = "TestUser";
        var emptyList = new List<string>();

        _mockSqlDataAccess.Setup(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .ReturnsAsync(emptyList);

        // Act
        var result = await _transactionRepository.JournalnosByBVRCNO(bvrcno, enteredBy);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task TransactionByProc_WithFailure_ShouldReturnFailureStatus()
    {
        // Arrange
        var transactionRequest = new TransactionProcDTO
        {
            SrcAccount = "123456",
            DestAccount = "654321",
            Description1 = "Test Transfer",
            TransCode = "TXN001",
            EnteredBy = "TestUser",
            Amount = 1000m
        };

        _mockSqlDataAccess.Setup(x => x.SaveData("sp_MobileTransaction", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate failure
                param.Add("@Journalno", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@Transno", 0, DbType.Int32, ParameterDirection.Output);
                param.Add("@Message", "Transaction Failed", DbType.String, size: 100, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.TransactionByProc(transactionRequest);

        // Assert
        result.Should().NotBeNull();
        result.Journalno.Should().Be(0);
        result.TransNoA.Should().Be(0);
        result.Message.Should().Be("Transaction Failed");
    }
}