using MobileBanking.Application.Models;
using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Models.RequestModels;

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
            description = (isIBT ? "IBT - " : "") + fundTransfer.description1,
            branchId = fundTransfer.srcBranchId,
            user = fundTransfer.enteredBy,
        };

    public static TransactionDataDTO ToTransactionDataDTO(FundTransferModel req, AccountIdentifier account,
        int journalno, int Transno, bool isDebit, string? branchID) => new TransactionDataDTO
        {
            Journalno = journalno,
            BVRCNO = req.transCode ?? "",
            transDate = req.transDate,
            branchid = branchID,
            mano = account.Mano,
            acno = account.Acno,
            itemcode = account.ItemCode,
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

    public static ReverseTansactionDTO ToReverseTansactionDTO(ReversalRequestModel req) =>
        new ReverseTansactionDTO
        {
            BVRCNO = req.BVRCNO,
            Newbvrcno = req.Newbvrcno,
            JournalNo = req.JournalNo,
            Description = req.Description,
            enteredBY = req.enteredBY
        };

    public static TransactionProcDTO ToTransactionProcDTO(FundTransferModel req) =>
        new TransactionProcDTO
        {
            SrcAccount = req.srcAccount,
            DestAccount = req.destAccount,
            Description1 = req.description1,
            Description2 = req.description2,
            Description3 = req.description3,
            TransCode = req.transCode,
            TransDate = req.transDate,
            EnteredBy = req.enteredBy,
            Amount = req.amount

        };

    public static AccountDetailPaged ToAccountDetailPagedDTO(AllDetailsQueryModel req) =>
        new AccountDetailPaged
        {
            MemberNo = req.MemberNo,
            AccountNumber = req.AccountNumber,
            MobileNumber = req.MobileNumber,
        };
}
