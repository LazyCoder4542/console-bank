using static bank_app.Utils;

namespace bank_app.view;

public class Landing : IView
{
  private BankApp _app;

  internal Landing(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    Console.Clear();
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("      WELCOME TO CONSOLE BANK       ");
    Console.WriteLine("====================================");
    Console.WriteLine("\n1. Login to your account");
    Console.WriteLine("2. Create a new account");
    Console.WriteLine("3. Exit\n");
    Console.Write("Please enter your choice (1-3): ");
  }

  public IView? OnInput(string input)
  {
    ClearLine(1);
    if (int.TryParse(input, out int number) & number >= 1 & number <= 3)
    {
      // Success - use 'number' variable
      Console.WriteLine($"Received {number}");

      if (number == 1)
      {
        return new Login(_app);
      }

      if (number == 2)
      {
        return new CreateAccount(_app);
      }
    }
    else
    {
      // Handle conversion failure
      Console.WriteLine("Invalid input");
    }

    Console.Write("Please enter your choice (1-3): ");

    return null;
  }
}
