namespace MobileBanking.Models.Response.ISmart;
public class MiniStatementResponse : BaseResponse
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
    public List<MiniStatementList>? statementList { get; init; }

}
public class MiniStatementList
{
    public required string date { get; init; }
    public required string remarks { get; init; }
    public required string type { get; init; }
    public decimal amount { get; init; }

}