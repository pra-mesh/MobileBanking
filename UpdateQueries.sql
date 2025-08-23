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
DBCC TRACEON(460, -1);
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
            ,@amount,0,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,@destBranchId,@destMano,@destAcno,@destItemcode,@destItemName,'','Mobile Banking',@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE())
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
            ,@amount,0,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,@srcBranchId,@ibtmano,@ibtAcno,@headItemcode,@headItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE())

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
			,@amount,0,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE()),
			   (@curJno,@transCode,@transDate,@destBranchId,@destMano,@destAcno,@destItemcode,@destItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE())
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
			,@amount,0,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE()),
			  (@curJno,@transCode,@transDate,'00',@ibtmano,@ibtAcno,@destBranchId,@destBranchItemName,'','Mobile Banking','IBT '+@description2,'CR'
			,0,@amount,@description1,@description2,'Mobile Banking',@description3,'Mobile Banking',@transno,@EnteredBy,GETDATE())	

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

go

CREATE or alter PROCEDURE sp_GetDepositAccountDetails
    @memberno VARCHAR(20) = NULL,
    @accountNumber VARCHAR(50) = NULL,
    @mobileNumber VARCHAR(20) = NULL,
    @offset INT = 0,
    @limit INT = 10
AS
begin
   SET NOCOUNT ON;
	select 
		m.MemberNo as memberId,
		m.MemName as memberName,
		VdcMun as address,
		m.mobileno as mobileNumber,
		Replace(MainBookNo,'.','')+d.AccountNo as accountNumber,
		m.BranchID as branchCode, 
		Case 
			when ISnull(d.[Disabled],'False') ='False' then 'True'
			else 'False' 
		end as isActive,
		DateofBirth as  dateOfBirth,
		case 
			when Gender='m' then 'Male'
			when Gender='f' then 'female'
			else 'other' 
		end as Gender,
		DepositType as accountType,
		s.SavingType,
		isnull(d.LockedAmount,0) as LockedAmount,
		isNull(GuarantedAmt,0) as GuarantedAmt,  
		Case
			when isnull(d.MinimumBalance,0)<>0 then isnull(d.MinimumBalance,0)
			else s.MinBal
		end as minBal, 
		balance as ledgerBalance,
		balance-isNull(GuarantedAmt,0)-
		Case 
			when isnull(d.MinimumBalance,0)<>0 then isnull(d.MinimumBalance,0)
			else s.MinBal
		end-isnull(d.LockedAmount,0) as availablebalance,
		d.expiredate,
		d.EntranceDate,
		dbo.Interest(MainBookNo,d.AccountNo,m.BranchID,getDate()) as accruedInterest, 
		case 
			when isNUll(d.InterestRate,0)=0 then isnull(s.RateOfInterest,0)
			else d.InterestRate
		end as interestrate, 
		case 
			when isNUll(d.InterestStartDAte,d.EntranceDate)>s.IntStartDate then isNUll(d.InterestStartDAte,d.EntranceDate) 
			else s.IntStartDate
		end as InterestStartDate,
		'Citizenship' as idType,
		isNUll(m.citizenshipno,'') as idNumber,
		isNUll(m.citDistrict,'') as idIssuePlace,
		isNUll(m.citDate,'') as issueDate 
	from DepositMaster d
	join MemberDetail m on d.MemberNo = m.MemberNo
	join savings s on  s.AccountNo=d.MainBookNo
	join depBal b on d.MainBookNo =b.ACNO and d.AccountNo=b.ItemCode 
	left join (
		select acno, itemno,isnull(SUM(lockedAmount),0)as GuarantedAmt 
		from Guarantee group by Acno, itemno 
		) as gt on gt.acno=d.MainBookNo and gt.ItemNo= d.AccountNo
	 WHERE 
		(d.Disabled = 0 or d.Disabled is null) and
        (@memberno IS NULL OR m.MemberNo = @memberno) AND
        (@accountNumber IS NULL OR REPLACE(MainBookNo, '.', '') + d.AccountNo = @accountNumber) AND
        (@mobileNumber IS NULL OR m.mobileno = @mobileNumber)
	ORDER BY accountNumber
	offset @offset  rows fetch next @limit rows only
end

