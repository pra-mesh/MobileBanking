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
    public async Task InsertTransactionAsync(TransactionDataDTO transactionData)
    {
        string query = @"insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values (@Journalno,@BVRCNO,@transDate,@branchid,@mano,@acno,@itemcode,@itemname,@itemlocation,@receivedpaidBy,@particulars,@dr_cr
            ,@Debit,@Credit,@description,@Remarks1,@Remarks2,@Remarks3,@Remarks4,@TransNoa,@EnteredBy,@EntryDate)";
        await _sqlDataAccess.SaveDataTransactionQuery<dynamic>(query, transactionData);
    }

    private static string GetTransNos()
    {
        return @"Insert Into Transone ([TRANSDATE],[DESCRIPTION],[TTID],[TransactionType],[PartyType],[PartyId],[PartyDocumentNO],[PartyReceiptno]
           ,[BillNO],[ReceiptNO],[EnteredBy],[EntryDate] ,[BranchId],[MemberNO],[MemberName],[PartyDocument]) 
            Values (@TransDate,@trDesc,@TTID,@TransactionType,@partyType,@PartyID,@PartyDocumentNO,@PartyReceiptno,@Billno,
            @ReceiptNo,@EnteredBy,@EntryDate,@BranchID,@MemberNO,@MemberName,@PartyDocument);
            SELECT CAST(SCOPE_IDENTITY() as int)";
    }

    public async Task<TransactionStatusDTO> SearchTransactionByJournalNo(int journalNO) =>
         await _sqlDataAccess.SingleDataQuery<TransactionStatusDTO, dynamic>
        ("select top 1 BVRCNO, journalno, TransNoA from Maintransbook where journalno=@journalno",
            new { journalNO });

    public async Task<TransactionStatusDTO> SearchTransactionByBVRCNO(string BVRCNO) =>
       await _sqlDataAccess.SingleDataQuery<TransactionStatusDTO, dynamic>
       ("select top 1 BVRCNO, journalno, TransNoA from Maintransbook where BVRCNO=@BVRCNO order by journalno",
           new { BVRCNO });

    public async Task<List<string>> JournalnosByBVRCNO(string BVRCNO, string enteredBy) =>
        await _sqlDataAccess.LoadDataQuery<string, dynamic>
        (@"Select Distinct Journalno from Maintransbook 
                    where BVRCNO =@BVRCNO and enteredBy=@enteredBy", new { BVRCNO, enteredBy });

    public async Task<List<string>> JournalnosBYJournalno(int Journalno, string enteredBy) =>
        await _sqlDataAccess.LoadDataQuery<string, dynamic>
        (@"Select Distinct Journalno from Maintransbook
                    where Journalno =@Journalno and enteredBy=@enteredBy", new { Journalno, enteredBy });

    public async Task<ReversalStatusDTO> ReverseTransaction(ReverseTansactionDTO reverseTansaction)
    {
        var p = new DynamicParameters(reverseTansaction);
        p.Add("@newJournalno", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@newTransno", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@Message", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

        await _sqlDataAccess.SaveData("sp_TransactionReversal", p);
        return new ReversalStatusDTO
        {
            BVRCNO = reverseTansaction.Newbvrcno ?? "",
            Message = p.Get<string>("@Message"),
            TransNoA = p.Get<int>("@newTransno"),
            Journalno = p.Get<int>("@newJournalno"),
        };

    }
    public async Task<TransactionProcStatusDTO> TransactionByProc(TransactionProcDTO dto)
    {
        var p = new DynamicParameters(dto);
        p.Add("@Journalno", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@Transno", dbType: DbType.Int32, direction: ParameterDirection.Output);
        p.Add("@Message", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
        await _sqlDataAccess.SaveData("sp_MobileTransaction", p);
        return new TransactionProcStatusDTO
        {
            BVRCNO = dto.TransCode ?? "",
            Message = p.Get<string>("@Message"),
            TransNoA = p.Get<int>("@Transno"),
            Journalno = p.Get<int>("@Journalno"),
        };
    }


}

