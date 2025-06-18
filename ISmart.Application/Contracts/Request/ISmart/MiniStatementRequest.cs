namespace MobileBanking.Application.Contracts.Request.ISmart;
public class MiniStatementRequest
{
    public string branchId { get; set; } = "00";
    public string accountNumber { get; set; } = "";
    public int count { get; set; } = 0;
}