go
CREATE or alter function [dbo].[DepositBalance](@accountno nvarchar(30))
returns numeric (18,2) 
begin 
 declare @balance numeric(18,2)
 set @balance = (select balq.balance-balq.minBal-balq.gamt-balq.lamt from 
  (select Depositmaster.mainbookno,depositmaster.accountno as acno1, 
  Left(depositmaster.mainbookno,2)+right(Depositmaster.mainbookno,2) + depositmaster.accountno as accountno,MemberDetail.Branchid, 
  case when isnull(depositmaster.MinimumBalance,0)=0 then isNull(savings.MinBal,0) else isnull(depositmaster.MinimumBalance,0) end as minBal, 
  isnull(mt.balance,0) as balance, 
  isnull(gt.guarantedamt,0) as gamt, 
  isnull(depositmaster.LockedAmount,0) as lamt from DepositMaster 
  left outer join MemberDetail on depositmaster.MemberNo = memberdetail.MemberNo 
  left outer join savings on depositmaster.MainBookNo = savings.AccountNo
  left outer join (select maintransbook.acno, maintransbook.itemcode,SUM(credit)- SUM(debit)as balance from MainTransBook
  Group by acno, itemcode having left(acno,3) + right(acno,2) + Itemcode =@accountno) as mt
  on depositmaster.MainBookNo = mt.acno and depositmaster.accountno = mt.itemcode
  left outer join (select Guarantee.acno, Guarantee.itemno,isnull(SUM(lockedAmount),0)as GuarantedAmt from Guarantee group by Acno, itemno 
  having left(guarantee.acno,3) + right(guarantee.acno,2) + guarantee.itemno =@accountno) as gt
  on depositmaster.MainBookNo = gt.acno and depositmaster.accountno = gt.itemno
  where left(depositmaster.mainbookno,3) + right(Depositmaster.mainbookno,2) + depositmaster.accountno =@accountno 
  and isnull(depositmaster.Disabled,0) =0 ) balq )
return @balance
end 

go

CREATE  or alter procedure [dbo].[balancewithfullacno](@accountno nvarchar(30))
as 
begin
  select depositmaster.DepositType as SavingName, Depositmaster.mainbookno,depositmaster.accountno as acno1, DepositMaster.Operator1 as AccountHolder,
  Left(depositmaster.mainbookno,3)+right(Depositmaster.mainbookno,2) + depositmaster.accountno as accountno,
  memberdetail.MemName,
  case when isNUll(DepositMaster.InterestRate,0)=0 then isnull(savings.RateOfInterest,0) else DepositMaster.InterestRate end as interestrate, 
  Case when isnull(depositmaster.MinimumBalance,0)<>0 then isnull(depositmaster.MinimumBalance,0)else savings.MinBal end as minBal, 
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

GO

if not exists (select * from Official where Item='GenerateJournalNo')
Begin
insert into Official  values ('GenerateJournalNo','1', '') 
end

GO

