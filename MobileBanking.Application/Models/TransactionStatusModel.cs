namespace MobileBanking.Application.Models;
public class TransactionStatusModel
{
    public required string BVRCNO { get; init; }
    public required int Journalno { get; init; }
    public required int TransNoA { get; init; }
}
