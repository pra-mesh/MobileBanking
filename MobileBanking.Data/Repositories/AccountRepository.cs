using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data.Repositories;
public class AccountRepository : IAccountRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public AccountRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public async Task<List<AccountDetailDTO>> GetAccountDetails(string accountNo)
    {
        return await _sqlDataAccess.LoadData<AccountDetailDTO, dynamic>("balancewithfullacno", new { accountNo });
    }
    public async Task<int> AccountCount(string accountNo) => await
        _sqlDataAccess.SingleDataQuery<int, dynamic>
        ("select count(itemcode) from itms1 where REPLACE(acno,'.','')+ITEMCODE =@accountno", new { accountNo });

    public async Task<decimal> GetBalance(string accountNo) =>
        await _sqlDataAccess.SingleDataQuery<decimal, dynamic>
        ("Select IsNUll((select Balance from ItemBal where REPLACE(acno,'.','')+ITEMCODE =@accountNo),0) as Balance", new { accountNo });

    public async Task<decimal> GetOpeningBalance(string accountNo, DateTime date) =>
        await _sqlDataAccess.SingleDataQuery<decimal, dynamic>
        (@"Select isNull(sum(credit)-sum(debit),0)  as Balance from Maintransbook where left(Maintransbook.acno,3) 
                    + right(Maintransbook.acno,2) + maintransbook.itemcode = @accountNO and transdate <@date",
            new
            {
                accountNo,
                date
            });
    public async Task<string> GetAccountBranch(string accountNO) =>
        await _sqlDataAccess.SingleDataQuery<string, dynamic>
        ("select top 1 branchId from Itms1 where REPLACE(acno,'.','')+Itemcode =@accountNO", new { accountNO });

    public async Task<string> GetItemName(string fullAccountNo) => await _sqlDataAccess.SingleDataQuery<string, dynamic>
          ("select top 1 ITEMNAME from Itms1 where REPLACE(acno,'.','')" +
          "+ITEMCODE =@fullAccountNo",
          new { fullAccountNo });
    public async Task<decimal> GetDepBalance(string accountNo) =>
       await _sqlDataAccess.SingleDataQuery<decimal, dynamic>
       ("Select IsNUll((SELECT [dbo].[DepositBalance](@accountno)),0) as Balance", new { accountNo });

}
