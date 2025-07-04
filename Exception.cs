namespace bank_app.exceptions;

// Domain-specific exceptions
public class LoginInvalidCredentialsException()
  : ApplicationException("Invalid Login credentials") { }

public class UsernameInUseException(string username)
  : ApplicationException($"Username '{username}' is already in use")
{
  public string RequestedUsername = username;
}

public class InvalidAccountNumberException(string accountNumber)
  : ApplicationException($"No Account with Account number '{accountNumber}'")
{
  public string AccountNumber = accountNumber;
}
