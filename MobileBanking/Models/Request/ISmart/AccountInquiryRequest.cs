using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class AccountInquiryRequest
{
    public string branchId { get; init; } = "";
    [Required]
    public string accountNumber { get; init; } = "";
}
