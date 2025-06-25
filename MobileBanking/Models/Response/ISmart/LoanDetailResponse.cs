namespace MobileBanking.Models.Response.ISmart;

public class LoanDetailResponse : BaseResponse
{
    public required string LoanType { get; init; }
    public required string AccountNumber { get; init; }
    public decimal interestRate { get; init; }
    public string? issuedOn { get; init; }
    public string? maturityDate { get; init; }
    public string? duration { get; init; }
    public string? interestType { get; init; }
    public decimal disbursedAmount { get; init; }
    public decimal outstandingBalance { get; init; }
}
