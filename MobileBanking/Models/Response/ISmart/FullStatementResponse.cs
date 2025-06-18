namespace MobileBanking.Models.Response.ISmart;
public class FullStatementResponse : BaseResponse
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
    public List<FullStatementList>? statementList { get; init; }
}
public class FullStatementList : MiniStatementList
{
    public decimal Balance { get; init; } = 0;
}