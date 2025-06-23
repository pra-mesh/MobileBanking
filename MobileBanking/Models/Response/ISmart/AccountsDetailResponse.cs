namespace MobileBanking.Models.Response.ISmart;

public class AccountsDetailResponse : BaseResponse
{
    public List<AccountFullDetail>? accountList { get; set; }
}
