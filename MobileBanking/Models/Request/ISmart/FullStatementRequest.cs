using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class FullStatementRequest
{
    public string branchId { get; init; } = "00";
    [Required]
    public required string accountNumber { get; init; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime fromDate { get; init; } = DateTime.Now;
    [Required]
    [DataType(DataType.Date)]
    public DateTime toDate { get; init; } = DateTime.Now;
}
