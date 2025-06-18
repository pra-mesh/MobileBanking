using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class BalanceInquiryRequest
{
    public string branchId { get; init; } = "";
    [Required]
    public string accountNumber { get; init; } = "";
}
