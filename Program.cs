using System.Security;
using bank_app.exceptions;
using bank_app.schema;
using bank_app.view;
using static bank_app.Utils;

// See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");

namespace bank_app;

public static class Program
{
  private static BankApp ConsoleBank = new();

  // private static IView create_acc = new

  public static void Main(string[] args)
  {
    ConsoleBank.Build();

    // Console.Write("fgfjhkgktktktkytyyrthrgrgthyj");
    // Console.SetCursorPosition(0, Console.CursorTop);
    // Console.Write(new string(' ', Console.WindowWidth));
    // Console.SetCursorPosition(0, Console.CursorTop);
    // Console.WriteLine("Hello");

    // TestDB();

    // var schema = SchemaGenerator.GenerateSchema<Account>();
    // foreach (var P in schema)
    // {
    //   Type Type;
    //   bool IsRequired;
    //   int MaxLength;

    //   (Type, IsRequired, MaxLength) = P.Value;

    //   Console.WriteLine(
    //     $"{P.Key,-20} Type {Type} - IsRequired {IsRequired} - MaxLength {MaxLength}"
    //   );
    // }
  }

  // private static DB InitDB()
  // {
  //  return new DB("data/");
  // }

  private static void TestDB()
  {
    DB dbContext = new DB("data/");
    User getUser = dbContext.Users.Find(1);
    Console.WriteLine(getUser.Id);
    Console.WriteLine(getUser.Name);
    Console.WriteLine(getUser.Email);
    Console.WriteLine(getUser.Username);
    Console.WriteLine(getUser.PasswordHash);
    Console.WriteLine(getUser.AccountId);

    // These will get independent IDs
    // var userId = dbContext.Users.Add(
    //   new User
    //   {
    //     // Id = 1,
    //     Name = "Alice",
    //     Email = "test",
    //     Username = "test",
    //     HashPassword = "test",
    //     AccountId = 0,
    //   }
    // ); // Might get ID 1
    // dbContext.Users.Add(
    //   new User
    //   {
    //     // Id = 1,
    //     Name = "Alice",
    //     Email = "test",
    //     Username = "test",
    //     HashPassword = "test",
    //     AccountId = 0,
    //   }
    // ); // Might get ID 2
    // Console.WriteLine(userId);

    // Next additions will increment separately
    // Will get ID 2
    // var anotherUser = dbContext.Users.Add(new User { Name = "Bob" });

    // dbContext.Transactions.Add(
    //   new Transaction
    //   {
    //     Type = TransactionType.Transfer,
    //     Amount = 1000,
    //     Description = "initial deposit",
    //     Timestamp = DateTime.Now,
    //     FromAccount = 1,
    //     ToAccount = 2,
    //   }
    // );

    decimal x = 1000;
    Console.WriteLine(string.Join(", ", decimal.GetBits(x).Select(b => b.ToString())));

    Transaction getTransaction = dbContext.Transactions.Find(1);
    Console.WriteLine(getTransaction.Id);
    Console.WriteLine(getTransaction.Type);
    Console.WriteLine(getTransaction.Amount);
    Console.WriteLine(getTransaction.Description);
    Console.WriteLine(getTransaction.Timestamp);
    Console.WriteLine(getTransaction.FromAccount);
    Console.WriteLine(getTransaction.ToAccount);
  }
}

public class BankApp
{
  private Screen _screen;

  private DB _db = new("data/");
  private User? currentUser;
  private Account? currentAccount;

  public const int MIN_DEPOSIT = 10;
  public const int MIN_TRANSFER = 1;

  public const int MIN_WITHDRAW = 2;

  public BankApp()
  {
    _screen = new(new Landing(this));
  }

  public User? CurrentUser
  {
    get { return currentUser; }
  }
  public Account? CurrentAccount
  {
    get { return currentAccount; }
  }

  internal string CreateUser(
    string name,
    string email,
    string username,
    string hash,
    string salt,
    decimal initialDeposit
  )
  {
    string accountNumber = string.Empty;
    int accountId = _db.Accounts.Add(i =>
    {
      accountNumber = CreateAccountNumber(i);
      return new Account
      {
        Name = name,
        Balance = initialDeposit,
        AccountNumber = accountNumber,
      };
    });
    _db.Users.Add(
      new User
      {
        Name = name,
        Email = email,
        Username = username,
        PasswordHash = hash,
        PasswordSalt = salt,
        AccountId = accountId,
      }
    );
    return accountNumber;
  }

