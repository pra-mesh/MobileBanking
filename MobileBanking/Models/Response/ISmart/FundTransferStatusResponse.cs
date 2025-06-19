namespace MobileBanking.Models.Response.ISmart;

public class FundTransferStatusResponse : BaseResponse
{
    public required string transactionId { get; set; }
}
