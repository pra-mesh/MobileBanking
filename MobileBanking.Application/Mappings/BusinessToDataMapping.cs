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

    public static JournalNoDTO ToJournalNoDTO(FundTransferModel fundTransfer, bool isIBT) =>
        new JournalNoDTO
        {
            tdate = fundTransfer.transDate,
            description = isIBT ? "IBT - " : "" + fundTransfer.description1,
            branchId = fundTransfer.srcBranchId,
            user = fundTransfer.enteredBy,
        };

    public static TransactionDataDTO ToTransactionDataDTO(FundTransferModel req, AccountIdentifier account, int journalno, int Transno, bool isDebit) =>
        new TransactionDataDTO
        {
            Journalno = journalno,
            BVRCNO = req.transCode ?? "",
            transDate = req.transDate,
            branchid = (isDebit ? req.srcBranchId : req.destBranchId),
            mano = account.Mano,
            acno = account.Acno,
            itemcode = account.Acno,
            itemname = account.ItemName,
            itemlocation = req.enteredBy,
            receivedpaidBy = "Mobile Banking",
            particulars = req.description2 ?? "Mobile Banking Transaction",
            dr_cr = isDebit ? "DR" : "CR",
            Debit = isDebit ? req.amount : 0,
            Credit = isDebit ? 0 : req.amount,
            description = req.description1,
            Remarks1 = req.description1,
            Remarks2 = req.description2,
            Remarks3 = "Mobile Banking",
            Remarks4 = req.description3,
            TransNoa = Transno,
            EnteredBy = req.enteredBy,
            EntryDate = DateTime.Now
        };

}
