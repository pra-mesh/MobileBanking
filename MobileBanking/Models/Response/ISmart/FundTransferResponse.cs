namespace MobileBanking.Models.Response.ISmart;

public class FundTransferResponse
{
    public decimal balance { get; set; }
    public string isoResponseCode { get; set; } = "00";
    public string transactionId { get; set; } = "";
}
