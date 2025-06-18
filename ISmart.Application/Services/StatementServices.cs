using MobileBanking.Application.Mappings;
using MobileBanking.Application.Models;
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

    public async Task<MiniStatementModel> MiniStatement(MiniStatementInquiryModel req)
    {
        await _accountValidation.IsSingleAccount(req.accountNumber);
        var balance = await _balanceInquiry.GetAccountDetails(req.accountNumber);
        var accountbal = balance.First();
        var statements = await _statement.MiniStatement(req.accountNumber, req.count);
        List<MiniStatement> miniStatements =
            DataToBusinessMapping.ToMiniStatmentList(statements);
        return new MiniStatementModel
        {
            minimumBalance = accountbal.MinBal,
            availableBalance = accountbal.Balance,
            statementList = miniStatements
        };
    }

    public async Task<FullStatementModel> FullStatement(FullStatmentInquiryModel req)
    {
        await _accountValidation.IsSingleAccount(req.accountNumber);
        var balance = await _balanceInquiry.GetAccountDetails(req.accountNumber);
        var accountBal = balance.First();
        var statements = await _statement.FullStatement(req.accountNumber, req.fromDate, req.toDate);
        List<Statement> fullStatements = DataToBusinessMapping.ToStatementList(statements);
        return new FullStatementModel
        {
            minimumBalance = accountBal.MinBal,
            availableBalance = accountBal.Balance,
            statementList = fullStatements
        };

    }


}
