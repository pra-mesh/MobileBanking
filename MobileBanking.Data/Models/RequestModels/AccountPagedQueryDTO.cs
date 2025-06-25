namespace MobileBanking.Data.Models.RequestModels;
public class AccountPagedQueryDTO:AccountQueryDTO
{
    public int Offset { get; set; } = 0;
    public int Limit { get; set; } = 1;
}
