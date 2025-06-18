namespace MobileBanking.Models.Response;
public class BaseResponse<T>
{
    public string statusCode { get; init; } = "96";
    public required T data { get; init; }

}
