create or ALTER PROCEDURE [dbo].[SP_MBFullStatment] 
	(
    @Accountno VARCHAR(20) ,
    @FromDate date,
    @ToDate Date) 
AS
Begin
select * from (select journalno, TransDate as [Date], dbo.engToNep(transdate)as Miti, Particulars as Description, abs(Credit - Debit) as [Amount],ACNO,
Case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type]
from maintransbook where left(acno,3) + right(acno,2) + itemcode = @accountNo and TransDate>= @fromdate and TransDate <@toDate
union
Select '' journalno,@fromdate as date, dbo.engToNep(@FromDate)as Miti, 'Opening Balance' as Description,
abs(isNull(sum(credit)-sum(debit),0))  as amount,Max(ACNO) as ACNO,
Case when isNull(sum(credit)-sum(debit),0)>0 then 'Credit' else 'Debit' end as  [type] from Maintransbook where left(Maintransbook.acno,3) 
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
as Description, abs(Credit - Debit) as [Amount],
case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type] from maintransbook m 
where left(acno,3) + right(acno,2) + itemcode = @AccountNO order by TransDate desc
end