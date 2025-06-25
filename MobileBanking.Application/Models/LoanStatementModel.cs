namespace MobileBanking.Application.Models;
public class LoanStatementModel
{
    public string? TranDate { get; set; }
    public string? InterestDate { get; set; }
    public string? Reference { get; set; }
    public decimal IssueAmount { get; set; }
    public decimal Payment { get; set; }
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal Fine { get; set; }
    public decimal Discount { get; set; }
    public decimal Balance { get; set; }
}