using MobileBanking.Data.Models.DTOs;
using MobileBanking.Data.Services.Connection;

namespace MobileBanking.Data.Repositories;
public class MemberDetailRepository : IMemberDetailRepository
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public MemberDetailRepository(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }
    public async Task<List<ShareDTO>> ShareList(string memberNO)
    {
        string q = @"select ItemCode as membercode ,case when issueddate is null then '' else
                dbo.engToNep(issueddate) end as openDate,isNUll(certino,'') kittaNumber,ShareAmount from ShareInMember s 
                left join sharebookcombined sb on s.ItemCode=sb.memberno and s.CertiFicateNo = sb.certino
                where s.ItemCode=@memberNO";
        var p = new { memberNO };
        List<ShareDTO> shares = await _sqlDataAccess.LoadDataQuery<ShareDTO, dynamic>(q, p);
        return shares;

    }
}