  private static string CreateAccountNumber(int id)
  {
    // Ensure the ID has at least 4 digits, pad with leading zeros if needed
    string idPart = id.ToString("D4"); // Formats as 4-digit with leading zeros

    // Take last 4 digits in case ID has more than 4 digits
    if (idPart.Length > 4)
    {
      idPart = idPart.Substring(idPart.Length - 4);
    }

    // Combine all parts
    return $"{BANK_CODE}{BRANCH_CODE}{idPart}";
  }

  public void Login(string username, SecureString password)
  {
    try
    {
      User user = _db.Users.Where(u => u.Username == username).First();
      if (SecureStringHasher.VerifySecureString(password, user.PasswordHash, user.PasswordSalt))
      {
        currentUser = user;
        currentAccount = _db.Accounts.Find(user.AccountId);
      }
      else
      {
        throw new Exception("Incorrect password!");
      }
    }
    catch (Exception)
    {
      throw new LoginInvalidCredentialsException();
    }
  }

  public void Logout()
  {
    currentUser = null;
    currentAccount = null;
    _screen.SwitchTo(new Landing(this));
  }

  public bool ProtectScreen()
  {
    if (currentUser is null || currentAccount is null)
    {
      _screen.SwitchTo(new Landing(this));
      return true;
    }
    return false;
  }

  public void Build()
  {
    _screen.Start();
  }

  internal void MakeDeposit(decimal amount, string desc)
  {
    if (currentUser is not null && currentAccount is not null && amount >= MIN_DEPOSIT)
    {
      _db.Transactions.Add(
        new Transaction
        {
          Type = TransactionType.Deposit,
          Amount = amount,
          Description = desc,
          Timestamp = DateTime.Now,
          ToAccount = currentAccount.AccountNumber,
        }
      );

      currentAccount = _db.Accounts.Update(
        currentAccount.Id,
        new Account
        {
          Name = currentAccount.Name,
          Balance = currentAccount.Balance + amount,
          AccountNumber = currentAccount.AccountNumber,
        }
      );
    }
  }

  internal void MakeTransfer(decimal amount, string desc, string to)
  {
    Account toAccount = GetAccount(to);
    if (
      currentUser is not null
      && currentAccount is not null
      && amount >= MIN_TRANSFER
      && currentAccount.Balance >= amount
      && toAccount is not null
    )
    {
      _db.Transactions.Add(
        new Transaction
        {
          Type = TransactionType.Transfer,
          Amount = amount,
          Description = desc,
          Timestamp = DateTime.Now,
          FromAccount = currentAccount.AccountNumber,
          ToAccount = toAccount.AccountNumber,
        }
      );

      currentAccount = _db.Accounts.Update(
        currentAccount.Id,
        new Account
        {
          Name = currentAccount.Name,
          Balance = currentAccount.Balance - amount,
          AccountNumber = currentAccount.AccountNumber,
        }
      );

      _db.Accounts.Update(
        toAccount.Id,
        new Account
        {
          Name = toAccount.Name,
          Balance = toAccount.Balance + amount,
          AccountNumber = toAccount.AccountNumber,
        }
      );
    }
  }

  internal void MakeWithdrawal(decimal amount, string desc)
  {
    if (currentUser is not null && currentAccount is not null && amount >= MIN_WITHDRAW)
    {
      _db.Transactions.Add(
        new Transaction
        {
          Type = TransactionType.Withdrawal,
          Amount = amount,
          Description = desc,
          Timestamp = DateTime.Now,
          FromAccount = currentAccount.AccountNumber,
        }
      );

      currentAccount = _db.Accounts.Update(
        currentAccount.Id,
        new Account
        {
          Name = currentAccount.Name,
          Balance = currentAccount.Balance - amount,
          AccountNumber = currentAccount.AccountNumber,
        }
      );
    }
  }

  internal IEnumerable<Transaction> GetTransactions()
  {
    if (currentUser is not null && currentAccount is not null)
    {
      return _db.Transactions.Where(t =>
        t.ToAccount == currentAccount.AccountNumber || t.FromAccount == currentAccount.AccountNumber
      );
    }
    return [];
  }

  internal Account GetAccount(string acc)
  {
    return _db.Accounts.Where(a => a.AccountNumber == acc).First();
  }

  internal string GetAccountName(string acc)
  {
    return GetAccount(acc).Name;
  }
}
