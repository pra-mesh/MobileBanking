namespace MobileBanking.Data.Models.DTOs;
public class LoanStatementDTO
{
    public string? tranDate { get; set; }
    public string? interestDate { get; set; }
    public string? reference { get; set; }
    private decimal _issueAmount;
    public decimal IssueAmount
    {
        get => Math.Round(_issueAmount, 2);
        set => _issueAmount = Math.Round(value, 2);
    }

    private decimal _payment { get; set; }
    public decimal Payment
    {
        get => Math.Round(_payment, 2);
        set => _payment = Math.Round(value, 2);
    }
    private decimal _principal;
    public decimal Principal
    {
        get => Math.Round(_principal, 2);
        set => _principal = Math.Round(value, 2);
    }
    private decimal _interest;
    public decimal Interest
    {
        get => Math.Round(_interest, 2);
        set => _interest = Math.Round(value, 2);
    }
    private decimal _fine;
    public decimal Fine
    {
        get => Math.Round(_fine, 2);
        set => _fine = Math.Round(value, 2);
    }
    private decimal _discount;
    public decimal Discount
    {
        get => Math.Round(_discount, 2);
        set => _discount = Math.Round(value, 2);
    }
    private decimal _balance;
    public decimal Balance
    {
        get => Math.Round(_balance, 2);
        set => _balance = Math.Round(value, 2);
    }
}