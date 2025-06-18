using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class FundTransferRequest
{
    public string? srcBranchId { get; init; }
    [Required]
    public string srcAccount { get; init; } = "";
    public string? srcAccountType { get; init; }
    public string? destBranchId { get; init; }
    [Required]
    public string destAccount { get; init; } = "";
    public string? destAccountType { get; init; }
    [Required]
    [MaxLength(100)]
    public string description1 { get; init; } = "";
    [MaxLength(100)]
    public string? description2 { get; init; } = "";
    [MaxLength(100)]
    public string? description3 { get; init; }
    [Required]
    public string? tranCode { get; init; }
    [Required]
    public DateTime tranDate { get; init; }
    [Required]
    public decimal amount { get; init; } = 0;
}
