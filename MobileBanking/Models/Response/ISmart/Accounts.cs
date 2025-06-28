namespace MobileBanking.Models.Response.ISmart;

public class Accounts
{
    public required string MemberName { get; init; }
    public required string AccountNumber { get; init; }
    public string? BranchCode { get; init; }
}
