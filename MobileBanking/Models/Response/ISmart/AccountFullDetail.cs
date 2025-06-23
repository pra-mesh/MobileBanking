namespace MobileBanking.Models.Response.ISmart;

public class AccountFullDetail
{
    public required string memberId { get; init; }
    public required string memberName { get; init; }
    public required string address { get; init; }
    public required string mobileNumber { get; init; }
    public required string accountNumber { get; init; }
    public string? branchCode { get; init; }
    public bool isActive { get; init; }
    public string? dateOfBirth { get; init; }
    public string? gender { get; init; }
    public decimal accruedInterest { get; init; }
    public decimal interestRate { get; init; }
    public required string accountType { get; init; }
    public decimal availableBalance { get; init; }
    public decimal minimumBalance { get; init; }
    public string? idType { get; init; }
    public string? idNumber { get; init; }
    public string? idIssuePlace { get; init; }
    public string? issueDate { get; init; }
}
