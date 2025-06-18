namespace MobileBanking.Data.Models.DTOs;
public class TransactionDataDTO
{
    public int Journalno { get; set; }
    public string BVRCNO { get; set; } = "";
    public DateTime transDate { get; set; } = DateTime.Now;
    public string branchid { get; set; } = "";
    public string mano { get; set; } = "";
    public string acno { get; set; } = "";
    public string itemcode { get; set; } = string.Empty;
    public string itemname { get; set; } = "";
    public string itemlocation { get; set; } = "";
    public string receivedpaidBy { get; set; } = "";
    public string particulars { get; set; } = "";
    public string dr_cr { get; set; } = "DR";
    public decimal Debit { get; set; } = 0;
    public decimal Credit { get; set; } = 0;
    public string description { get; set; } = "";
    public string Remarks1 { get; set; } = "";
    public string Remarks2 { get; set; } = "";
    public string Remarks3 { get; set; } = "";
    public string Remarks4 { get; set; } = "";
    public int TransNoa { get; set; } = 0;
    public string EnteredBy { get; set; } = "";
    public DateTime EntryDate { get; set; } = DateTime.Now;
}
