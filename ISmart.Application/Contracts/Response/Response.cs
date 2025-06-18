namespace MobileBanking.Application.Contracts.Response;
public class BaseResponse<T>
{
    public string statusCode { get; set; } = "96";
    public required T data { get; set; }

}
