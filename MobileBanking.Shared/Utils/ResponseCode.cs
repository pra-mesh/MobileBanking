namespace MobileBanking.Shared.Utils;
public class ResponseCode
{
    private static readonly Dictionary<string, string> ResStatusCode = new()
    {
        { "Unable To Process", "05" },
        { "Transaction Not Allowed", "39" },
        { "Insufficient Fund", "51" },
        { "Account Dormant", "52" },
        { "Account Closed", "54" },
        { "Account Restricted", "62" },
        { "Invalid Account", "76" },
        { "System Error", "96" },
        { "Duplicate Reversal", "98" },
        { "Format Error", "30" },
        { "Success", "00" }
    };
    public static string GetResponseCode(Exception ex)
    {
        foreach (var key in ResStatusCode.Keys)
        {
            if (ex.Message.Contains(key))
            {
                return ResStatusCode[key];
            }
        }
        return ResStatusCode["System Error"]; // Default case
    }
}
