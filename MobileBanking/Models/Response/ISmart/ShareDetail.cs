namespace MobileBanking.Models.Response.ISmart;

public class ShareDetail
{
    public required string MemberID { get; set; }
    public string? OpenDate { get; set; }
    public decimal Balance { get; set; }
    public string? KittaNumber { get; set; }
}
