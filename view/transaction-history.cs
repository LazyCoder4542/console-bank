using bank_app.schema;
using static bank_app.Utils;

namespace bank_app.view;

public class TransactionHistory : IView
{
  private BankApp _app;

  private List<Transaction> transactions = [];
  private int read = 0;

  private const int maxRows = 25;

  private bool showAll = false;

  public TransactionHistory(BankApp app)
  {
    _app = app;
  }

  public void GenerateScreenContent()
  {
    if (_app.ProtectScreen())
      return;
    transactions = _app.GetTransactions().OrderByDescending(t => t.Id).ToList();
    Console.Clear();
    Console.WriteLine("====================================");
    Console.WriteLine("       TRANSACTION HISTORY         ");
    Console.WriteLine("====================================");

    if (transactions.Count == 0)
    {
      Console.WriteLine("\nNo transactions found.");
    }
    else
    {
      Console.WriteLine(
        $"\n{"Date".Align(20)}\t{"Type".Align(10)}\t{"Amount".Align(20, Alignment.Right)}\t{"Description".Align(20)}\t{"From/To".Align(20, Alignment.Right)}"
      );
      Console.WriteLine(new string('-', 90 + 4 * 4));
      for (int i = 0; i < Math.Ceiling((double)transactions.Count / maxRows); i++)
      {
        while (read < Math.Min(transactions.Count, maxRows * (i + 1)))
        {
          Transaction transaction = transactions[read];
          bool isTo = _app.CurrentAccount!.AccountNumber == transaction.ToAccount;
          Func<string, string> accName = (x) =>
            x.Length == 0
              ? "CONSOLEBANK/" + transaction.Type.ToString().ToUpper()
              : _app.GetAccountName(x);
          string ToFrom = accName(isTo ? transaction.FromAccount : transaction.ToAccount);

          Console.Write(
            $"{$"{transaction.Timestamp:G}".Align(20)}\t{transaction.Type.ToString().Align(10)}\t"
          );
          Console.ForegroundColor = isTo ? ConsoleColor.Green : ConsoleColor.Red;
          Console.Write($"{$"{transaction.Amount:C}".Align(20, Alignment.Right)}\t");
          Console.ResetColor();
          Console.Write($"{transaction.Description.Align(20)}\t");
          Console.Write($"{ToFrom.Align(20, Alignment.Left)}");
          Console.WriteLine();
          read++;
        }
        if (!showAll && read < transactions.Count)
        {
          Console.WriteLine(
            $"<Next-{Math.Min(maxRows, transactions.Count - read)}-transaction(s)>".Align(
              90 + 4 * 4,
              Alignment.Center,
              '-'
            )
          );
          var key = Console.ReadKey(intercept: true).Key;

          while (key != ConsoleKey.Enter && key != ConsoleKey.Spacebar)
          {
            key = Console.ReadKey(intercept: true).Key;
          }

          ClearLine(1);
          // Console.WriteLine(new string(' ', Console.WindowWidth));
          // Console.SetCursorPosition(0, Console.CursorTop);

          if (key == ConsoleKey.Spacebar)
          {
            showAll = true;
          }
        }
      }
    }

    Console.WriteLine("\n\nPress Enter");
  }

  public IView? OnInput(string input)
  {
    return new MainMenu(_app);
  }
}
