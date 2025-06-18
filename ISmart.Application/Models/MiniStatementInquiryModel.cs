namespace MobileBanking.Application.Models;
public class MiniStatementInquiryModel
{
    public string branchId { get; init; } = "00";
    public string accountNumber { get; init; } = "";
    public int count { get; init; } = 0;
}
