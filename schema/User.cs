namespace bank_app.schema;

public class User
{
  [DbField(IsRequired = true)]
  public int Id { get; set; }

  [DbField(IsRequired = true, MaxLength = 100)]
  public string Name { get; set; } = string.Empty;

  [DbField(IsRequired = true, MaxLength = 100)]
  public string Email { get; set; } = string.Empty;

  [DbField(IsRequired = true, MaxLength = 100)]
  public string Username { get; set; } = string.Empty;

  [DbField(IsRequired = true)]
  public string PasswordHash { get; set; } = string.Empty;

  [DbField(IsRequired = true)]
  public string PasswordSalt { get; set; } = string.Empty;

  [DbField(IsRequired = true)]
  public int AccountId { get; set; }
}
