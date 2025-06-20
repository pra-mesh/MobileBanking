using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;

public class FundtransferReverseRequest
{
    [Required]
    [MaxLength(30)]
    public required string transactionIdentifier { get; set; }
    [Required]
    [MaxLength(30)]
    public required string newTransactionIdentifier { get; set; }
}
