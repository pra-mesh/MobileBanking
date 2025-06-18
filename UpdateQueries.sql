go
create or ALTER procedure [dbo].[balancewithfullacno](@accountno nvarchar(30))
as 
begin
  select depositmaster.DepositType as SavingName, Depositmaster.mainbookno,depositmaster.accountno as acno1, DepositMaster.Operator1 as AccountHolder,
  Left(depositmaster.mainbookno,3)+right(Depositmaster.mainbookno,2) + depositmaster.accountno as accountno,
  memberdetail.MemName, isnull(savings.RateOfInterest,0) as interestrate, isnull(depositmaster.MinimumBalance,0) as minBal, 
  isnull(depositmaster.Disabled,0) as Disabled, 
  isnull(mt.balance,0) as balance, isnull(gt.guarantedamt,0) as gamt, isnull(depositmaster.LockedAmount,0) as lamt from DepositMaster 
  left outer join MemberDetail on depositmaster.MemberNo = memberdetail.MemberNo 
  left outer join savings on depositmaster.MainBookNo = savings.AccountNo
  left outer join (select maintransbook.acno, maintransbook.itemcode,SUM(credit)- SUM(debit)as balance from MainTransBook  Group by acno, 
  itemcode having left(acno,3) + right(acno,2) + Itemcode =@accountno) as mt
  on depositmaster.MainBookNo = mt.acno and depositmaster.accountno = mt.itemcode
  left outer join (select Guarantee.acno, Guarantee.itemno,isnull(SUM(lockedAmount),0)as GuarantedAmt from Guarantee group by Acno, itemno 
  having left(guarantee.acno,3) + right(guarantee.acno,2) + guarantee.itemno =@accountno) as gt
  on depositmaster.MainBookNo = gt.acno and depositmaster.accountno = gt.itemno
  where left(depositmaster.mainbookno,3) + right(Depositmaster.mainbookno,2) + depositmaster.accountno =@accountno
 end
 go

create or ALTER PROCEDURE [dbo].[SP_MBFullStatment] 
	(
    @Accountno VARCHAR(20) ,
    @FromDate date,
    @ToDate Date) 
AS
Begin
select * from (select journalno, TransDate as [Date], dbo.engToNep(transdate)as Miti, Particulars as remarks, abs(Credit - Debit) as [Amount],ACNO,
Case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type]
from maintransbook where left(acno,3) + right(acno,2) + itemcode = @accountNo and TransDate>= @fromdate and TransDate <@toDate
union
Select '' journalno,@fromdate as date, dbo.engToNep(@FromDate)as Miti, 'Opening Balance' as Description,
isNull(sum(credit)-sum(debit),0)  as amount,Max(ACNO) as ACNO,
Case when isNull(sum(credit)-sum(debit),0)>0 then 'Debit' else 'Credit' end as  [type] from Maintransbook where left(Maintransbook.acno,3) 
                    + right(Maintransbook.acno,2) + maintransbook.itemcode = @accountNo and transdate < @fromdate) a order by a.Date,JournalNO

end

go
create or ALTER  PROCEDURE [dbo].[SP_MBMiniStatment]
    @Accountno VARCHAR(20) ,
    @NoOfTransaction int
AS
BEGIN
select top(@NoOfTransaction) journalno,TransDate as date,  dbo.engToNep(transdate)as Miti,
(Select Description from Journalnos where journalno=m.journalno) 
as Description, Credit - Debit as [Amount],
case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type] from maintransbook m 
where left(acno,3) + right(acno,2) + itemcode = @AccountNO order by TransDate desc
end