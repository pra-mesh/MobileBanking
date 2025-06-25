namespace MobileBanking.Data.Models.DTOs;
public class AccountDTO
{
    public required string MemberName { get; init; }
    public required string AccountNumber { get; init; }
    public string? BranchCode { get; init; }
}
