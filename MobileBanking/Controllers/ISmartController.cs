using Microsoft.AspNetCore.Mvc;
using MobileBanking.Application.Contracts.Request.ISmart;
using MobileBanking.Application.Contracts.Response.ISmart;
using MobileBanking.Application.Services;

namespace MobileBanking.Controllers;
[Route("v1/api/transaction")]
[ApiController]
public class ISmartController : ControllerBase
{
    private readonly IBalanceInquiry _balanceInquiry;
    private readonly IStatementServices _statementServices;

    public ISmartController(IBalanceInquiry balanceInquiry, IStatementServices statementServices)
    {
        _balanceInquiry = balanceInquiry;
        _statementServices = statementServices;
    }
    [HttpPost("BalanceInquiry")]
    public async Task<BalanceInquiryResponse> Inquiry(BalanceInquiryRequest req) =>
    await _balanceInquiry.GetBalance(req);
    [HttpPost("fullstatement")]
    public async Task<FullStatementResponse> FullStatement(FullStatementRequest req) =>
        await _statementServices.FullStatement(req);
    [HttpPost("ministatement")]
    public async Task<MiniStatementResponse> MiniStatement(MiniStatementRequest req) =>
        await _statementServices.MiniStatement(req);
}
