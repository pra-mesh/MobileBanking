namespace MobileBanking.Application.Contracts.Response.ISmart;
public class BalanceInquiryResponse : BaseResponse
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
}
