using Microsoft.AspNetCore.Mvc;
using MobileBanking.Application.Services;
using MobileBanking.Mappings.APIToBusinessMappings;
using MobileBanking.Mappings.BusinessToAPIMapping;
using MobileBanking.Models.Request.ISmart;
using MobileBanking.Models.Response.ISmart;

namespace MobileBanking.Controllers;
[Route("v1/api/transaction")]
[ApiController]
public class ISmartController : ControllerBase
{
    private readonly IBalanceInquiry _balanceInquiry;
    private readonly IStatementServices _statementServices;
    private readonly ITransactionService _transactionService;

    public ISmartController(
        IBalanceInquiry balanceInquiry,
        IStatementServices statementServices,
        ITransactionService transactionService)
    {
        _balanceInquiry = balanceInquiry;
        _statementServices = statementServices;
        _transactionService = transactionService;
    }
    [HttpPost("BalanceInquiry")]
    public async Task<BalanceInquiryResponse> Inquiry(BalanceInquiryRequest req)
    {
        var balanceRequest = ISMartRequestMapping.ToBalanceInquiryRequest(req);
        var balanceResult = await _balanceInquiry.GetBalance(balanceRequest);
        return ISmartResponseMapping.ToBalanceInquiryResponse(balanceResult);
    }

    [HttpPost("fullstatement")]
    public async Task<FullStatementResponse> FullStatement(FullStatementRequest req)
    {
        var statementRequest = ISMartRequestMapping.ToFullStatmentInquiryModel(req);
        var result = await _statementServices.FullStatement(statementRequest);
        return ISmartResponseMapping.ToFullStatementResponse(result);
    }
    [HttpPost("ministatement")]
    public async Task<MiniStatementResponse> MiniStatement(MiniStatementRequest req)
    {
        var miniStatementRequedt = ISMartRequestMapping.ToMiniStatementInquiryModel(req);
        var result = await _statementServices.MiniStatement(miniStatementRequedt);
        return ISmartResponseMapping.ToMiniStatementResponse(result);
    }
    [HttpPost()]
    public async Task<FundTransferResponse> FundTransferResponse(FundTransferRequest req)
    {
        var fundTransfer = ISMartRequestMapping.ToFundTransferModel(req);
        var result = await _transactionService.FundTransferAsync(fundTransfer);
        return ISmartResponseMapping.ToFundTransferResponse(result);
    }
}
