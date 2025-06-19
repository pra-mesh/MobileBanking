namespace MobileBanking.Application.Models;
public class FullStatmentInquiryModel
{
    public string branchId { get; init; } = "00";
    public required string accountNumber { get; init; }
    public DateTime fromDate { get; init; } = DateTime.Now;
    public DateTime toDate { get; init; } = DateTime.Now;
}
