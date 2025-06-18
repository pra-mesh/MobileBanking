using MobileBanking.Application.Models;
using MobileBanking.Models.Request.ISmart;

namespace MobileBanking.Mappings.APIToBusinessMappings;

public static class ISMartRequestMapping
{
    public static BalanceInquiryModel ToBalanceInquiryRequest(BalanceInquiryRequest request) =>
        new BalanceInquiryModel
        {
            branchId = request.branchId,
            accountNumber = request.accountNumber
        };
    public static FullStatmentInquiryModel ToFullStatmentInquiryModel(FullStatementRequest req) =>
        new FullStatmentInquiryModel
        {
            branchId = req.branchId,
            accountNumber = req.accountNumber,
            fromDate = req.fromDate,
            toDate = req.toDate,
        };
    public static MiniStatementInquiryModel ToMiniStatementInquiryModel(MiniStatementRequest req) =>
        new MiniStatementInquiryModel
        {
            branchId = req.branchId,
            accountNumber = req.accountNumber,
            count = req.count
        };
}
