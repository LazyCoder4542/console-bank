using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class AccountBalance : IView
{
  private BankApp _app;

  public AccountBalance(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    if (_app.ProtectScreen())
      return;
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("        ACCOUNT BALANCE            ");
    Console.WriteLine("====================================");
    Console.WriteLine($"\nYour current balance: {_app.CurrentAccount!.Balance:C}");
    Console.WriteLine("\nPress any key to return to main menu...");
    Console.ReadKey();
  }

  public IView? OnInput(string input)
  {
    return new MainMenu(_app);
  }
}
