using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class WithdrawMoney : IView
{
  private BankApp _app;
  private decimal amount = 0;
  private string desc = string.Empty;

  private int progress = 0;
  private readonly int maxProgress = 2;

  public WithdrawMoney(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    if (_app.ProtectScreen())
      return;
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("           WITHDRAW MONEY           ");
    Console.WriteLine("====================================");
    Console.WriteLine($"Current balance: {_app.CurrentAccount!.Balance:C}");

    Console.WriteLine("Minimum Withdrawal is ${0}", BankApp.MIN_WITHDRAW);

    Console.WriteLine("\nExit: Enter -1");

    DisplayPrompt();
  }

  private void DisplayPrompt()
  {
    switch (progress)
    {
      case 0:
        Console.Write("\nEnter amount to withdraw: $");
        break;
      case 1:
        Console.Write("\nEnter description(optional): ");
        break;
    }
  }

  private void Read(string input)
  {
    switch (progress)
    {
      case 0:
        if (int.TryParse(input, out int number) & number >= BankApp.MIN_WITHDRAW)
        {
          amount = number;
          progress++;
        }
        else
        {
          Console.WriteLine("Invalid amount. Please enter at least the minimum amount");
        }
        break;
      case 1:
        desc = input;
        progress++;
        break;
    }
  }

  public IView? OnInput(string input)
  {
    ClearLine(1);

    if (int.TryParse(input, out int number) && number == -1)
    {
      return new MainMenu(_app);
    }
    else
    {
      Read(input);
    }

    if (progress == maxProgress)
    {
      _app.MakeWithdrawal(amount, desc);
      Console.WriteLine("\nWithdrawal successful");
      Console.Write("Press 'any' key to go back: ");
      Console.ReadKey();
      return new MainMenu(_app);
    }

    GenerateScreenContent();

    return null;
  }
}
