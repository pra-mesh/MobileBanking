namespace MobileBanking.Application.Models;
public class FundTransferModel
{
    public string? srcBranchId { get; set; }
    public string srcAccount { get; init; } = "";
    public string? srcAccountType { get; init; }
    public string? destBranchId { get; set; }
    public string destAccount { get; init; } = "";
    public string? destAccountType { get; init; }
    public string description1 { get; init; } = "";
    public string? description2 { get; init; } = "";
    public string? description3 { get; init; }
    public string? transCode { get; init; }
    public DateTime transDate { get; init; }
    public decimal amount { get; init; } = 0;
    public string enteredBy { get; init; } = "mBank";
}
