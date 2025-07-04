using bank_app.exceptions;
using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class Login : IView
{
  private BankApp _app;
  private string username = string.Empty;

  private bool isAuthenticated = false;

  private int progress = 0;
  private readonly int maxProgress = 2;

  public Login(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("            LOGIN SCREEN            ");
    Console.WriteLine("====================================");
    DisplayPrompt();
  }

  private void DisplayPrompt()
  {
    switch (progress)
    {
      case 0:
        Console.Write("\nEnter your username: ");
        break;
      case 1:
        while (true)
        {
          try
          {
            using (var password = GetPasswordFromConsole("\nEnter a password: "))
            {
              _app.Login(username, password);
              isAuthenticated = true;
              break; // Exit loop on success
            }
          }
          catch (InvalidOperationException ex) when (ex.Message.Contains("empty"))
          {
            Console.WriteLine("\nPassword cannot be empty. Please try again.");
          }
          catch (LoginInvalidCredentialsException)
          {
            break;
          }
        }
        progress++;
        GenerateScreenContent();
        if (isAuthenticated)
        {
          Console.WriteLine("\nLogin successful, 'Enter' to continue");
        }
        else
        {
          Console.WriteLine("\nInvalid login credentials! 'Enter' to retry");
        }

        break;
    }
  }

  private void Read(string input)
  {
    switch (progress)
    {
      case 0:
        username = input;
        progress++;
        break;
    }
  }

  private void Reset()
  {
    username = string.Empty;
    isAuthenticated = false;
    progress = 0;
  }

  public IView? OnInput(string input)
  {
    ClearLine(1);
    if (int.TryParse(input, out int number) & number == -1)
    {
      // Success - use 'number' variable
      Console.WriteLine($"Received {number}");

      if (number == 2)
      {
        return new CreateAccount(_app);
      }
    }
    else
    {
      Read(input);
    }

    if (progress == maxProgress)
    {
      if (isAuthenticated)
      {
        return new MainMenu(_app);
      }
      else
      {
        Reset();
      }
    }
    GenerateScreenContent();

    return null;
  }
}
