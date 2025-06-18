
namespace MobileBanking.Application.Services;

public interface IAccountValidation
{
    Task IsSingleAccount(string accountNo);
}
