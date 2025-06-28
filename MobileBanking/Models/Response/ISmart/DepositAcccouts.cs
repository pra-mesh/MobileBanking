namespace MobileBanking.Models.Response.ISmart;

public class DepositAcccouts
{
    public required string AccountNumber { get; init; }
    public string? BranchCode { get; init; }
    public decimal AccruedInterest { get; init; }
    public decimal InterestRate { get; init; }
    public required string AccountType { get; init; }
    public decimal AvailableBalance { get; init; }
    public decimal MinimumBalance { get; init; }
    public bool IsActive { get; init; }
}
