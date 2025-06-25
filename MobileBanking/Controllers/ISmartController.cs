using Microsoft.AspNetCore.Mvc;
using MobileBanking.Application.Services;
using MobileBanking.Attributes;
using MobileBanking.Mappings.APIToBusinessMappings;
using MobileBanking.Mappings.BusinessToAPIMapping;
using MobileBanking.Models.Request.ISmart;
using MobileBanking.Models.Response.ISmart;

namespace MobileBanking.Controllers;
[Route("v1/api/transaction")]
[ApiKey]
[ApiController]
public class ISmartController : ControllerBase
{
    private readonly IBalanceInquiry _balanceInquiry;
    private readonly IStatementServices _statementServices;
    private readonly ITransactionService _transactionService;
    private readonly ILoanServices _loanServices;

    public ISmartController(
        IBalanceInquiry balanceInquiry,
        IStatementServices statementServices,
        ITransactionService transactionService,
        ILoanServices loanServices)
    {
        _balanceInquiry = balanceInquiry;
        _statementServices = statementServices;
        _transactionService = transactionService;
        _loanServices = loanServices;
    }

    [HttpPost("BalanceInquiry")]
    public async Task<BalanceInquiryResponse> Inquiry(AccountInquiryRequest req)
    {
        var balanceRequest = ISMartRequestMapping.ToBalanceInquiryRequest(req);
        var balanceResult = await _balanceInquiry.GetBalance(balanceRequest);
        return ISmartResponseMapping.ToBalanceInquiryResponse(balanceResult);
    }

    [HttpPost("fullstatement")]
    public async Task<FullStatementResponse> FullStatement(FullStatementRequest req)
    {
        var statementRequest = ISMartRequestMapping.ToFullStatmentInquiryModel(req);
        var result = await _statementServices.FullStatementBalance(statementRequest);
        return ISmartResponseMapping.ToFullStatementResponse(result);
    }

    [HttpPost("ministatement")]
    public async Task<MiniStatementResponse> MiniStatement(MiniStatementRequest req)
    {
        var miniStatementRequest = ISMartRequestMapping.ToMiniStatementInquiryModel(req);
        var result = await _statementServices.MiniStatementBalance(miniStatementRequest);
        return ISmartResponseMapping.ToMiniStatementResponse(result);
    }

    [HttpPost()]
    public async Task<FundTransferResponse> FundTransferResponse(FundTransferRequest req)
    {
        var fundTransfer = ISMartRequestMapping.ToFundTransferModel(req);
        var result = await _transactionService.FundTransferbyProcWithBalance(fundTransfer);
        return ISmartResponseMapping.ToFundTransferResponse(result);
    }

    [HttpPost("fundTransferStatus")]
    public async Task<FundTransferStatusResponse> FundTransferStatus(FundTransferStatusCheckRequest req)
    {
        var FundTransferStatus = ISMartRequestMapping.ToFundTransferStatusInquiryModel(req);
        var result = await _transactionService.FundTransferStatus(FundTransferStatus);
        return ISmartResponseMapping.ToFundTransferStatusResponse(result);
    }

    [HttpPost("Reversal")]
    public async Task<FundTransferStatusResponse> ReversalIsmart(FundtransferReverseRequest req)
    {
        var result = await _transactionService.TransactionReversal
            (ISMartRequestMapping.ToReversalRequestModel(req));
        return ISmartResponseMapping.ToRevesalStatusResponse(result);
    }

    [HttpPost("AccountValidation")]
    public async Task<AccountsDetailResponse> AccountDetail(AccountDetailByIdRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.accountNumber) && string.IsNullOrWhiteSpace(req.mobileNumber))
            throw new Exception("Unable To Process [Required either  mobile number or account no]");
        var result = await _balanceInquiry.GetAccountsDetailList(ISMartRequestMapping.ToAllDetailsQueryModel(req));
        if (result.Count < 1)
            throw new Exception("Unable To Process [No Account found]");
        return new AccountsDetailResponse
        {
            accountList = result.Select(ISmartResponseMapping.ToAccountFullDetail).ToList(),
            isoResponseCode = "00"
        };
    }

    [HttpPost("MobileNumberValidation")]
    public async Task<AccountsResponse> AccountValidationbyMobile(AccountByMobileNumberRequest gm)
    {
        var result = await _balanceInquiry.GetAccounts(ISMartRequestMapping.ToAllDetailsQueryModelOnlyMobile(gm));
        if (result.Count < 1)
            throw new Exception("Unable To Process [No Account found]");
        return new AccountsResponse
        {
            isoResponseCode = "00",
            accountList = result.Select(ISmartResponseMapping.ToAccounts).ToList(),
        };
    }

    [HttpPost("loanInformation")]
    public async Task<LoanDetailResponse> LoanDetail(AccountInquiryRequest req)
    {
        var result = await _loanServices.LoanDetail
            (ISMartRequestMapping.ToBalanceInquiryRequest(req));
        return ISmartResponseMapping.ToLoanDetailResponse(result);
    }

    [HttpPost("loanFullStatement")]
    public async Task<LoanStatementResonse> LoanStatement(FullStatementRequest req)
    {
        var result = await _loanServices.LoanStatement
            (ISMartRequestMapping.ToFullStatmentInquiryModel(req));
        return new LoanStatementResonse
        {
            isoResponseCode = "00",
            StatementList = result.Select(ISmartResponseMapping.ToLoanFullStatement).ToList(),
        };
    }


}
