using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Models.Request.ISmart;
public class FullStatementRequest
{
    public string branchId { get; init; } = "00";
    public required string accountNumber { get; init; }
    [DataType(DataType.Date)]
    public DateTime fromDate { get; init; } = DateTime.Now;

    [DataType(DataType.Date)]
    public DateTime toDate { get; init; } = DateTime.Now;
}