--#region check balance side of mainaccount
delete from MainAccount
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'1', N'SHARES', N'010', N'LIABILITY', N'All share balance', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'z]o/', N'SHARE', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'शेयर', N'शेयर पूंजी हिसाब खाता', N'१')
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'2', N'RESERVES', N'020', N'LIABILITY', N'All reserves balance', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'hu]*f sf]if', N'RESERVE', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'कोष', N' जगेडा तथा अन्य कोष हिसाब खाता', N'२')
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'3', N'DEPOSITS', N'030', N'LIABILITY', N'Various types of savings', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'lgIf]k', N'DEPOSIT', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'बचत', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'4', N'CREDITORS', N'040', N'LIABILITY', N'Varios types of Creditors we have to pay them', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'afx\o C)f', N'OUT LOAN', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'बाह्य ऋण', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'5', N'SUBSIDIES', N'050', N'LIABILITY', N'Various type of donations received', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'cg''bfg', N'SUBSIDIES', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'अनुदान', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'6', N'PAYABLES', N'060', N'LIABILITY', N'Outstanding payables', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'e''QmfgL lbg''kg]{', N'PAYABLES', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'भुक्तानी दिनुपर्ने ', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'7', N'OTHER LIABILITIES', N'070', N'LIABILITY', N'Other types of liabilities wich is not mentioned above', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'cGo bfloTj', N'O.LIABILITIES', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'अन्य दायित्व', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'8', N'CASH', N'080', N'ASSET', N'Various cash receipts', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'gub', N'CASH', N'CASH', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'नगद', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'9', N'BANKS', N'090', N'ASSET', N'Bank accounts of the cooperatives', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'a}+s', N'BANK', N'CASH', NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'बैंक', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'10', N'INVESTMENTS', N'100', N'ASSET', N'General Investments', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'nufgL', N'INVSTMNT', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'लगानी', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'11', N'LOANS', N'110', N'ASSET', N'Loan distributed to the members', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'C)f', N'LOAN', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'ऋण', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'12', N'RECEIVABLES', N'120', N'ASSET', N'Various outstanding receivables', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'kfpg''kg]{', N'RECVBLE', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'पाउनु पर्ने', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'13', N'FIXED ASSETS', N'130', N'ASSET', N'various types of physical assets', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N':yL/ ;DklQ', N'FIXED ASTS', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'स्थायी सम्पत्ति', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'14', N'OTHER ASSETS', N'140', N'ASSET', N'Other Assets which are not mentioned', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'cGo ;DklQ', N'OTHER ASTS', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'अन्य सम्पत्ति', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'15', N'EXPENDITURES', N'150', N'EXPENDITURE', N'Various types of expenditures', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'vr{', N'EXPENDITURE', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'खर्च', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'16', N'INCOMES', N'160', N'INCOME', N'income entries', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'cfDbfgL', N'INCOMES', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'आम्दानी', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'17', N'PREBALS', N'170', N'PREBAL', N'Previous Balance Maintain', CAST(N'2003-01-15T00:00:00' AS SmallDateTime), N'k''/fgf] Aofn]G;', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'अ. ल्या.', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'18', N'LIABILITY SUSP', N'180', N'SUSPENSE', N'Suspense account for interest receivable is created.', CAST(N'2003-06-26T00:00:00' AS SmallDateTime), N'ph|ftL bfloTj', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'संकास्पद दायित्व', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'19', N'ASSETS SUSP', N'190', N'SUSPENSE', N'Suspense account for interest receivable is created. 
', CAST(N'2003-06-26T00:00:00' AS SmallDateTime), N'ph|ftL ;DklQ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'संकास्पद सम्पत्ति', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'20', N'EXPENDITURE SUSP', N'200', N'SUSPENSE', N'CAPITALIZED INTEREST', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'ph|ftL vr{', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'संकास्पद खर्च', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'21', N'INCOME SUSP', N'210', N'SUSPENSE', N'BALANCE TRANSFER SUSPENSE', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'ph|ftL cfDbfgL', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'संकास्पद आम्दानी', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'22', N'BALANCE SUSP', N'220', N'SUSPENSE', N'BALANCE TRANSFER SUSPENSE', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'ph|ftL af+sL', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'संकास्पद अ. ल्या.', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'23', N'INTEROFFICE PAYABLE', N'230', N'INTEROFFICE TRANSCR', N'IOT PAYABLE', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'cGt/sfof{no e''QfgL lbg''kg]{', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'अन्तरशाखा कारोबार दायित्व', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'24', N'INTEROFFICE RECEIVABLE', N'240', N'INTEROFFICE TRANSDR', N'IOT RECEIVABLE', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'cGt/sfof{no kfpg'' kg]{', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, N'अन्तरशाखा कारोबार सम्पत्ती', NULL, NULL)
GO
INSERT [dbo].[MainAccount] ([MAID], [MANAME], [ACNO], [SANAME], [MADESC], [FormedDate], [ManameNepali], [ManameShort], [ACTYPE], [ManameShortNep], [SuperAccount], [EffectOn], [EntrySide], [GroupTitle], [AccountType], [Typeno], [BalanceSide], [UnManame], [CopomisManame], [CoopmisAnex]) VALUES (N'25', N'PROFIT AND  LOSS', N'250', N'PROFIT AND LOSS', N'PROFIT OR LOSS', CAST(N'2004-03-25T00:00:00' AS SmallDateTime), N'gfkmf gf]S;fg vftf ', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, -1, N'नाफा नोक्सान हिसाब', NULL, NULL)
GO


--- check date alert


create or ALTER trigger [dbo].[checkDateInUpDel] on [dbo].[MainTransBook] 
for  INSERT, UPDATE, DELETE 
AS
declare @muser nvarchar(50)
declare @transdate datetime  
declare @olddate as datetime 
declare @enteredBy nvarchar(50)
declare @isAdmin bit
declare @checkLimit bit
declare @transLimit decimal(18,4)
declare @itemName  as nvarchar(50)
declare @isCollTrans nvarchar(50)
declare @tdate datetime
declare @tOpen bit
declare @approvedby nvarchar(100)
declare @transactionAmount decimal(18,4)
declare @transNOA int
select  @olddate = max (transdate) from deleted 
select  @transdate= max (transdate) from inserted 
select  @isCollTrans =max(Remarks4) from inserted
select  @transactionAmount =max(debit) from inserted
select  @TransnoA = max(transnoa) from inserted
select @muser = max(Enteredby) from inserted
select @itemName =  max(itemname) from deleted
select @tdate= case max(isnull(itemdata,0)) when 1 then (select max(CONVERT(char(12), Transactiondate, 9)) from coopadmin.dbo. transdate)  
else CONVERT(char(12), GETDATE(), 9)  end  from coopadmin.dbo.off2 where item='DayEndSystem' 
select @checkLimit = case (isnull(itemdata,0)) when '1' then 1 else 0 end from Coopadmin.dbo.off2 where item='CheckLimit'
Select @transLimit  = isnull(transactionLimit,0) from coopadmin.dbo.coopusers where username = @muser
select @isAdmin= [Administrator] from coopadmin.dbo.coopusers where username=@muser
select @olddate= isnull(@olddate,@tdate)
select @transdate=isnull(@transdate,@tdate)
select @approvedby = ISNULL(Approvedby, '') from deleted 
--select transdate,current from TransactionDates  
if (@oldDate in (select TransDate from transactiondates where transdate = @olddate  and currentState='Active') and  
	@TransDate  in (select TransDate from transactiondates where transdate =@transdate  and currentState='Active'))
