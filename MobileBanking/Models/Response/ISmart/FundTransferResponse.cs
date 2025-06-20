namespace MobileBanking.Models.Response.ISmart;

public class FundTransferResponse : BaseResponse
{
    public decimal balance { get; set; }
    public string transactionId { get; set; } = "";
}
