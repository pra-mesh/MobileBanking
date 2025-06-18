using MobileBanking.Application.Contracts.Request.ISmart;
using MobileBanking.Application.Contracts.Response.ISmart;
using MobileBanking.Data.Repositories;

namespace MobileBanking.Application.Services;
public class StatementServices : IStatementServices
{
    private readonly IStatementRepository _statement;
    private readonly IAccountValidation _accountValidation;
    private readonly IAccountRepository _balanceInquiry;

    public StatementServices(IStatementRepository statement, IAccountValidation accountValidation, IAccountRepository balanceInquiry)
    {
        _statement = statement;
        _accountValidation = accountValidation;
        _balanceInquiry = balanceInquiry;
    }

    public async Task<MiniStatementResponse> MiniStatement(MiniStatementRequest req)
    {
        await _accountValidation.IsSingleAccount(req.accountNumber);
        var balance = await _balanceInquiry.GetAccountDetails(req.accountNumber);
        var accountbal = balance.First();
        var statements = await _statement.MiniStatement(req.accountNumber, req.count);
        List<MiniStatement> miniStatements = new();
        foreach (var statement in statements)
        {
            miniStatements.Add(new MiniStatement
            {
                date = statement.Date.Date.ToString(),
                remarks = statement.Description,
                amount = statement.Amount,
                type = statement.Type
            });
        }
        return new MiniStatementResponse
        {
            minimumBalance = accountbal.MinBal,
            availableBalance = accountbal.Balance,
            isoResponseCode = "00",
            statementList = miniStatements
        };
    }

    public async Task<FullStatementResponse> FullStatement(FullStatementRequest req)
    {
        await _accountValidation.IsSingleAccount(req.accountNumber);
        var balance = await _balanceInquiry.GetAccountDetails(req.accountNumber);
        var accountBal = balance.First();
        var statements = await _statement.FullStatement(req.accountNumber, req.fromDate, req.toDate);
        List<FullStatement> fullStatements = new();
        decimal depBal = 0;
        foreach (var statement in statements)
        {
            if (statement.Type == "Credit")
                depBal += statement.Amount;
            else
                depBal -= statement.Amount;
            fullStatements.Add(new FullStatement
            {
                date = statement.Date.Date.ToString(),
                remarks = statement.Description,
                amount = statement.Amount,
                type = statement.Type,
                Balance = depBal
            });
        }
        return new FullStatementResponse
        {
            minimumBalance = accountBal.MinBal,
            availableBalance = accountBal.Balance,
            isoResponseCode = "00",
            statementList = fullStatements
        };

    }


}
