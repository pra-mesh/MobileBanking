namespace MobileBanking.Models.Response.ISmart;

public class AccountsResponse : BaseResponse
{
    public List<Accounts>? accountList { get; init; }
}
