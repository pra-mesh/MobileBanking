namespace MobileBanking.Data.Models.DTOs;
public class MiniStatementDTO
{
    public int JournalNo { get; set; }
    public DateTime Date { get; set; }
    public string Miti { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Amount { get; set; }
    public string Type { get; set; } = "Debit";

}
public sealed class FullStatementDTO : MiniStatementDTO
{
    public decimal Balance { get; set; }
}