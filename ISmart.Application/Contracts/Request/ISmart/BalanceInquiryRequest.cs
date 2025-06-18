using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Application.Contracts.Request.ISmart;
public class BalanceInquiryRequest
{
    public string branchId { get; init; } = "";
    [Required]
    public string accountNumber { get; init; } = "";
}
