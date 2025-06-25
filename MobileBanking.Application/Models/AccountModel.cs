namespace MobileBanking.Application.Models;
public class AccountModel
{
    public required string AccountHolderName { get; set; }
    public required string AccountNumber { get; set; }
    public string? BranchCode { get; set; }
}
