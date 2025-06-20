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

go
create or ALTER procedure [dbo].[sp_TransactionReversal]
( @Journalno int =0 , @bvrcno nvarchar(30) ='',
@Description varchar(100)='Reversal', @enteredBy nvarchar(30),@newbvrcno nvarchar(30)='',
 @newJournalno int=0 output,@newTransno int =0 output, @Message nvarchar(100) output)
 as
 Begin
 set @Journalno = ISNULL(@journalno,0);
 set @bvrcno = ISNULL(@bvrcno,'');
 set @Description = ISNULL(@Description,'Reversal');
 set @newbvrcno = ISNULL(@newbvrcno,'');
 set @newJournalno = ISNULL(@newJournalno,0);
declare @oldTransnoa int, @count int;
declare @VoucherAvailable int ;
declare @oldJno int =0;
declare @curJno int =0;
Declare @branchid nvarchar(3)='00', @transdate DateTime = GetDate()
declare @curBVRCNO nvarchar(50) ='';

--Check and load current transnoa
if(@Journalno=0 and LTRIM(RTRIM(@bvrcno))='')
begin
select @newJournalno=0, @newTransno =0, @Message='Invalid Request'
end
if(@Journalno =0 and LTRIM(RTRIM(@bvrcno))<>'')
begin
select @count = count(Distinct transnoa),@oldTransnoa =max(transnoa)  from MainTransBook where BVRCNO=@bvrcno
end
if(@Journalno <>0)
begin
select @count = count(Distinct transnoa), @oldTransnoa =max(transnoa) from MainTransBook where JournalNO=@Journalno
end
if(@count>1)
begin
select @newJournalno=0, @newTransno =0, @Message='Multiple transaction found'
return
end
if(@count=0)
begin
select @newJournalno=0, @newTransno =0, @Message='Transaction not found'
return
end

set NoCount ON
SET XACT_ABORT ON

Begin Try
	Begin Transaction;
		insert into TransOne (DESCRIPTION,transdate) values ('Voucher Reverse tranaction'+convert(varchar(15),@oldTransnoa),@transdate)
		set @newTransno=SCOPE_IDENTITY();
		
		Declare curJournalnos Cursor local Keyset for
		Select distinct journalno from MainTransBook where TransNoA=@oldTransnoa
		open curJournalnos
		fetch first from curJournalnos into @oldJno
		while @@FETCH_STATUS =0
		begin
        Select   @branchid = max(Branchid)  from Maintransbook where journalno = @oldJno 
		exec [dbo].[sp_GetJournalno] 
				@tdate =@transdate,
				@description = @Description,
				@user = @enteredBy,
				@branchid=@branchid,
				@newjno = @curJno OUTPUT
			if( @newJournalno is null or @newJournalno =0 )
			 set @newJournalno=@curJno;
	
	
	set @curBVRCNO =case when ltrim(rtrim(@newbvrcno))='' then 'RV'+ cast(@oldJno as nvarchar(15)) else @newbvrcno end;

	insert into maintransbook (
       [JournalNO],[ReceiptNo],[BVRCNO],[TransDate],[MANO],[ACNO],[ItemCode],[ItemName],[ItemLocation],[ReceivedPaidBy]
      ,[Particulars],[Dr_Cr] ,[Debit] ,[Credit] ,[EnteredBy] ,[EntryDate] ,[Description] ,[Remarks1] ,[Remarks2] ,[Remarks3]
      ,[Remarks4] ,[TransNoA] ,[TransNoM] ,[Area],[ApprovedBy],[ApprovedOn],[MFGroup]
      ,[MemberNO],[BillNO] ,[PartyID],[PartyName],[DebQty],[CredQty] ,[Rate],[TransactionType],[BatchNo]
      ,[mfdDate],[ExpDate] ,[TransNo],[BranchID])
      
       SELECT @curJno as journalno ,0 as receiptno,@curBVRCNO,
       @transDate as Transdate,[MANO],[ACNO],[ItemCode],[ItemName],
       [ItemLocation],'Office' as receivedpaidby , 'Voucher Reversed ' + @description as particulars ,'DR' as dr_cr,
       [Credit] as debit  ,[Debit] as credit ,@enteredBy as enteredby,
       @transdate as entrydate,'Voucher Reversed ' +Convert(varchar(20), @oldJno) as [Description] ,[Remarks1] ,[Remarks2] ,[Remarks3]
      ,[Remarks4] ,@newTransno as transnoa ,[TransNoM] ,[Area],@enteredBy as approvedby ,GetDate()approvedon,[MFGroup]
      ,[MemberNO],[BillNO] ,[PartyID],[PartyName],[CredQty]as Debqty,[DebQty] as CredQty ,[Rate],[TransactionType],[BatchNo]
      ,[mfdDate],[ExpDate] ,[TransNo],[BranchID] 
       from [MainTransBook]  where journalno = @oldJno and credit> 0 
 		union all
       SELECT @curJno as journalno ,0 as receiptno,@curBVRCNO,
       @transDate as Transdate,[MANO],[ACNO],[ItemCode],[ItemName],
       [ItemLocation],'Office' as receivedpaidby ,'Voucher Reversed ' + @description as particulars ,'CR' as dr_cr,
       [Credit] as debit  ,[Debit] as credit ,@enteredBy as enteredby,
       @transdate as entrydate,'Voucher Reversed ' +Convert(varchar(20), @oldJno) as [Description] ,[Remarks1] ,[Remarks2] ,[Remarks3]
      ,[Remarks4] ,@newTransno as transnoa ,[TransNoM] ,[Area],@enteredBy as approvedby ,GetDate()approvedon,[MFGroup]
      ,[MemberNO],[BillNO] ,[PartyID],[PartyName],[CredQty]as Debqty,[DebQty] as CredQty ,[Rate],[TransactionType],[BatchNo]
      ,[mfdDate],[ExpDate] ,[TransNo],[BranchID] 
       from [MainTransBook]  where journalno = @oldJno and debit> 0 
			
			fetch next from curJournalnos into @oldJno
		end
		close curJournalnos
		deallocate curJournalnos
	Commit Transaction;
	set @Message='Voucher reversed successfully'
End Try

BEGIN CATCH
IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION;
	END
	PRINT ERROR_MESSAGE()
	PRINT 'Error on Line Number ' + Cast(ERROR_LINE() as nvarchar(10))
select @newJournalno=0, @newTransno =0, @Message='Reversal failed'
End catch
End