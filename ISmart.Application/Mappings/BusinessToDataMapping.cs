using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Application.Mappings;
internal static class BusinessToDataMapping
{
    public static TransactionDTO ToTransactionModel(FundTransferModel fundTransfer) =>
        new TransactionDTO
        {
            TransDate = fundTransfer.transDate,
            TrDesc = fundTransfer.description1,
            TTID = fundTransfer.transCode ?? "",
            TransactionType = "Mobile banking",
            PartyType = "Mobile",
            PartyDocument = fundTransfer.description2 ?? "",
            EnteredBy = fundTransfer.enteredBy,
        };

    public static JournalNoDTO ToJournalNoDTO(FundTransferModel fundTransfer) =>
        new JournalNoDTO
        {
            tdate = fundTransfer.transDate,
            description = fundTransfer.description1,
            branchId = fundTransfer.srcBranchId,
            user = fundTransfer.enteredBy,
        };
}
