using MobileBanking.Application.Models;
using MobileBanking.Models.Response.ISmart;

namespace MobileBanking.Mappings.BusinessToAPIMapping;

public static class ISmartResponseMapping
{
    public static BalanceInquiryResponse ToBalanceInquiryResponse(AccountDetailModel account) =>
        new BalanceInquiryResponse { availableBalance = account.Balance, minimumBalance = account.MinBal };
    public static FullStatementResponse ToFullStatementResponse(FullStatementModel fullStatement) =>
        new FullStatementResponse
        {
            minimumBalance = fullStatement.minimumBalance,
            availableBalance = fullStatement.availableBalance,
            statementList = fullStatement.statementList?.Select(GetSatemenlist).ToList(),
        };

    private static FullStatementList GetSatemenlist(Statement statement) =>
    new FullStatementList
    {
        date = statement.date.Date.ToString(),
        remarks = statement.remarks,
        amount = statement.amount,
        type = statement.type,
        Balance = statement.Balance
    };

    public static MiniStatementResponse ToMiniStatementResponse(MiniStatementModel miniStatement) =>
        new MiniStatementResponse
        {
            minimumBalance = miniStatement.minimumBalance,
            availableBalance = miniStatement.availableBalance,
            statementList = miniStatement.statementList?.Select(GetMiniStatement).ToList()
        };

    private static MiniStatementList GetMiniStatement(MiniStatement statement) =>
    new MiniStatementList
    {
        date = statement.date.Date.ToString(),
        remarks = statement.remarks,
        amount = statement.amount,
        type = statement.type,
    };


    public static FundTransferResponse ToFundTransferResponse(FundTransferedModel model) =>
        new FundTransferResponse
        {
            transactionId = model.Journalno.ToString(),
            balance = model.balance,
            isoResponseCode = "00"
        };
    public static FundTransferStatusResponse ToFundTransferStatusResponse(TransactionStatusModel model)
        => new FundTransferStatusResponse
        {
            transactionId = model.Journalno.ToString(),
            isoResponseCode = model.Journalno == 0 ? "05" : "00"
        };

    public static FundTransferStatusResponse ToRevesalStatusResponse(ReversalStatusModel model) =>
        new FundTransferStatusResponse
        {
            transactionId = model.Journalno.ToString(),
            isoResponseCode = (model.Journalno == 0 || model.TransNoA == 0) ? "05" : "00"
        };
    public static AccountFullDetail ToAccountFullDetail(AccountDetailFullModel model) =>
        new AccountFullDetail
        {
            memberId = model.MemberId,
            MemberName = model.AccountHolderName,
            address = model.Address,
            mobileNumber = model.MobileNumber,
            AccountNumber = model.AccountNumber,
            BranchCode = model.BranchCode,
            isActive = model.IsActive,
            dateOfBirth = model.DateOfBirth,
            gender = model.Gender,
            accruedInterest = model.AccruedInterest,
            interestRate = model.InterestRate,
            accountType = model.AccountType,
            availableBalance = model.AvailableBalance,
            minimumBalance = model.MinBal,
            idType = model.IdType,
            idNumber = model.IdNumber,
            idIssuePlace = model.IdIssuePlace,
            issueDate = model.IssueDate
        };

    public static Accounts ToAccounts(AccountModel model) =>
        new Accounts
        {
            AccountNumber = model.AccountNumber,
            MemberName = model.AccountHolderName,
            BranchCode = model.BranchCode,
        };
    public static LoanDetailResponse ToLoanDetailResponse(LoanInfoModel model) =>
        new LoanDetailResponse
        {
            LoanType = model.LoanType,
            AccountNumber = model.AccountNumber,
            interestRate = model.InterestRate,
            issuedOn = model.IssuedOn,
            maturityDate = model.MaturesOn,
            duration = model.NoOfKista + " " + model.KistaPeriod,
            interestType = model.InterestType,
            disbursedAmount = model.DisburseAmount,
            outstandingBalance = model.Balance,
        };
    public static LoanFullStatement ToLoanFullStatement(LoanStatementModel model) =>
        new LoanFullStatement
        {
            TranDate = model.TranDate,
            InterestDate = model.InterestDate,
            Description = model.Reference,
            IssuedAmount = model.IssueAmount,
            Payment = model.Payment,
            Principal = model.Principal,
            Interest = model.Interest,
            Fine = model.Fine,
            Discount = model.Discount,
            Balance = model.Balance,
        };
    public static ShareDetail ToShareDetail(ShareModel shareModel) =>
        new ShareDetail
        {
            MemberID = shareModel.MemberID,
            OpenDate = shareModel.OpenDate,
            Balance = shareModel.Balance,
            KittaNumber = shareModel.KittaNumber,
        };

    public static DepositAcccouts ToDepositAcccouts(AccountDetailFullModel model) =>
      new DepositAcccouts
      {
          AccountNumber = model.AccountNumber,
          BranchCode = model.BranchCode,
          IsActive = model.IsActive,
          AccruedInterest = model.AccruedInterest,
          InterestRate = model.InterestRate,
          AccountType = model.AccountType,
          AvailableBalance = model.AvailableBalance,
          MinimumBalance = model.MinBal,
      };
}
