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

go
create or ALTER     procedure [dbo].[sp_GetJournalno]
(@tdate date, @description nvarchar(100),@BranchID nvarchar(3), @user nvarchar(100) ,@newjno int  output) 
as 
declare @genJournal int =0;
select @genJournal = Isnull([Values],0) from Official where Item='GenerateJournalNo'
if(@genJournal=0)
begin
	INSERT INTO [dbo].[JournalNos]
           ([Description]
           ,[Date]
           ,[BranchID])
     VALUES (@description,@tdate,@BranchID) select @newjno=SCOPE_IDENTITY();
end
else
Begin 
	declare @tblname sysname
	declare @fyStartName nvarchar(5) 
	select @fyStartName =substring(fyname,3,3)  from fiscalyears where @tdate between startdate and enddate  
	select @tblName ='JN' + @fyStartName 
 
    DECLARE @DynamicSQL NVARCHAR(4000), @par nvarchar(100) 
	declare @type nvarchar(10)
	set @type ='U'
    SET @DynamicSQL ='IF NOT EXISTS (SELECT * FROM sysobjects WHERE name=''' + @tblname + ''' and xtype=''' + @type +''') CREATE TABLE ' + @tblname + '([JournalNo] [int] IDENTITY(1,1) NOT NULL,	[Description] [nvarchar](255) NULL,	[Date] DateTime  null,	username nvarchar(100) NULL)';
    EXECUTE sp_executesql @DynamicSQL;
	
	set @DynamicSQL =    'Insert into ' + @tblName +  '([description], UserName)    values (''' + @description + ''',''' + @user + ''')'
		+' select @jnoout = cast(SCOPE_IDENTITY() as int)'
		set @par =N'@jnoout int OUTPUT';
	
	EXECUTE sp_executesql @DynamicSQL,@par,@jnoout=@newjno output;
    set @newjno= @newjno + Convert(int,@fyStartName) *1000000
	INSERT INTO [dbo].[JournalNos]
           ([JournalNo]
           ,[Description]
           ,[Date]
           ,[BranchID])
     VALUES (@newjno,@description,@tdate,@BranchID)
end 

go
create or alter procedure sp_MobileTransaction ( @srcAccount nvarchar(20), @destAccount nvarchar(20), @description1 nvarchar(100),
@description2 nvarchar(100)='Mobile Banking Transaction',@description3 nvarchar(100) ='',@transCode nvarchar(30)='',
@transDate Datetime = getDate,@enteredBy nvarchar(50), @amount money , @journalno int =0 output, @transno int =0 output, @message nvarchar(100) output )
as
begin 
declare @srcAccountCount int = 0, @srcAcno nvarchar(6), @srcMano nvarchar(3),@srcItemcode nvarchar(15),
@srcItemName nvarchar(100), @srcBranchId nvarchar(100),@srcBalanceSide int =1, @srcBalance money
declare @destAccountCount int = 0, @destAcno nvarchar(6), @destMano nvarchar(3),@destItemcode nvarchar(15),
@destItemName nvarchar(100), @destBranchId nvarchar(100),@destBalanceSide int =1, @destBalance money
declare @curJno int;

Begin Try
set @journalno=isNUll(@journalno, 0);
set @transno=isNUll(@transno, 0);
set @transDate = ISNULL(@transDate, GETDATE());
set @description2=isNUll(@description2, 'Mobile Banking Transaction');
set @description1=isNUll(@description1, 'Mobile Banking Transaction');


drop table if exists #accounts
select ACNO, MANO,ITEMCODE,ITEMNAME,branchid into #accounts from itms1 where REPLACE(ACNO,'.','')+ITEMCODE in (@srcAccount,@destAccount)
if (select count(acno) from #accounts)<>2
Begin
select @srcAccountCount= count(acno) from #accounts where REPLACE(ACNO,'.','')+ITEMCODE =@srcAccount
if(@srcAccountCount>1)
 Throw 50001,'Multiple source Account found',16; 
if(@srcAccountCount<1)
  Throw 50002,'Source Account Not found',16; 
select @destAccountCount= count(acno) from #accounts where REPLACE(ACNO,'.','')+ITEMCODE =@destAccount
if(@destAccountCount>1)
 Throw 50001,'Multiple Destination Account found',16; 
 if(@destAccountCount<1)
  Throw 50002,'Destination Account Not found',16; 
end

select Top 1 @srcMano=mano, @srcAcno= acno,@srcItemcode =ITEMCODE,@srcItemName =ItemName,@srcBranchId =branchId
from #accounts where REPLACE(ACNO,'.','')+ITEMCODE =@srcAccount


Select Top 1 @destMano=mano, @destAcno=acno, @destItemcode=ITEMCODE,@destItemName = ItemName,@destBranchId =branchId
from itms1 where REPLACE(ACNO,'.','')+ITEMCODE =@destAccount


select @srcBalanceSide=balanceside from mainaccount where acno=@srcMano
select @destBalanceSide = balanceside from mainaccount where acno=@destBalanceSide

if Exists(Select 1 from MainAccount where BalanceSide =-1 and acno=@srcMano)
begin
	if(@srcMano='030')
		set @srcBalance =[dbo].[DepositBalance](@srcAccount)
	else
		Select @srcBalance=@srcBalanceSide*IsNUll((select Balance from ItemBal where REPLACE(acno,'.','')+ITEMCODE =@srcAccount),0)
		
	if(@srcBalance<@amount)
	 throw 50003, 'Insufficient Source Account Balance',1
end

if Exists(Select 1 from MainAccount where BalanceSide =1 and acno=@destMano)
begin
	if(@destMano='030')
		set @destBalance =[dbo].[DepositBalance](@destAccount)
	else
		Select @destBalance=@destBalanceSide*IsNUll((select Balance from ItemBal where REPLACE(acno,'.','')+ITEMCODE =@destAccount),0)
	if(@destBalance<@amount)
	 throw 50003, 'Insufficient Destination Account Balance',1
end

Begin TRANSACTION

insert into TransOne (DESCRIPTION,transdate,TTID,TransactionType, PartyType ,EnteredBy) 
values (@description1,@transdate,@transCode,'Mobile Banking','Mobile',@enteredBy)
set @transno=SCOPE_IDENTITY();
if(@transno = 0)
 throw 50004, 'Could not generate Transno',1
exec [dbo].[sp_GetJournalno] 
				@tdate =@transdate,
				@description = @description1,
				@user = @enteredBy,
				@branchid=@srcBranchId,
				@newjno = @curJno OUTPUT
if(@curJno = 0)
 throw 50004, 'Could not generate Journalno',1
set @journalno = @curJno;

if(@destBranchId= @srcBranchId)
Begin

		insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values (@curJno,@transCode,@transDate,@srcBranchId,@srcMano,@srcAcno,@srcItemcode,@srcItemName,'','Mobile Banking',@description2,'DR'
            ,@amount,0,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,@destBranchId,@destMano,@destAcno,@destItemcode,@destItemName,'','Mobile Banking',@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE())
End
else
Begin
declare @headItemName nvarchar(15),@srcBranchItemName nvarchar(15),@destBranchItemName nvarchar(15), 
@ibtmano nvarchar(3)='120', @ibtAcno nvarchar(6) ='120.20', @headItemcode nvarchar(5)='00'
select @headItemName=Isnull(branchName,'HEAD OFFICEE') from Branches where BranchId='00'
select @srcBranchItemName=Isnull(branchName,@srcBranchId) from Branches where BranchId=@srcBranchId
select @destBranchItemName=Isnull(branchName,@destBranchId) from Branches where BranchId=@destBranchId
insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values (@curJno,@transCode,@transDate,@srcBranchId,@srcMano,@srcAcno,@srcItemcode,@srcItemName,'','Mobile Banking',@description2,'DR'
            ,@amount,0,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,@srcBranchId,@ibtmano,@ibtAcno,@headItemcode,@headItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE())

exec [dbo].[sp_GetJournalno] 
				@tdate =@transdate,
				@description = @description1,
				@user = @enteredBy,
				@branchid=@srcBranchId,
				@newjno = @curJno OUTPUT
if(@curJno = 0)
 throw 50004, 'Could not generate Journalno',1
 insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values  (@curJno,@transCode,@transDate,@destBranchId,@ibtmano,@ibtAcno,@headItemcode,@headItemName,'','Mobile Banking','IBT '+@description2,'DR'
			,@amount,0,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE()),
			   (@curJno,@transCode,@transDate,@destBranchId,@destMano,@destAcno,@destItemcode,@destItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE())
exec [dbo].[sp_GetJournalno] 
				@tdate =@transdate,
				@description = @description1,
				@user = @enteredBy,
				@branchid=@srcBranchId,
				@newjno = @curJno OUTPUT
if(@curJno = 0)
 throw 50004, 'Could not generate Journalno',1
 	 insert into maintransbook ([Journalno],[BVRCNO],[transDate],[branchid],[mano],[acno],[itemcode],[itemname],[itemlocation]
            ,[receivedpaidBy],[particulars],[dr_cr],[Debit],[Credit],[description],[Remarks1],[Remarks2],[Remarks3],[Remarks4],[TransNoa]
            ,[EnteredBy],[EntryDate]) 
            values  (@curJno,@transCode,@transDate,'00',@ibtmano,@ibtAcno,@srcBranchId,@srcBranchItemName,'','Mobile Banking','IBT '+@description2,'DR'
			,@amount,0,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,'00',@ibtmano,@ibtAcno,@destBranchId,@destBranchItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'',@transno,@EnteredBy,GETDATE())	

end

Commit Transaction
set @message ='Transaction Successfull'

End Try
Begin Catch
IF @@TRANCOUNT > 0
	BEGIN
		ROLLBACK TRANSACTION;
	END
PRINT ERROR_MESSAGE()
PRINT 'Error on Line Number ' + Cast(ERROR_LINE() as nvarchar(10))
set @journalno=0;
set @transno=0;
set @message =ERROR_MESSAGE();
end Catch
end




