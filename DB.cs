using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using bank_app.schema;

namespace bank_app;

// public class DB
// {
//   private readonly string _storagePath;
//   private readonly Dictionary<string, ConcurrentDictionary<int, long>> _docs;
//   private readonly ReaderWriterLockSlim _lock = new();

//   public DB(string storagePath)
//   {
//     _storagePath = storagePath;
//     Directory.CreateDirectory(storagePath);

//     _docs = InitDocs();

//     LoadIndexes();
//   }

//   private Dictionary<string, ConcurrentDictionary<int, long>> InitDocs()
//   {
//     return new Dictionary<string, ConcurrentDictionary<int, long>>
//     {
//       ["user"] = new ConcurrentDictionary<int, long>(),
//       ["transaction"] = new ConcurrentDictionary<int, long>(),
//     };
//   }

//   private void LoadIndexes()
//   {
//     foreach (var doc in _docs)
//     {
//       var IndexPath = Path.Combine(_storagePath, $"{doc.Key}.index");
//       if (File.Exists(IndexPath))
//       {
//         var lines = File.ReadAllLines(IndexPath);
//         foreach (var line in lines)
//         {
//           var parts = line.Split("|");
//           doc.Value[int.Parse(parts[0])] = long.Parse(parts[1]);
//         }
//       }
//     }
//   }

//   private void SaveIndexes()
//   {
//     _lock.EnterWriteLock();
//     try
//     {
//       //Can still refractor

//       var userIndexPath = Path.Combine(_storagePath, "user.index");
//       File.WriteAllLines(userIndexPath, _docs["user"].Select(kvp => $"{kvp.Key}|{kvp.Value}"));

//       var transactionIndexPath = Path.Combine(_storagePath, "transaction.index");
//       File.WriteAllLines(userIndexPath, _docs["user"].Select(kvp => $"{kvp.Key}|{kvp.Value}"));
//     }
//     finally
//     {
//       _lock.ExitWriteLock();
//     }
//   }
// }

public class DB
{
  private string _storagePath;
  public DBSet<User> Users;
  public DBSet<Transaction> Transactions;
  public DBSet<Account> Accounts;

  private readonly Dictionary<string, ConcurrentDictionary<int, long>> _docs;

  private readonly ReaderWriterLockSlim _lock = new();

  private readonly ConcurrentDictionary<string, int> _lastIds = new();

  public DB(string storagePath)
  {
    Directory.CreateDirectory(storagePath);

    _storagePath = storagePath;

    _docs = InitDocs();

    LoadAllIndexes();

    // Initialize last IDs for each entity type
    foreach (var kvp in _docs)
    {
      _lastIds[kvp.Key] = kvp.Value.Keys.DefaultIfEmpty().Max();
    }

    Users = new(this, "user");
    Transactions = new(this, "transaction");
    Accounts = new(this, "account");

    // string s = string.Join(
    //   ",",
    //   _docs.Select(
    //     (k) =>
    //       k.Key + ": [" + string.Join(", ", k.Value.Select((kk) => $"{kk.Key}: {kk.Value}")) + "]"
    //   )
    // );
  }

  public class DBSet<T>
    where T : new()
  {
    private readonly DB _db;
    private readonly string _name;
    private readonly ConcurrentDictionary<int, long> _doc = new();

    internal DBSet(DB db, string name)
    {
      _db = db;
      _name = name;
    }

    public T Find(int id)
    {
      return _db.Get<T>(id);
    }

    public IEnumerable<T> FindAll()
    {
      return _db.GetAll<T>();
    }

    public IEnumerable<T> Where(Func<T, bool> predicate)
    {
      return _db.GetAll<T>().Where(predicate);
    }

    public int Add(Func<int, T> createEntity)
    {
      return _db.Add(createEntity);
    }

    public int Add(T entity)
    {
      return _db.Add(entity);
    }

    public T Update(int id, T entity)
    {
      return _db.Update(id, entity);
    }

    private void LoadIndex()
    {
      _db.LoadIndex(_name);
    }

    private void SaveIndex()
    {
      _db.SaveIndex(_name);
    }
  }

  private Dictionary<string, ConcurrentDictionary<int, long>> InitDocs()
  {
    return new Dictionary<string, ConcurrentDictionary<int, long>>
    {
      ["user"] = new ConcurrentDictionary<int, long>(),
      ["transaction"] = new ConcurrentDictionary<int, long>(),
      ["account"] = new ConcurrentDictionary<int, long>(),
    };
  }

