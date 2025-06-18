namespace MobileBanking.Application.Models;
public class AccountDetailModel
{
    public required string SavingName { get; init; }
    public required string mainbookno { get; init; }
    public required string acno { get; init; }
    public required string AccountHolder { get; init; }
    public required string accountno { get; init; }
    public required string MemName { get; init; }
    public bool Disabled { get; init; }
    public decimal InterestRate { get; init; }
    public decimal MinBal { get; init; }
    public decimal Balance { get; init; }
    public decimal Gamt { get; init; }
    public decimal Lamt { get; init; }
}
