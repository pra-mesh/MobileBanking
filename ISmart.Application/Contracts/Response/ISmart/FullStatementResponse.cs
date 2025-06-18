namespace MobileBanking.Application.Contracts.Response.ISmart;
public class FullStatementResponse : BaseResponse
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
    public List<FullStatement>? statementList { get; init; }
}
public class FullStatement : MiniStatement
{
    private decimal balance = 0;
    public decimal Balance { get => Math.Round(balance, 2); set => balance = Math.Round(value, 2); }
}