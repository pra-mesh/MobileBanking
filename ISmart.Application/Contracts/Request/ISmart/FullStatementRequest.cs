using System.ComponentModel.DataAnnotations;

namespace MobileBanking.Application.Contracts.Request.ISmart;
public class FullStatementRequest
{
    public string branchId { get; set; } = "00";
    public required string accountNumber { get; set; }
    [DataType(DataType.Date)]
    public DateTime fromDate { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    public DateTime toDate { get; set; } = DateTime.Now;
}
