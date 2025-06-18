namespace MobileBanking.Data.Models.DTOs;
public class AccountDetailDTO
{
    public required string SavingName { get; set; }
    public required string mainbookno { get; set; }
    public required string acno { get; set; }
    public required string AccountHolder { get; set; }
    public required string accountno { get; set; }
    public required string MemName { get; set; }
    public bool Disabled { get; set; }

    private decimal interestrate;
    public decimal InterestRate
    {
        get => Math.Round(interestrate, 2);
        set => interestrate = Math.Round(value, 2);

    }

    private decimal minBal;
    public decimal MinBal
    {
        get => Math.Round(minBal, 2);
        set => minBal = Math.Round(value, 2);
    }


    private decimal balance;
    public decimal Balance
    {
        get => Math.Round(balance, 2);
        set => balance = Math.Round(value, 2);
    }

    private decimal gamt;

    public decimal Gamt { get => Math.Round(gamt, 2); set => gamt = Math.Round(value, 2); }

    private decimal lamt;
    public decimal Lamt { get => Math.Round(lamt, 2); set => lamt = Math.Round(value, 2); }

}
