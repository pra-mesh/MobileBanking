namespace MobileBanking.Data.Models.DTOs;
public class JournalNoDTO
{
    public DateTime tdate { get; set; } = DateTime.Now;
    public string description { get; set; } = "";
    public string? branchId { get; set; } = "";
    public string user { get; set; } = "iSmart";
}
