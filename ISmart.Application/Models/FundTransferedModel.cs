namespace MobileBanking.Application.Models;
public class FundTransferedModel
{
    public required int journalno { get; init; }
    public decimal balance { get; init; }
    public decimal transactionBalance { get; init; }

    public required string transactionIdentifier { get; init; }
}
