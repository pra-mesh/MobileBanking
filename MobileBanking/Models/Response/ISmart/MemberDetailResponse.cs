namespace MobileBanking.Models.Response.ISmart;

public class MemberDetailResponse : BaseResponse
{
    public required string MemberName { get; init; }
    public required string Address { get; init; }
    public required string MobileNumber { get; init; }
    public string? Gender { get; init; } = "Male";
    public required List<DepositAcccouts> AccountList { get; init; }
    public List<LoanDetail>? LoanList { get; init; }
    public List<ShareDetail>? ShareList { get; init; }
}
