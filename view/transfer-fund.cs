using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class TransferFund : IView
{
  private BankApp _app;
  private string toAcc = string.Empty;
  private string accName = string.Empty;
  private decimal amount = 0;
  private string desc = string.Empty;

  private int progress = 0;
  private readonly int maxProgress = 3;

  public TransferFund(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    if (_app.ProtectScreen())
      return;
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("           TRANSFER FUNDS           ");
    Console.WriteLine("====================================");
    Console.WriteLine("Minimum Transfer is ${0}", BankApp.MIN_TRANSFER);

    Console.WriteLine("\nExit: Enter -1");

    DisplayPrompt();
  }

  private void DisplayPrompt()
  {
    switch (progress)
    {
      case 0:
        Console.Write("\nEnter account number of recipient: ");
        break;
      case 1:
        Console.Write("\nEnter amount to transfer: $");
        break;
      case 2:
        Console.Write("\nEnter description(optional): ");
        break;
    }
  }

  private void Read(string input)
  {
    switch (progress)
    {
      case 0:
        toAcc = input;
        string name = _app.GetAccountName(toAcc);
        if (name is null)
        {
          Console.Write("Account not found: Press any key to Try again");
        }
        else
        {
          accName = name;
          Console.WriteLine($"Account name: {name}");
          Console.Write("Press any key to continue");
          progress++;
        }
        Console.ReadKey();
        break;
      case 1:
        if (decimal.TryParse(input, out decimal number) & number >= BankApp.MIN_TRANSFER)
        {
          if (_app.CurrentAccount!.Balance >= number)
          {
            amount = number;
            progress++;
          }
          Console.WriteLine("Insufficient funds. Please enter a smaller amount: ");
        }
        else
        {
          Console.WriteLine("Invalid amount. Please enter at least the minimum amount");
        }
        break;
      case 2:
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
      Console.Write($"Are you sure you want to transfer {amount:C} to {accName}: Yes/(No)");
      string? res = Console.ReadLine();
      if (res is not null && (res.Equals("y") || res.Equals("yes")))
      {
        _app.MakeTransfer(amount, desc, toAcc);
        Console.WriteLine("\nTransfer successful");
        Console.Write("Press 'any' key to go back: ");
        Console.ReadKey();
        return new MainMenu(_app);
      }
      progress = 0;
    }

    GenerateScreenContent();

    return null;
  }
}
