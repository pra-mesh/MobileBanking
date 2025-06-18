using Dapper;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Services.Connection;
using System.Data;

namespace MobileBanking.Data.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public TransactionRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }
    public async Task<int> GenerateJournalNoAsync(JournalNoDTO journal)
    {
        var p = new DynamicParameters(journal);
        p.Add("@newjno", DbType.Int32, direction: ParameterDirection.Output);
        await _sqlDataAccess.SaveDataTransactionProcedure("sp_GetJournalno", p);
        return p.Get<int>("@newjno");

    }
    public async Task<int> GetTransNoAsync(TransactionDTO transactionData)
    {
        return await _sqlDataAccess.SaveDataScalarTransaction<dynamic>(GetTransNos(), transactionData);
    }
    public void InsertTransaction(TransactionDataDTO transactionData)
    {
        string query = @"insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values (@Journalno,@BVRCNO,@transDate,@branchid,@mano,@acno,@itemcode,@itemname,@itemlocation,@receivedpaidBy,@particulars,@dr_cr
            ,@Debit,@Credit,@description,@Remarks1,@Remarks2,@Remarks3,@Remarks4,@TransNoa,@EnteredBy,@EntryDate)";
        _sqlDataAccess.SaveDataTransactionQuery<dynamic>(query, transactionData);
    }

    private static string GetTransNos()
    {
        return @"Insert Into Transone ([TRANSDATE],[DESCRIPTION],[TTID],[TransactionType],[PartyType],[PartyId],[PartyDocumentNO],[PartyReceiptno]
           ,[BillNO],[ReceiptNO],[EnteredBy],[EntryDate] ,[BranchId],[MemberNO],[MemberName],[PartyDocument]) 
            Values (@TransDate,@trDesc,@TTID,@TransactionType,@partyType,@PartyID,@PartyDocumentNO,@PartyReceiptno,@Billno,
            @ReceiptNo,@EnteredBy,@EntryDate,@BranchID,@MemberNO,@MemberName,@PartyDocument);
            SELECT CAST(SCOPE_IDENTITY() as int)";
    }
}

