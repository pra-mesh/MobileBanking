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

    [Theory]
    [InlineData("Test Transaction 1", "01", "TestUser1", 12345)]
    [InlineData("Test Transaction 2", "02", "TestUser2", 12346)]
    [InlineData("Test Transaction 3", "00", "TestUser3", 12347)]
    public async Task GenerateJournalNoAsync_WithVariousJournals_ShouldReturnJournalNumber(
        string description, string branchId, string user, int expectedJournalNo)
    {
        // Arrange
        var journal = new JournalNoDTO
        {
            tdate = DateTime.Now,
            description = description,
            branchId = branchId,
            user = user
        };

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

    [Theory]
    [InlineData("TXN001", "TestUser1", 67890)]
    [InlineData("TXN002", "TestUser2", 67891)]
    [InlineData("TXN003", "TestUser3", 67892)]
    [InlineData("", "TestUser4", 0)]
    public async Task GetTransNoAsync_WithVariousTransactions_ShouldReturnTransactionNumber(
        string ttid, string enteredBy, int expectedTransNo)
    {
        // Arrange
        var transactionData = new TransactionDTO
        {
            TransDate = DateTime.Now,
            TrDesc = "Test Transaction",
            TTID = ttid,
            EnteredBy = enteredBy
        };

        _mockSqlDataAccess.Setup(x => x.SaveDataScalarTransaction<dynamic>(It.IsAny<string>(), transactionData))
            .ReturnsAsync(expectedTransNo);

        // Act
        var result = await _transactionRepository.GetTransNoAsync(transactionData);

        // Assert
        result.Should().Be(expectedTransNo);
        _mockSqlDataAccess.Verify(x => x.SaveDataScalarTransaction<dynamic>(It.IsAny<string>(), transactionData), Times.Once);
    }

    [Theory]
    [InlineData(12345, "TXN001", "DR", 1000, 0)]
    [InlineData(12346, "TXN002", "CR", 0, 1500)]
    [InlineData(12347, "TXN003", "DR", 500, 0)]
    public async Task InsertTransactionAsync_WithVariousTransactionData_ShouldInsertTransaction(
        int journalNo, string bvrcno, string drCr, decimal debit, decimal credit)
    {
        // Arrange
        var transactionData = new TransactionDataDTO
        {
            Journalno = journalNo,
            BVRCNO = bvrcno,
            transDate = DateTime.Now,
            branchid = "01",
            mano = "030",
            acno = "030.01",
            itemcode = "123456",
            itemname = "Test Account",
            dr_cr = drCr,
            Debit = debit,
            Credit = credit,
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

    [Theory]
    [InlineData(12345, "TXN001", 67890)]
    [InlineData(12346, "TXN002", 67891)]
    [InlineData(0, "", 0)] // Non-existent journal
    public async Task SearchTransactionByJournalNo_WithVariousJournalNumbers_ShouldReturnExpectedStatus(
        int journalNo, string expectedBvrcno, int expectedTransNoA)
    {
        // Arrange
        var expectedStatus = new TransactionStatusDTO
        {
            BVRCNO = expectedBvrcno,
            Journalno = journalNo,
            TransNoA = expectedTransNoA
        };

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            "select top 1 BVRCNO, journalno, TransNoA from Maintransbook where journalno=@journalno",
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _transactionRepository.SearchTransactionByJournalNo(journalNo);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be(expectedBvrcno);
        result.Journalno.Should().Be(journalNo);
        result.TransNoA.Should().Be(expectedTransNoA);

        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("journalNO")!.GetValue(p)!.Equals(journalNo))), Times.Once);
    }

    [Theory]
    [InlineData("TXN001", 12345, 67890)]
    [InlineData("TXN002", 12346, 67891)]
    [InlineData("NONEXISTENT", 0, 0)]
    public async Task SearchTransactionByBVRCNO_WithVariousBVRCNOs_ShouldReturnExpectedStatus(
        string bvrcno, int expectedJournalNo, int expectedTransNoA)
    {
        // Arrange
        var expectedStatus = new TransactionStatusDTO
        {
            BVRCNO = bvrcno,
            Journalno = expectedJournalNo,
            TransNoA = expectedTransNoA
        };

        _mockSqlDataAccess.Setup(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            "select top 1 BVRCNO, journalno, TransNoA from Maintransbook where BVRCNO=@BVRCNO order by journalno",
            It.IsAny<object>()))
            .ReturnsAsync(expectedStatus);

        // Act
        var result = await _transactionRepository.SearchTransactionByBVRCNO(bvrcno);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be(bvrcno);
        result.Journalno.Should().Be(expectedJournalNo);
        result.TransNoA.Should().Be(expectedTransNoA);

        _mockSqlDataAccess.Verify(x => x.SingleDataQuery<TransactionStatusDTO, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => p.GetType().GetProperty("BVRCNO")!.GetValue(p)!.Equals(bvrcno))), Times.Once);
    }

    [Theory]
    [InlineData("TXN001", "TestUser1", new[] { "12345", "12346" })]
    [InlineData("TXN002", "TestUser2", new[] { "12347" })]
    [InlineData("NONEXISTENT", "TestUser3", new string[0])]
    public async Task JournalnosByBVRCNO_WithVariousBVRCNOs_ShouldReturnExpectedJournalNumbers(
        string bvrcno, string enteredBy, string[] expectedJournalNos)
    {
        // Arrange
        var expectedJournalNosList = expectedJournalNos.ToList();

        _mockSqlDataAccess.Setup(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .ReturnsAsync(expectedJournalNosList);

        // Act
        var result = await _transactionRepository.JournalnosByBVRCNO(bvrcno, enteredBy);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedJournalNos.Length);
        
        foreach (var expectedJournalNo in expectedJournalNos)
        {
            result.Should().Contain(expectedJournalNo);
        }

        _mockSqlDataAccess.Verify(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => 
                p.GetType().GetProperty("BVRCNO")!.GetValue(p)!.Equals(bvrcno) &&
                p.GetType().GetProperty("enteredBy")!.GetValue(p)!.Equals(enteredBy))), Times.Once);
    }

    [Theory]
    [InlineData(12345, "TestUser1", new[] { "12345" })]
    [InlineData(12346, "TestUser2", new[] { "12346", "12347" })]
    [InlineData(99999, "TestUser3", new string[0])]
    public async Task JournalnosBYJournalno_WithVariousJournalNumbers_ShouldReturnExpectedJournalNumbers(
        int journalno, string enteredBy, string[] expectedJournalNos)
    {
        // Arrange
        var expectedJournalNosList = expectedJournalNos.ToList();

        _mockSqlDataAccess.Setup(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.IsAny<object>()))
            .ReturnsAsync(expectedJournalNosList);

        // Act
        var result = await _transactionRepository.JournalnosBYJournalno(journalno, enteredBy);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(expectedJournalNos.Length);
        
        foreach (var expectedJournalNo in expectedJournalNos)
        {
            result.Should().Contain(expectedJournalNo);
        }

        _mockSqlDataAccess.Verify(x => x.LoadDataQuery<string, dynamic>(
            It.IsAny<string>(),
            It.Is<object>(p => 
                p.GetType().GetProperty("Journalno")!.GetValue(p)!.Equals(journalno) &&
                p.GetType().GetProperty("enteredBy")!.GetValue(p)!.Equals(enteredBy))), Times.Once);
    }

    [Theory]
    [InlineData("TXN001", 12345, "TestUser1", "REV001", 54321, 98765, "Reversal Successful")]
    [InlineData("TXN002", 12346, "TestUser2", "REV002", 54322, 98766, "Reversal Completed")]
    [InlineData("TXN003", 0, "TestUser3", "REV003", 0, 0, "Reversal Failed")]
    public async Task ReverseTransaction_WithVariousRequests_ShouldReturnExpectedReversalStatus(
        string bvrcno, int journalNo, string enteredBy, string newBvrcno, 
        int expectedJournalNo, int expectedTransNoA, string expectedMessage)
    {
        // Arrange
        var reversalRequest = new ReverseTansactionDTO
        {
            BVRCNO = bvrcno,
            JournalNo = journalNo,
            enteredBY = enteredBy,
            Description = "Reversal Test",
            Newbvrcno = newBvrcno
        };

        _mockSqlDataAccess.Setup(x => x.SaveData("sp_TransactionReversal", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate the output parameters being set
                param.Add("@newJournalno", expectedJournalNo, DbType.Int32, ParameterDirection.Output);
                param.Add("@newTransno", expectedTransNoA, DbType.Int32, ParameterDirection.Output);
                param.Add("@Message", expectedMessage, DbType.String, size: 100, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.ReverseTransaction(reversalRequest);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be(newBvrcno);
        result.Journalno.Should().Be(expectedJournalNo);
        result.TransNoA.Should().Be(expectedTransNoA);
        result.Message.Should().Be(expectedMessage);

        _mockSqlDataAccess.Verify(x => x.SaveData("sp_TransactionReversal", It.IsAny<DynamicParameters>()), Times.Once);
    }

    [Theory]
    [InlineData("123456", "654321", "TXN001", 1000, 12345, 67890, "Transaction Successful")]
    [InlineData("111111", "222222", "TXN002", 500, 12346, 67891, "Transaction Completed")]
    [InlineData("333333", "444444", "TXN003", 2000, 0, 0, "Transaction Failed")]
    public async Task TransactionByProc_WithVariousRequests_ShouldReturnExpectedTransactionStatus(
        string srcAccount, string destAccount, string transCode, decimal amount,
        int expectedJournalNo, int expectedTransNoA, string expectedMessage)
    {
        // Arrange
        var transactionRequest = new TransactionProcDTO
        {
            SrcAccount = srcAccount,
            DestAccount = destAccount,
            Description1 = "Test Transfer",
            Description2 = "Mobile Banking",
            TransCode = transCode,
            TransDate = DateTime.Now,
            EnteredBy = "TestUser",
            Amount = amount
        };

        _mockSqlDataAccess.Setup(x => x.SaveData("sp_MobileTransaction", It.IsAny<DynamicParameters>()))
            .Callback<string, DynamicParameters>((proc, param) =>
            {
                // Simulate the output parameters being set
                param.Add("@Journalno", expectedJournalNo, DbType.Int32, ParameterDirection.Output);
                param.Add("@Transno", expectedTransNoA, DbType.Int32, ParameterDirection.Output);
                param.Add("@Message", expectedMessage, DbType.String, size: 100, ParameterDirection.Output);
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _transactionRepository.TransactionByProc(transactionRequest);

        // Assert
        result.Should().NotBeNull();
        result.BVRCNO.Should().Be(transCode);
        result.Journalno.Should().Be(expectedJournalNo);
        result.TransNoA.Should().Be(expectedTransNoA);
        result.Message.Should().Be(expectedMessage);

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
}