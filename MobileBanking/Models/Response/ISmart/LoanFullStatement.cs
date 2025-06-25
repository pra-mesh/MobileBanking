namespace MobileBanking.Models.Response.ISmart;

public class LoanFullStatement
{
    public string? TranDate { get; init; } = string.Empty;
    public string? InterestDate { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public decimal IssuedAmount { get; init; }
    public decimal Payment { get; init; }
    public decimal Principal { get; init; }
    public decimal Interest { get; init; }
    public decimal Fine { get; init; }
    public decimal Discount { get; init; }
    public decimal Balance { get; init; }
}
