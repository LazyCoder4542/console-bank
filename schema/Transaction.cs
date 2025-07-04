namespace bank_app.schema;

public enum TransactionType
{
  Deposit,
  Withdrawal,
  Transfer,
}

public class Transaction
{
  [DbField(IsRequired = true)]
  public int Id { get; set; }

  public TransactionType Type { get; set; }

  public decimal Amount { get; set; }

  public string Description { get; set; } = string.Empty;

  public DateTime Timestamp { get; set; }

  public string FromAccount { get; set; } = string.Empty;
  public string ToAccount { get; set; } = string.Empty;
}
