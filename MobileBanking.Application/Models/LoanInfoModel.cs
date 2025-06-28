namespace MobileBanking.Application.Models;
public class LoanInfoModel
{
    public required string LoanType { get; init; }
    public required string AccountNumber { get; init; }
    public required string MemberID { get; set; }
    public decimal InterestRate { get; init; }
    public string? IssuedOn { get; init; }
    public string? MaturesOn { get; init; }
    public int NoOfKista { get; init; }
    public string? KistaPeriod { get; init; }
    public string? InterestType { get; init; }
    public decimal DisburseAmount { get; init; }
    public decimal Balance { get; init; }
    public decimal IntInstallments { get; init; }
    public decimal PrincipalInstallments { get; init; }
}
