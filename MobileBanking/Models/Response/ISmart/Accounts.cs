namespace MobileBanking.Models.Response.ISmart;

public class Accounts
{
    public required string memberName { get; init; }
    public required string accountNumber { get; init; }
    public string? branchCode { get; init; }
}
