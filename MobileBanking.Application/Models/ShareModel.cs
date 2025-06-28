namespace MobileBanking.Application.Models;
public class ShareModel
{
    public required string MemberID { get; set; }
    public string? OpenDate { get; set; }
    public decimal Balance { get; set; }
    public string? KittaNumber { get; set; }
}
