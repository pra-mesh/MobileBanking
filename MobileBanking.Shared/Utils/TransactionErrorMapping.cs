namespace MobileBanking.Shared.Utils;
public class TransactionErrorMapping
{
    private static readonly Dictionary<(string, string), Func<string, Exception>> exceptionMap =
            new Dictionary<(string, string), Func<string, Exception>>
                {
                { ("destination", "multiple"), acc => new MultipleAccountsFoundException(acc) },
                { ("destination", "Not found"), acc => new AccountNotFoundException(acc) },
                { ("destination", "insufficient"), acc => new InsufficientBalanceException(acc) },
                { ("source", "multiple"), acc => new MultipleAccountsFoundException(acc) },
                { ("source", "Not found"), acc => new AccountNotFoundException(acc) },
                { ("source", "insufficient"), acc => new InsufficientBalanceException(acc) }
                };
    public static void GetError(string msg, string destaccount, string srcaccount)
    {
        foreach (var key in exceptionMap.Keys)
        {
            if (msg.Contains(key.Item1) && msg.Contains(key.Item2))
            {
                var account = key.Item1 == "destination" ? destaccount : srcaccount;
                throw exceptionMap[key](account);
            }
        }
        throw new Exception(msg);
    }

}
