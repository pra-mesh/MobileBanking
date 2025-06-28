using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Application.Mappings;
internal static class DataToBusinessMapping
{
    public static AccountDetailModel ToAccountDetailModel(AccountDetailDTO accountDetail) =>
        new AccountDetailModel
        {
            SavingName = accountDetail.SavingName,
            mainbookno = accountDetail.mainbookno,
            acno = accountDetail.acno,
            AccountHolder = accountDetail.AccountHolder,
            accountno = accountDetail.accountno,
            MemName = accountDetail.MemName,
            Disabled = accountDetail.Disabled,
            InterestRate = accountDetail.InterestRate,
            MinBal = accountDetail.MinBal,
            Balance = accountDetail.Balance,
            Gamt = accountDetail.Gamt,
            Lamt = accountDetail.Lamt
        };
    public static List<Statement> ToStatementList(List<FullStatementDTO> statements)
    {
        List<Statement> mappedStatement = new();
        decimal balance = 0;
        foreach (var statement in statements)
        {
            balance += statement.Type == "Credit" ? statement.Amount : -statement.Amount;
            mappedStatement.Add(new Statement
            {
                date = statement.Date,
                remarks = statement.Description,
                amount = statement.Amount,
                type = statement.Type,
                Balance = balance
            });
        }
        return mappedStatement;

    }
    public static List<MiniStatement> ToMiniStatmentList(List<MiniStatementDTO> statements)
    {
        List<MiniStatement> miniStatements = new();
        foreach (var statement in statements)
        {
            miniStatements.Add(new MiniStatement
            {
                date = statement.Date,
                remarks = statement.Description,
                amount = statement.Amount,
                type = statement.Type,

            });
        }
        return miniStatements;
    }
    public static TransactionStatusModel ToFundTransferedModel(TransactionStatusDTO dto) =>
        new TransactionStatusModel { Journalno = dto.Journalno, BVRCNO = dto.BVRCNO, TransNoA = dto.TransNoA };
    public static ReversalStatusModel ToReversalStatusModel(ReversalStatusDTO dto) =>
        new ReversalStatusModel
        {
            BVRCNO = dto.BVRCNO,
            Journalno = dto.Journalno,
            TransNoA = dto.TransNoA,
            Message = dto.Message
        };
    public static TransactionStatusProcModel ToTransactionStatusProcModel(TransactionProcStatusDTO dto) =>
        new TransactionStatusProcModel
        {
            BVRCNO = dto.BVRCNO,
            Journalno = dto.Journalno,
            TransNoA = dto.TransNoA,
            Message = dto.Message
        };
    public static AccountDetailFullModel ToAccountDetailFullModel(AccountFullDetalDTO dto) =>
        new AccountDetailFullModel
        {
            MemberId = dto.MemberId,
            AccountHolderName = dto.MemberName,
            Address = dto.Address,
            MobileNumber = dto.MobileNumber,
            AccountNumber = dto.AccountNumber,
            BranchCode = dto.BranchCode,
            IsActive = dto.IsActive,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            AccountType = dto.AccountType,
            SavingType = dto.SavingType,
            LockedAmount = dto.LockedAmount,
            GuarantedAmt = dto.GuarantedAmt,
            LedgerBalance = dto.LedgerBalance,
            MinBal = dto.MinBal,
            AvailableBalance = dto.AvailableBalance,
            ExpireDate = dto.ExpireDate,
            EntranceDate = dto.EntranceDate,
            AccruedInterest = dto.AccruedInterest,
            InterestRate = dto.InterestRate,
            InterestStartDate = dto.InterestStartDate,
            IdType = dto.IdType,
            IdNumber = dto.IdNumber,
            IdIssuePlace = dto.IdIssuePlace,
            IssueDate = dto.IssueDate

        };

    public static AccountModel ToAccountModel(AccountDTO dto) =>
        new AccountModel
        {
            AccountHolderName = dto.MemberName,
            AccountNumber = dto.AccountNumber,
            BranchCode = dto.BranchCode
        };
    public static LoanInfoModel ToLoanInfoModel(LoanInfoDTO dto) =>
        new LoanInfoModel
        {
            LoanType = dto.LoanType,
            AccountNumber = dto.AccountNumber,
            MemberID = dto.Memberno,
            InterestRate = dto.InterestRate,
            IssuedOn = dto.IssuedOn,
            MaturesOn = dto.MaturesOn,
            NoOfKista = dto.NoOfKista,
            KistaPeriod = dto.KistaPeriod,
            InterestType = dto.InterestType,
            DisburseAmount = dto.DisburseAmount,
            Balance = dto.Balance,
            IntInstallments = dto.IntInstallments,
            PrincipalInstallments = dto.PrincipalInstallments,
        };
    public static LoanStatementModel ToLoanStatement(LoanStatementDTO dto) =>
        new LoanStatementModel
        {
            TranDate = dto.tranDate,
            InterestDate = dto.interestDate,
            Reference = dto.reference,
            IssueAmount = dto.IssueAmount,
            Payment = dto.Payment,
            Principal = dto.Principal,
            Interest = dto.Interest,
            Fine = dto.Fine,
            Discount = dto.Discount,
            Balance = dto.Balance
        };

    public static ShareModel ToShareModel(ShareDTO dto) =>
        new ShareModel
        {
            MemberID = dto.MemberCode,
            OpenDate = dto.OpenDate,
            Balance = dto.Balance,
            KittaNumber = dto.KittaNumber
        };
}
