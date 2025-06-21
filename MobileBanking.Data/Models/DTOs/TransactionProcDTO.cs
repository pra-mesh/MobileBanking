namespace MobileBanking.Data.Models.DTOs;
public class TransactionProcDTO
{
    public required string SrcAccount { get; init; }
    public required string DestAccount { get; init; }
    public required string Description1 { get; init; }

    public string? Description2 { get; init; } = "Mobile Banking Transaction";
    public string? Description3 { get; init; } = "";
    public string? TransCode { get; init; } = "";

    public DateTime TransDate { get; init; } = DateTime.Now;
    public required string EnteredBy { get; init; }
    public decimal Amount { get; init; }
}
