using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;

public class FundTransferStatusCheckRequest
{
    [Required]
    public required string transactionIdentifier { get; set; }
}
