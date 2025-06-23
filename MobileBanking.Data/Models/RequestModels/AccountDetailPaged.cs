namespace MobileBanking.Data.Models.RequestModels;
public class AccountDetailPaged
{
    public string? MemberNo { get; set; }
    public string? AccountNumber { get; set; }
    public string? MobileNumber { get; set; }
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 1;
}
