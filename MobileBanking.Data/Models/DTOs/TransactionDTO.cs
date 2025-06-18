namespace MobileBanking.Data.Models.DTOs;
public class TransactionDTO
{
    public DateTime TransDate { get; set; } = DateTime.Now;
    public string TrDesc { get; set; } = "";
    public string TTID { get; set; } = "";
    public string TransactionType { get; set; } = "Mobile Banking";
    public string PartyType { get; set; } = "Depositer";
    public string PartyID { get; set; } = "";
    public string PartyDocumentNO { get; set; } = "";
    public string PartyReceiptno { get; set; } = "";
    public string Billno { get; set; } = "";
    public string ReceiptNo { get; set; } = "";
    public string EnteredBy { get; set; } = "mBank";
    public DateTime EntryDate { get; set; } = DateTime.Now;
    public string BranchID { get; set; } = "";
    public string MemberNO { get; set; } = "";
    public string MemberName { get; set; } = "";
    public string PartyDocument { get; set; } = "";
}
