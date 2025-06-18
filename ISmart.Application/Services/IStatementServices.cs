using MobileBanking.Application.Contracts.Request.ISmart;
using MobileBanking.Application.Contracts.Response.ISmart;

namespace MobileBanking.Application.Services;
public interface IStatementServices
{
    Task<FullStatementResponse> FullStatement(FullStatementRequest req);
    Task<MiniStatementResponse> MiniStatement(MiniStatementRequest req);
}