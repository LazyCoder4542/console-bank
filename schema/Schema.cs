using System.Reflection;

namespace bank_app.schema;

[AttributeUsage(AttributeTargets.Property)]
public class DbFieldAttribute : Attribute
{
  public bool IsRequired { get; set; }
  public int MaxLength { get; set; } = -1;
}

public class SchemaGenerator
{
  public static Dictionary<string, (Type Type, bool IsRequired, int MaxLength)> GenerateSchema<T>()
  {
    var schema = new Dictionary<string, (Type, bool, int)>();

    foreach (var prop in typeof(T).GetProperties())
    {
      var attr = prop.GetCustomAttribute<DbFieldAttribute>();
      schema.Add(prop.Name, (prop.PropertyType, attr?.IsRequired ?? false, attr?.MaxLength ?? -1));
    }

    return schema;
  }
}
