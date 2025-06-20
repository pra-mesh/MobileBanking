namespace MobileBanking.Application.Models;
public class TranactionStatusInquiryModel
{
    public string? BVRCNO { get; init; } = "";
    public int JournalNo { get; init; } = 0;
    public required string enteredBY { get; init; }
}
