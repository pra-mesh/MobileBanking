namespace MobileBanking.Data.Models.DTOs;
public class TenantModel
{
    public required string ServerIP { get; init; }
    public required string CurrentDB { get; init; }
    public required string ApiKey { get; init; }
    public required string ClientName { get; init; }
    public required string ClinetName { get; init; }
}
