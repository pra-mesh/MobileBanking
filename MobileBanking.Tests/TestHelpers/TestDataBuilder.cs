using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Tests.TestHelpers;

public static class TestDataBuilder
{
    public static AccountDetailDTO CreateAccountDetailDTO(
        string accountNumber = "123456",
        decimal balance = 1000m,
        decimal minBalance = 100m,
        string accountHolder = "John Doe")
    {
        return new AccountDetailDTO
        {
            SavingName = "Savings Account",
            mainbookno = "030",
            acno = "030.01",
            AccountHolder = accountHolder,
            accountno = accountNumber,
            MemName = accountHolder,
            Disabled = false,
            InterestRate = 5.5m,
            MinBal = minBalance,
            Balance = balance,
            Gamt = 0m,
            Lamt = 0m
        };
    }

    public static FundTransferModel CreateFundTransferModel(
        string srcAccount = "123456",
        string destAccount = "654321",
        decimal amount = 1000m,
        string transCode = "TXN001")
    {
        return new FundTransferModel
        {
            srcAccount = srcAccount,
            destAccount = destAccount,
            amount = amount,
            description1 = "Test Transfer",
            description2 = "Mobile Banking Transaction",
            transCode = transCode,
            transDate = DateTime.Now,
            enteredBy = "TestUser"
        };
    }

    public static List<MiniStatementDTO> CreateMiniStatementDTOs(int count = 3)
    {
        var statements = new List<MiniStatementDTO>();
        for (int i = 0; i < count; i++)
        {
            statements.Add(new MiniStatementDTO
            {
                Date = DateTime.Now.AddDays(-i - 1),
                Description = $"Transaction {i + 1}",
                Amount = (i + 1) * 100m,
                Type = i % 2 == 0 ? "Credit" : "Debit"
            });
        }
        return statements;
    }

    public static List<FullStatementDTO> CreateFullStatementDTOs(int count = 3)
    {
        var statements = new List<FullStatementDTO>();
        for (int i = 0; i < count; i++)
        {
            statements.Add(new FullStatementDTO
            {
                Date = DateTime.Now.AddDays(-i - 1),
                Description = $"Transaction {i + 1}",
                Amount = (i + 1) * 100m,
                Type = i % 2 == 0 ? "Credit" : "Debit"
            });
        }
        return statements;
    }

    public static AccountFullDetalDTO CreateAccountFullDetailDTO(
        string memberId = "M001",
        string memberName = "John Doe",
        string accountNumber = "123456")
    {
        return new AccountFullDetalDTO
        {
            MemberId = memberId,
            MemberName = memberName,
            Address = "Test Address",
            MobileNumber = "9841234567",
            AccountNumber = accountNumber,
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
    }

    public static TransactionStatusDTO CreateTransactionStatusDTO(
        string bvrcno = "TXN001",
        int journalno = 12345,
        int transNoA = 67890)
    {
        return new TransactionStatusDTO
        {
            BVRCNO = bvrcno,
            Journalno = journalno,
            TransNoA = transNoA
        };
    }

    public static TransactionProcStatusDTO CreateTransactionProcStatusDTO(
        string bvrcno = "TXN001",
        int journalno = 12345,
        int transNoA = 67890,
        string message = "Transaction Successful")
    {
        return new TransactionProcStatusDTO
        {
            BVRCNO = bvrcno,
            Journalno = journalno,
            TransNoA = transNoA,
            Message = message
        };
    }
}