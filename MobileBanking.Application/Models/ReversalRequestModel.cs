namespace MobileBanking.Application.Models;
public class ReversalRequestModel : TranactionStatusInquiryModel
{
    public string? Description { get; init; } = "";
    public string? Newbvrcno { get; init; } = "";
}
