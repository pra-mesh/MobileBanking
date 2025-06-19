namespace MobileBanking.Application.Models;
public class MiniStatementModel
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
    public List<MiniStatement>? statementList { get; init; }

}
public class MiniStatement
{
    public required DateTime date { get; init; }
    public required string remarks { get; init; }
    public required string type { get; init; }
    public decimal amount { get; init; }

}