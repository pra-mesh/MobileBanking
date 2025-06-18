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
}