  private void LoadIndex(string index)
  {
    var indexPath = Path.Combine(_storagePath, $"{index}.index");
    Console.WriteLine(indexPath);
    if (File.Exists(indexPath))
    {
      var lines = File.ReadAllLines(indexPath);
      foreach (var line in lines)
      {
        var parts = line.Split("|");
        var id = int.Parse(parts[0]);
        _docs[index][id] = long.Parse(parts[1]);

        // Update the last ID for this index if needed
        if (!_lastIds.ContainsKey(index) || id > _lastIds[index])
        {
          _lastIds[index] = id;
        }
      }
    }
  }

  private void SaveIndex(string index)
  {
    //Can still refractor

    var indexPath = Path.Combine(_storagePath, $"{index}.index");
    File.WriteAllLines(indexPath, _docs[index].Select(kvp => $"{kvp.Key}|{kvp.Value}"));
  }

  private void LoadAllIndexes()
  {
    LoadIndex("user");
    LoadIndex("transaction");
    LoadIndex("account");
  }

  private void SaveAllIndexes()
  {
    SaveIndex("user");
    SaveIndex("transaction");
    LoadIndex("account");
  }

  public T Get<T>(int id)
    where T : new()
  {
    var indexName = GetIndexNameForType(typeof(T));
    if (!_docs.TryGetValue(indexName, out var index))
    {
      throw new InvalidOperationException($"No index found for type {typeof(T).Name}");
    }

    if (!index.TryGetValue(id, out var position))
    {
      throw new KeyNotFoundException($"Record with ID {id} not found");
    }

    var dataFilePath = Path.Combine(_storagePath, $"{indexName}.data");
    using (var stream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
    using (var reader = new BinaryReader(stream))
    {
      stream.Seek(position, SeekOrigin.Begin);
      T obj = ReadObject<T>(reader);
      // obj.GetType().GetProperty("Id").SetValue(T,)
      return obj;
    }
  }

  public IEnumerable<T> GetAll<T>()
    where T : new()
  {
    var indexName = GetIndexNameForType(typeof(T));
    if (!_docs.TryGetValue(indexName, out var index))
    {
      throw new InvalidOperationException($"No index found for type {typeof(T).Name}");
    }

    var dataFilePath = Path.Combine(_storagePath, $"{indexName}.data");
    using (var stream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read))
    using (var reader = new BinaryReader(stream))
    {
      foreach (var position in index.Values)
      {
        stream.Seek(position, SeekOrigin.Begin);
        yield return ReadObject<T>(reader);
      }
    }
  }

  private T ReadObject<T>(BinaryReader reader)
    where T : new()
  {
    var schema = SchemaGenerator.GenerateSchema<T>();
    var obj = new T();

    foreach (var field in schema)
    {
      var property = typeof(T).GetProperty(field.Key);
      if (property == null)
        continue;

      object value = ReadField(reader, field.Value.Type);
      property.SetValue(obj, value);
    }

    return obj;
  }

  private object ReadField(BinaryReader reader, Type type)
  {
    if (type == typeof(string))
    {
      return reader.ReadString();
    }
    else if (type == typeof(int))
    {
      return reader.ReadInt32();
    }
    else if (type == typeof(long))
    {
      return reader.ReadInt64();
    }
    else if (type == typeof(double))
    {
      return reader.ReadDouble();
    }
    else if (type == typeof(bool))
    {
      return reader.ReadBoolean();
    }
    else if (type == typeof(DateTime))
    {
      return DateTime.FromBinary(reader.ReadInt64());
    }
    else if (type == typeof(decimal))
    {
      // Decimal is stored as four 32-bit integers
      var bits = new int[4];
      for (int i = 0; i < 4; i++)
      {
        bits[i] = reader.ReadInt32();
      }
      return new decimal(bits);
    }
    else if (type.IsEnum)
    {
      Type underlyingType = Enum.GetUnderlyingType(type);
      int bitSize = Marshal.SizeOf(underlyingType) * 8;
      long value = 0;

      int byteSize = bitSize / 8;
      byte[] bytes = reader.ReadBytes(byteSize);

      // Pad the array to 8 bytes if needed
      if (byteSize < 8)
      {
        byte[] paddedBytes = new byte[8];
        Buffer.BlockCopy(bytes, 0, paddedBytes, 0, byteSize);
        value = BitConverter.ToInt64(paddedBytes, 0);
      }
      else
      {
        value = BitConverter.ToInt64(bytes, 0);
      }

      // Handle smaller types by masking
      switch (Marshal.SizeOf(underlyingType))
      {
        case 1:
          value &= 0xFF;
          break; // byte/sbyte
        case 2:
          value &= 0xFFFF;
          break; // short/ushort
        case 4:
          value &= 0xFFFFFFFF;
          break; // int/uint
        // long/ulong uses all 64 bits
      }

      return Enum.ToObject(type, value);
    }
    // Add more type support as needed
    else
    {
      throw new NotSupportedException($"Type {type.Name} is not supported for deserialization");
    }
  }

