namespace bank_app.schema;

public class Account
{
  [DbField(IsRequired = true)]
  public int Id { get; set; }

  [DbField(IsRequired = true, MaxLength = 100)]
  public string Name { get; set; } = string.Empty;

  public decimal Balance { get; set; }

  public string AccountNumber { get; set; } = string.Empty;
}
