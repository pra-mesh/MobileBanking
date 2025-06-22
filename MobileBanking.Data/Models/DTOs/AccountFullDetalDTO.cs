namespace MobileBanking.Data.Models.DTOs;
public class AccountFullDetalDTO
{
    public required string memberId { get; set; }
    public required string memberName { get; set; }
    public required string address { get; set; }
    public required string mobileNumber { get; set; }
    public required string accountNumber { get; set; }
    public string? branchCode { get; set; }
    public bool isActive { get; set; }
    public string? dateOfBirth { get; set; }
    public string? gender { get; set; } = "Male";
    private decimal _accruedInterest;
    public decimal AccruedInterset
    {
        get => Math.Round(_accruedInterest, 2);
        init => _accruedInterest = Math.Round(_accruedInterest, 2);
    }
    private decimal _interestRate;
    public decimal InterestRate
    {
        get => Math.Round(_interestRate, 2);
        init => _interestRate = Math.Round(_interestRate, 2);
    }
    public required string accountType { get; set; }
    private decimal _availableBalance;
    public decimal AvailableBalance
    {
        get => Math.Round(_availableBalance, 2);
        init => _availableBalance = Math.Round(_availableBalance, 2);
    }
    private decimal _minimumBalance;
    public decimal MinimumBalance
    {
        get => Math.Round(_minimumBalance, 2);
        init => _minimumBalance = Math.Round(_minimumBalance, 2);
    }
    public string? idType { get; set; }
    public string? idNumber { get; set; }
    public string? idIssuePlace { get; set; }
    public string? issueDate { get; set; }
}
