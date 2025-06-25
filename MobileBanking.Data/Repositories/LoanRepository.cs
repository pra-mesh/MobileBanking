using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data.Repositories;
public class LoanRepository : ILoanRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public LoanRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<LoanInfoDTO> LoanInfo(string accountNumber) =>
        await _sqlDataAccess.SingleDataQuery<LoanInfoDTO, dynamic>(
            @"select [Loan Type] as LoanType,REPLACE(LoanTypeNo,'.','')+loanno as accountNumber, 
                        case when InterestRate2 is null or InterestRate2=0 then interestrate else InterestRate2 
                        end as interestRate,dbo.engToNep(ISNULL([Starting Date],GetDate())) as issuedOn,
                        dbo.engToNep(ISNULL([FinalDate],GETDATE())) as maturesOn,
                        isNull(TotalNoOfKista,0) as NoOfKista,case when isNUll(Kistaperiod,'Month')<>'Daily'
                        then replace(isNUll(Kistaperiod,'Month'),'ly','')else 'day' end as Kistaperiod,
                        case when intCalcMethod=0 then 'Diminising' else 'Flat' end as interestType,
                        isnull([Approved Loan],0) as disburseAmount,[Current Balance] as balance,
                        0 as intInstallments, KistaRate as principalInstallments from LoanStatementAll2 
                        where REPLACE(LoanTypeNo,'.','')+loanno =@accountNumber", new { accountNumber });

    public async Task<List<LoanStatementDTO>> LoanStatements(string accountNumber, DateTime fromDate, DateTime toDate) =>
        await _sqlDataAccess.LoadData<LoanStatementDTO, dynamic>("sp_mLoanStatement",
            new { accountNumber, fromDate, toDate });
}
