public class AccountNotFoundException : Exception
{
    public AccountNotFoundException(string accountNo) :
        base($"Invalid Account [No account was found for the account number: {accountNo}]")
    { }
}

public class MultipleAccountsFoundException : Exception
{
    public MultipleAccountsFoundException(string accountNo) :
        base($"Invalid Account [Multiple accounts found for the account number:{accountNo}]")
    { }
}

public class InsufficientBalanceException : Exception
{
    public InsufficientBalanceException(string accountNo) : base($"Insufficient Fund [{accountNo} doesn't have sufficent fund.]")
    { }
}
public class InvalidAccountException : Exception
{
    public InvalidAccountException(string accountNo) : base($"Invalid Account [Invalid accound format: {accountNo}]")
    {

    }

}

//public class InsufficientBalance : Exception
//{
//    public InsufficientBalance(string accountNo)
//        : base(GenerateMessage(accountNo)) { }

//    private static string GenerateMessage(string accountNo)
//    {
//        if (accountNo.StartsWith("VIP"))
//            return $"Account [{accountNo}] requires special handling due to insufficient balance.";
//        else if (accountNo == "000123")
//            return $"Account [{accountNo}] is flagged. Please contact support due to insufficient funds.";
//        else
//            return $"Insufficient Fund: Account [{accountNo}] does not have sufficient balance.";
//    }
//}