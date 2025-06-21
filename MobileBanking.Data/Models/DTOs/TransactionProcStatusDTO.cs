namespace MobileBanking.Data.Models.DTOs;
public class TransactionProcStatusDTO : TransactionStatusDTO
{
    public required string Message { get; set; } = "";
}
