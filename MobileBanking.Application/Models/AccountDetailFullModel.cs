namespace MobileBanking.Application.Models;
public class AccountDetailFullModel
{
    public required string MemberId { get; init; }
    public required string MemberName { get; init; }
    public required string Address { get; init; }
    public required string MobileNumber { get; init; }
    public required string AccountNumber { get; init; }
    public string? BranchCode { get; init; }
    public bool IsActive { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; } = "Male";
    public required string AccountType { get; init; }
    public required string SavingType { get; init; } = "Normal";
    public decimal LockedAmount { get; init; }
    public decimal GuarantedAmt { get; init; }
    public decimal LedgerBalance { get; init; }
    public decimal MinBal { get; init; }
    public decimal AvailableBalance { get; init; }
    public DateTime? ExpireDate { get; init; }
    public DateTime? EntranceDate { get; init; }
    public decimal AccruedInterest { get; init; }
    public decimal InterestRate { get; init; }
    public DateTime? InterestStartDate { get; init; }
    public string? IdType { get; init; } = "";
    public string? IdNumber { get; init; } = "";
    public string? IdIssuePlace { get; init; } = "";
    public string? IssueDate { get; init; } = "";
}
