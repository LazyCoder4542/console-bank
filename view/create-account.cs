using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class CreateAccount : IView
{
  private BankApp _app;
  private string name = string.Empty;
  private string email = string.Empty;
  private string username = string.Empty;
  private string hash = string.Empty;
  private string salt = string.Empty;
  private decimal initialDeposit = 0;
  private int progress = 0;
  private readonly int maxProgress = 5;

  public CreateAccount(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("        CREATE NEW ACCOUNT         ");
    Console.WriteLine("====================================");
    Console.WriteLine("\n1. Back");
    Console.WriteLine("2. Restart process");
    DisplayPrompt();
  }

  private void DisplayPrompt()
  {
    switch (progress)
    {
      case 0:
        Console.Write("\nEnter your name: ");
        break;
      case 1:
        Console.Write("\nEnter your email address: ");
        break;
      case 2:
        Console.Write("\nCreate a username: ");
        break;
      case 3:
        while (true)
        {
          try
          {
            using (var password = GetPasswordFromConsole("\nCreate a password: "))
            {
              Console.WriteLine("\nPassword accepted!");
              hash = SecureStringHasher.HashSecureString(password, out salt);

              break; // Exit loop on success
            }
          }
          catch (InvalidOperationException ex) when (ex.Message.Contains("empty"))
          {
            Console.WriteLine("\nPassword cannot be empty. Please try again.");
          }
        }
        progress++;
        GenerateScreenContent();

        break;
      case 4:
        Console.Write("\nInitial deposit amount: $");
        break;
    }
  }

  private void Read(string input)
  {
    switch (progress)
    {
      case 0:
        name = input;
        progress++;
        break;
      case 1:
        email = input;
        progress++;
        break;
      case 2:
        username = input;
        progress++;
        break;
      case 4:
        if (decimal.TryParse(input, out decimal number) & number >= 0)
        {
          initialDeposit = number;
          progress++;
        }
        else
        {
          Console.WriteLine("Invalid amount. Please enter a positive number");
        }
        break;
    }
  }

  private void Reset()
  {
    name = string.Empty;
    email = string.Empty;
    username = string.Empty;
    hash = string.Empty;
    salt = string.Empty;
    initialDeposit = 0;
    progress = 0;
  }

  public IView? OnInput(string input)
  {
    ClearLine(1);

    if (int.TryParse(input, out int number) & number >= 1 & number <= 2)
    {
      // Success - use 'number' variable
      Console.WriteLine($"Received {number}");

      if (number == 1)
      {
        return new Landing(_app);
      }
      if (number == 2)
      {
        Reset();
      }
    }
    else
    {
      Read(input);
    }

    if (progress == maxProgress)
    {
      Console.WriteLine();
      try
      {
        string accountNo = _app.CreateUser(name, email, username, hash, salt, initialDeposit);
        Console.WriteLine("\nAccount created successfully!");
        Console.WriteLine($"Account Number: {accountNo}");
      }
      catch
      {
        Console.WriteLine("Oops! Something went wrong.");
      }

      Console.WriteLine("\nPress any key to return to login...");
      Console.ReadKey();

      return new Login(_app);
    }
    else
    {
      GenerateScreenContent();
    }

    return null;
  }
}
