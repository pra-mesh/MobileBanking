namespace MobileBanking.Application.Models;
public class FullStatementModel
{
    public decimal minimumBalance { get; init; } = 0;
    public decimal availableBalance { get; init; } = 0;
    public List<Statement>? statementList { get; init; }
}
public class Statement : MiniStatement
{
    public decimal Balance { get; set; } = 0;
}