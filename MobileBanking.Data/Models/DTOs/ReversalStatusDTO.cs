namespace MobileBanking.Data.Models.DTOs;
public class ReversalStatusDTO : TransactionStatusDTO
{
    public string? Message { get; set; } = "";
}
