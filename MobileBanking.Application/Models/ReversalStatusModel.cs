namespace MobileBanking.Application.Models;
public class ReversalStatusModel : TransactionStatusModel
{
    public string? Message { get; set; } = "";
}
