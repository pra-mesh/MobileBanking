namespace MobileBanking.Data.Models.DTOs;
public class LoanInfoDTO
{
    public required string LoanType { get; set; }
    public required string AccountNumber { get; set; }
    public required string Memberno { get; set; }
    public decimal InterestRate { get; set; }
    public string? IssuedOn { get; set; }
    public string? MaturesOn { get; set; }
    public int NoOfKista { get; set; }
    public string? KistaPeriod { get; set; }
    public string? InterestType { get; set; }
    private decimal _disburseAmount;

    public decimal DisburseAmount
    {
        get => Math.Round(_disburseAmount, 2);
        set => _disburseAmount = Math.Round(value, 2);
    }
    private decimal _balance;
    public decimal Balance
    {
        get => Math.Round(_balance, 2);
        set => _balance = Math.Round(value, 2);
    }
    private decimal _intInstallments;
    public decimal IntInstallments
    {
        get => Math.Round(_intInstallments, 2);
        set => _intInstallments = Math.Round(value, 2);
    }
    private decimal _principalInstallments;
    public decimal PrincipalInstallments
    {
        get => Math.Round(_principalInstallments, 2);
        set => _principalInstallments = Math.Round(value, 2);
    }

}
