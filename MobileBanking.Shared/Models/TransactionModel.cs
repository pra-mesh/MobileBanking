namespace MbankWebApi.Model;

public class TransactionModel
{
    public TransactionModel(int journalno, string bVRCNO, DateTime transDate,
        string branchid, string mano, string acno, string itemcode, string itemname,
        string itemlocation, string receivedpaidBy, string particulars, string dr_cr,
        decimal debit, decimal credit, string description, string remarks1, string remarks2,
        string remarks3, string remarks4, int transNoa, string enteredBy, DateTime entryDate)
    {
        Journalno = journalno;
        BVRCNO = bVRCNO;
        this.transDate = transDate;
        this.branchid = branchid;
        this.mano = mano;
        this.acno = acno;
        this.itemcode = itemcode;
        this.itemname = itemname;
        this.itemlocation = itemlocation;
        this.receivedpaidBy = receivedpaidBy;
        this.particulars = particulars;
        this.dr_cr = dr_cr;
        Debit = debit;
        Credit = credit;
        this.description = description;
        Remarks1 = remarks1;
        Remarks2 = remarks2;
        Remarks3 = remarks3;
        Remarks4 = remarks4;
        TransNoa = transNoa;
        EnteredBy = enteredBy;
        EntryDate = entryDate;
    }

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
