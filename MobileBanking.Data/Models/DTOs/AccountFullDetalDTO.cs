namespace MobileBanking.Data.Models.DTOs;
public class AccountFullDetalDTO : AccountDTO
{
    public required string MemberId { get; init; }
    public required string Address { get; init; }
    public required string MobileNumber { get; init; }
    public bool IsActive { get; init; }
    public string? DateOfBirth { get; init; }
    public string? Gender { get; init; } = "Male";
    public required string AccountType { get; init; }
    public required string SavingType { get; init; } = "Normal";
    private decimal _lockedAmount;
    public decimal LockedAmount
    {
        get => Math.Round(_lockedAmount, 2);
        init => _lockedAmount = Math.Round(value, 2);
    }
    private decimal _guarantedAmt;
    public decimal GuarantedAmt
    {
        get => Math.Round(_guarantedAmt, 2);
        init => _guarantedAmt = Math.Round(value, 2);
    }
    private decimal _ledgerBalance;
    public decimal LedgerBalance
    {
        get => Math.Round(_ledgerBalance, 2);
        init => _ledgerBalance = Math.Round(value, 2);
    }

    private decimal _minBal;
    public decimal MinBal
    {
        get => Math.Round(_minBal, 2);
        init => _minBal = Math.Round(value, 2);
    }
    private decimal _availableBalance;
    public decimal AvailableBalance
    {
        get => Math.Round(_availableBalance, 2);
        init => _availableBalance = Math.Round(value, 2);
    }
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
