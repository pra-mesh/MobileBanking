namespace MobileBanking.Application.AuthService;
public interface IApiKeyValidation
{
    bool IsValidAPIKey(string userApiKey);
}
