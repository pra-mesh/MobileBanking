namespace MobileBanking.Models.Request.ISmart;

public class AccountDetailByIdRequest
{
    public string? mobileNumber { get; set; }
    public string? accountNumber { get; set; }
    public string? branchCode { get; set; }
}
