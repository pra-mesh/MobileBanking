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
            statementList = GetSatemenlist(fullStatement.statementList)
        };

    private static List<FullStatementList> GetSatemenlist(List<Statement>? statementList)
    {
        List<FullStatementList> st = new List<FullStatementList>();
        if (statementList is not null)
            foreach (Statement statement in statementList)
            {
                st.Add(new FullStatementList
                {
                    date = statement.date.Date.ToString(),
                    remarks = statement.remarks,
                    amount = statement.amount,
                    type = statement.type,
                    Balance = statement.Balance
                });
            }
        return st;
    }
    public static MiniStatementResponse ToMiniStatementResponse(MiniStatementModel miniStatement) =>
        new MiniStatementResponse
        {
            minimumBalance = miniStatement.minimumBalance,
            availableBalance = miniStatement.availableBalance,
            statementList = GetMiniStatementList(miniStatement.statementList)
        };

    private static List<MiniStatementList> GetMiniStatementList(List<MiniStatement>? statementList)
    {
        List<MiniStatementList> miniStatements = new();
        if (statementList is not null)
            foreach (var statement in statementList)
            {
                miniStatements.Add(new MiniStatementList
                {
                    date = statement.date.Date.ToString(),
                    remarks = statement.remarks,
                    amount = statement.amount,
                    type = statement.type,
                });

            }
        return miniStatements;
    }
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
}
