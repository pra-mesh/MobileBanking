using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface ITransactionRepository
{
    Task<int> GenerateJournalNoAsync(JournalNoDTO journal);
    Task<int> GetTransNoAsync(TransactionDTO transactionData);
    Task InsertTransactionAsync(TransactionDataDTO transactionData);
    Task SearchTransactionByJournalNo(string journalNO);
}
