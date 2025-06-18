public class AccountNotFoundException : Exception
{
    public AccountNotFoundException() : base("Invalid Account:[No account found for the provided account number.]") { }
}

public class MultipleAccountsFoundException : Exception
{
    public MultipleAccountsFoundException() : base("Invalid Account: [Multiple accounts found for the given account number.]") { }
}