  private string GetIndexNameForType(Type type)
  {
    // Simple implementation - you might want to make this more sophisticated
    return type.Name.ToLower();
  }

  // private int Add<T>(T obj)
  //   where T : new()
  // {
  //   var indexName = GetIndexNameForType(typeof(T));
  // }

  public int Add<T>(T o)
    where T : new()
  {
    return Add<T>(i => o);
  }

  public int Add<T>(Func<int, T> func)
    where T : new()
  {
    T obj;
    var indexName = GetIndexNameForType(typeof(T));
    if (!_docs.TryGetValue(indexName, out var index))
    {
      throw new InvalidOperationException($"No index found for type {typeof(T).Name}");
    }

    var dataFilePath = Path.Combine(_storagePath, $"{indexName}.data");

    _lock.EnterWriteLock();
    try
    {
      // int lastIndex = _lastIds[indexName];
      // Generate new ID specific to this entity type
      // var newId = Interlocked.Increment(ref lastIndex);

      var newId = ++_lastIds[indexName];

      obj = func(newId);

      typeof(T).GetProperty("Id")!.SetValue(obj, newId);

      // Append to data file and get position
      long position;
      using (var stream = new FileStream(dataFilePath, FileMode.Append, FileAccess.Write))
      using (var writer = new BinaryWriter(stream))
      {
        position = stream.Position;
        WriteObject(writer, obj);
      }

      // Update index
      index[newId] = position;
      SaveIndex(indexName);

      return newId;
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }

  public T Update<T>(int id, T obj)
    where T : new()
  {
    var indexName = GetIndexNameForType(typeof(T));
    if (!_docs.TryGetValue(indexName, out var index))
    {
      throw new InvalidOperationException($"No index found for type {typeof(T).Name}");
    }

    var dataFilePath = Path.Combine(_storagePath, $"{indexName}.data");

    _lock.EnterWriteLock();
    try
    {
      typeof(T).GetProperty("Id")!.SetValue(obj, id); // ensure same id

      // Update to data file and get position
      using (var stream = new FileStream(dataFilePath, FileMode.Open, FileAccess.Write))
      using (var writer = new BinaryWriter(stream))
      {
        writer.BaseStream.Position = index[id];
        WriteObject(writer, obj);
      }

      // Update index
      // index[newId] = position;
      // SaveIndex(indexName);
      return obj;
    }
    finally
    {
      _lock.ExitWriteLock();
    }
  }

  private void WriteObject<T>(BinaryWriter writer, T obj)
  {
    var schema = SchemaGenerator.GenerateSchema<T>();

    foreach (var P in schema)
    {
      Type Type;
      bool IsRequired;
      int MaxLength;

      (Type, IsRequired, MaxLength) = P.Value;

      // Console.WriteLine(
      //   $"{P.Key, -20} Type {Type} - IsRequired {IsRequired} - MaxLength {MaxLength}"
      // );
    }

    foreach (var field in schema)
    {
      var property = typeof(T).GetProperty(field.Key);
      if (property == null)
        continue;

      var value = property.GetValue(obj);
      WriteField(writer, value!, field.Value.Type);
    }
  }

  private void WriteField(BinaryWriter writer, object value, Type type)
  {
    if (type == typeof(string))
    {
      writer.Write((string)value);
    }
    else if (type == typeof(int))
    {
      writer.Write((int)value);
    }
    else if (type == typeof(long))
    {
      writer.Write((long)value);
    }
    else if (type == typeof(double))
    {
      writer.Write((double)value);
    }
    else if (type == typeof(bool))
    {
      writer.Write((bool)value);
    }
    else if (type == typeof(DateTime))
    {
      writer.Write(((DateTime)value).ToBinary());
    }
    else if (type == typeof(decimal))
    {
      // Decimal is stored as four 32-bit integers
      decimal decimalValue = (decimal)value;
      int[] bits = decimal.GetBits(decimalValue);
      foreach (int bit in bits)
      {
        writer.Write(bit);
      }
    }
    else if (type.IsEnum)
    {
      long numericValue = Convert.ToInt64(value);
      Type underlyingType = Enum.GetUnderlyingType(value.GetType());
      int bitSize = Marshal.SizeOf(underlyingType) * 8;

      byte[] bytes = BitConverter.GetBytes(numericValue);
      writer.Write(bytes, 0, bitSize / 8);
    }
    else
    {
      throw new NotSupportedException($"Type {type.Name} is not supported for serialization");
    }
  }
}
