namespace MobileBanking.Models.Response.ISmart;

public class LoanStatementResonse : BaseResponse
{
    public List<LoanFullStatement>? StatementList { get; set; }
}
