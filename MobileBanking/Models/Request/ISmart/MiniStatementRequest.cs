namespace MobileBanking.Models.Request.ISmart;
public class MiniStatementRequest
{
    public string branchId { get; init; } = "00";
    public string accountNumber { get; init; } = "";
    public int count { get; init; } = 0;
}