begin 
	set @tOpen = 1 
end 
else 
begin
	set @topen = 0 
end 
if(@muser in ('mofin','moblie','ismart','mbank'))
begin
set @tOpen=1
end
IF len(@approvedby)>0 
BEGIN 
	RAISERROR('You can not edit/delete this transaction...',16,1)
	rollback transaction 
end 
--if not @isAdmin =1 
--begin 
if (not @tOpen=1) -- (@transdate= @tdate) and (@olddate=@tdate))     
   begin 
	   RAISERROR ('DB Error: Could not commit, Transaction date is not active or not opened!',
	      16, 1)
	   ROLLBACK TRANSACTION
   end 
--end
go
alter table maintransbook 
alter column BVRCNO nvarchar(150)
alter table maintransbook
alter column Remarks1 nvarchar(255)
alter table maintransbook
alter column Particulars nvarchar(255)
alter table maintransbook
alter column Remarks2 nvarchar(255)
alter table maintransbook
alter column Remarks3 nvarchar(255)
alter table maintransbook
alter column TransactionType nvarchar(255)
alter table transone
alter column TransactionType nvarchar(255)

go


INSERT INTO [dbo].[ACCOUNTS]
           ([ACNAME]
           ,[ACNO]
           ,[MANAME]
           ,[MANO]
           ,[SANAME]
           ,[DESCRIPTION]
           ,[FORMEDDATE])
		   values
		    ('ISMART BANKING ACCOUNT', '120.40', 'RECEIVABLES','120','ASSET','MOBILE BANKING',GETDATE()),
		   ('ISMART PARKING POOLING', '060.40', 'PAYABLES','060','LIABILITY','MOBILE BANKING',GETDATE()),
		   ('ISMART MOBILE BANKING', '160.40', 'INCOMES','160','INCOME','MOBILE BANKING',GETDATE())

		   INSERT INTO [dbo].[ItemMaster]
           ([ItemCode]
           ,[ItemName]
           ,[NepaliName]
           ,[ItemLocation]
           ,[Saname]
           ,[Maname]
           ,[Acname]
           ,[ACNO]
           ,[MANO]
           ,[Description]    
           ,[formedDate]
           ,[TransDate]
           ,[User]
		   ,[BranchID])
		   Values
		   ('I001','ISMART UTILITY POOL/PARKING','','','LIABILITY','PAYABLES','ISMART PARKING POOLING','060.40','060','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
		   ('I002','BANK TRANSFER POOL/PARKING','','','LIABILITY','PAYABLES','ISMART PARKING POOLING','060.40','060','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
		   ('I003','CELLPAY PARKING','','','LIABILITY','PAYABLES','ISMART PARKING POOLING','060.40','060','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
		   ('I004','NLOAD PARKING','','','LIABILITY','PAYABLES','ISMART PARKING POOLING','060.40','060','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
		   ('I001','ISMART UTILITY OPERATOR','','','ASSET','RECEIVABLES','ISMART BANKING ACCOUNT','120.40','120','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
		   ('I002','NLOAD FROM BANK','','','ASSET','RECEIVABLES','ISMART BANKING ACCOUNT','120.40','120','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
			('I003','CELLPAY  OPERATOR','','','ASSET','RECEIVABLES','ISMART BANKING ACCOUNT','120.40','120','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
			('MI001','MOBILE BANKING REGISTRATION','','','INCOME','INCOMES','ISMART MOBILE BANKING','160.40','160','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01'),
			('MI002','MOBILE BANKING CHARGES','','','INCOME','INCOMES','ISMART MOBILE BANKING','160.40','160','MOBILE BANKING', GETDATE(),GETDATE(),'Oxpan','01')
