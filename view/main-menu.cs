using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class MainMenu : IView
{
  private BankApp _app;
  private User? currentUser;
  private Account? currentAccount;

  public MainMenu(BankApp app)
  {
    _app = app;
    currentUser = app.CurrentUser;
    currentAccount = app.CurrentAccount;
  }

  public void GenerateScreenContent()
  {
    if (_app.ProtectScreen())
      return;

    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine($"    WELCOME BACK, {currentUser!.Name.ToUpper()}    ");
    Console.WriteLine("====================================");
    Console.WriteLine("\n1. View Account Balance");
    Console.WriteLine("2. Deposit Money");
    Console.WriteLine("3. Withdraw Money");
    Console.WriteLine("4. Transfer Funds");
    Console.WriteLine("5. View Transaction History");
    Console.WriteLine("6. Account Settings");
    Console.WriteLine("7. Logout");
    Console.Write("\nPlease enter your choice (1-7): ");
  }

  public IView? OnInput(string input)
  {
    if (currentAccount is null || currentUser is null)
    {
      return new Landing(_app);
    }

    ClearLine(1);

    if (int.TryParse(input, out int number) & number >= 1 & number <= 7)
    {
      // Success - use 'number' variable
      Console.WriteLine($"Received {number}");

      if (number == 1)
      {
        return new AccountBalance(_app);
      }
      if (number == 2)
      {
        return new DepositMoney(_app);
      }
      if (number == 3)
      {
        return new WithdrawMoney(_app);
      }
      if (number == 4)
      {
        return new TransferFund(_app);
      }
      if (number == 5)
      {
        return new TransactionHistory(_app);
      }
      if (number == 7)
      {
        _app.Logout();
        return null;
      }
    }

    GenerateScreenContent();

    return null;
  }
}
