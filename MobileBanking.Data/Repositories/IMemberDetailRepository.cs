using MobileBanking.Data.Models.DTOs;

namespace MobileBanking.Data.Repositories;
public interface IMemberDetailRepository
{
    Task<List<ShareDTO>> ShareList(string memberNO);
}
