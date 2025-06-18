declare @fromdate nvarchar(10) ='7/22/2024'
declare @toDate date =DATEADD(DAY,1, GETDATE()), @accountNo nvarchar(20)= '03002GS1716' , @NoOfTransaction int =10

select * from (select journalno, TransDate as [Date], dbo.engToNep(transdate)as Miti, Particulars as remarks, abs(Credit - Debit) as [Amount],ACNO,
Case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type]
from maintransbook where left(acno,3) + right(acno,2) + itemcode = @accountNo and TransDate>= @fromdate and TransDate <@toDate
union
Select '' journalno,@fromdate as date, dbo.engToNep(@FromDate)as Miti, 'Opening Balance' as remarks,
isNull(sum(credit)-sum(debit),0)  as amount,Max(ACNO) as ACNO,
Case when isNull(sum(credit)-sum(debit),0)>0 then 'Debit' else 'Credit' end as  [type] from Maintransbook where left(Maintransbook.acno,3) 
                    + right(Maintransbook.acno,2) + maintransbook.itemcode = @accountNo and transdate < @fromdate) a order by a.Date,JournalNO

select top(@NoOfTransaction) journalno, dbo.engToNep(transdate)as Miti,TransDate as date, 
(Select Description from Journalnos where journalno=m.journalno) 
as Description, Credit - Debit as [Amount],
case when dr_cr='DR' then 'Debit' else 'Credit' end as  [type] from maintransbook m 
where left(acno,3) + right(acno,2) + itemcode = @AccountNO order by TransDate desc

