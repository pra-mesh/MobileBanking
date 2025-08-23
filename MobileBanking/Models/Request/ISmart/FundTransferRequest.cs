using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class FundTransferRequest
{
    public string? sourceBranchCode { get; init; }
    [Required]
    public string sourceAccountNumber { get; init; } = "";
    public string? sourceAccountType { get; init; }
    public string? destinationBranchCode { get; init; }
    [Required]
    public string destinationAccountNumber { get; init; } = "";
    public string? destinationAccountType { get; init; }
    [Required]
    [MaxLength(100)]
    public string description1 { get; init; } = "";
    [MaxLength(100)]
    public string? description2 { get; init; } = "";
    [MaxLength(100)]
    public string? description3 { get; init; }
    [Required]
    public string? transactionIdentifier { get; init; }
    [Required]
    public DateTime transactionDate { get; init; }
    [Required]
    public decimal amount { get; init; } = 0;
}
