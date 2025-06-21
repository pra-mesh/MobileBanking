using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface ITransactionRepository
{
    Task<int> GenerateJournalNoAsync(JournalNoDTO journal);
    Task<int> GetTransNoAsync(TransactionDTO transactionData);
    Task InsertTransactionAsync(TransactionDataDTO transactionData);
    Task<List<string>> JournalnosByBVRCNO(string BVRCNO, string enteredBy);
    Task<List<string>> JournalnosBYJournalno(int Journalno, string enteredBy);
    Task<ReversalStatusDTO> ReverseTransaction(ReverseTansactionDTO reverseTansaction);
    Task<TransactionStatusDTO> SearchTransactionByBVRCNO(string BVRCNO);
    Task<TransactionStatusDTO> SearchTransactionByJournalNo(int journalNO);
    Task<TransactionProcStatusDTO> TransactionByProc(TransactionProcDTO dto);
}
