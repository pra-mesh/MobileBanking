namespace MobileBanking.Application.Models;
public class FundTransferedModel : TransactionStatusModel
{
    public decimal balance { get; init; }
    public decimal transactionBalance { get; init; }
    public required string transactionIdentifier { get; init; }
}
