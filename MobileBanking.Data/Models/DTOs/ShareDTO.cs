namespace MobileBanking.Data.Models.DTOs;
public class ShareDTO
{
    public required string MemberCode { get; set; }
    public string? OpenDate { get; set; }
    private decimal _balance;
    public decimal Balance
    {
        get => Math.Round(_balance, 2);
        set => _balance = Math.Round(value, 2);
    }
    public string? KittaNumber { get; set; }
}
