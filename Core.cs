// namespace bank_app;

// public class Account
// {
//   public string AccountNumber { get; }
//   public string OwnerName { get; set; }
//   public decimal Balance { get; private set; }
//   public byte[] PinHash { get; }
//   public List<Transaction> Transactions { get; }

//   public Account(string accountNumber, string ownerName, byte[] pinHash)
//   {
//     AccountNumber = accountNumber;
//     OwnerName = ownerName;
//     Balance = 0;
//     PinHash = pinHash;
//     Transactions = new List<Transaction>();
//   }

//   public void Withdraw(decimal amount)
//   {
//     if (amount <= 0)
//       throw new ArgumentException("Invalid amount");
//     if (Balance < amount)
//       throw new InvalidOperationException("Insufficient funds");

//     Balance -= amount;
//     Transactions.Add(
//       new Transaction(
//         type: TransactionType.Withdrawal,
//         amount: -amount,
//         description: $"ATM withdrawal"
//       )
//     );
//   }
// }

// public class Bank
// {
//   private List<Account> accounts = new List<Account>();
//   public Account? CurrentUser { get; private set; }
// }
