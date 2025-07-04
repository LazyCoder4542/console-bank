using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace bank_app;

public static class Utils
{
  public const string BANK_CODE = "999";
  public const string BRANCH_CODE = "000";

  public static SecureString GetPasswordFromConsole(string prompt = "Enter password: ")
  {
    Console.Write(prompt);
    var securePassword = new SecureString();

    try
    {
      ConsoleKeyInfo key;
      while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
      {
        // Handle backspace
        if (key.Key == ConsoleKey.Backspace)
        {
          if (securePassword.Length > 0)
          {
            securePassword.RemoveAt(securePassword.Length - 1);
            Console.Write("\b \b"); // Erase the last asterisk
          }
        }
        // Ignore other non-character keys
        else if (!char.IsControl(key.KeyChar))
        {
          if (securePassword.Length < 32) // Prevent overly long passwords
          {
            securePassword.AppendChar(key.KeyChar);
            Console.Write("*");
          }
          else
          {
            Console.Beep(); // Alert user
          }
        }
      }

      if (securePassword.Length == 0)
      {
        throw new InvalidOperationException("Password cannot be empty");
      }

      Console.WriteLine(); // Move to next line after Enter
      securePassword.MakeReadOnly();
      return securePassword;
    }
    catch
    {
      securePassword.Dispose();
      throw;
    }
  }

  // public static void ClearLine()
  // {
  //   Console.SetCursorPosition(0, Console.CursorTop);
  //   Console.Write(new string(' ', Console.WindowWidth));
  //   Console.SetCursorPosition(0, Console.CursorTop);
  // }

  public static void ClearLine(int lineCount)
  {
    if (lineCount <= 0)
      return;

    int currentLine = Console.CursorTop;
    int targetLine = Math.Max(currentLine - lineCount, 0);

    Console.SetCursorPosition(0, targetLine);
    for (int i = 0; i < lineCount && targetLine + i < Console.BufferHeight; i++)
    {
      Console.Write(new string(' ', Console.WindowWidth - 1));
    }
    Console.SetCursorPosition(0, targetLine);
  }
}

public static class SecureStringHasher
{
  // Hash a SecureString using PBKDF2 with salt
  public static string HashSecureString(SecureString secureString, out string saltBase64)
  {
    // Generate random salt
    byte[] salt = new byte[32];
    using (var rng = RandomNumberGenerator.Create())
    {
      rng.GetBytes(salt);
    }

    // Convert SecureString to byte array
    byte[] passwordBytes = SecureStringToByteArray(secureString);

    // Hash with PBKDF2
    using (
      var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 100000, HashAlgorithmName.SHA512)
    )
    {
      byte[] hash = pbkdf2.GetBytes(64); // 64 bytes = 512 bits
      saltBase64 = Convert.ToBase64String(salt);
      return Convert.ToBase64String(hash);
    }
  }

  // Verify a SecureString against stored hash and salt
  public static bool VerifySecureString(
    SecureString secureString,
    string storedHashBase64,
    string saltBase64
  )
  {
    byte[] salt = Convert.FromBase64String(saltBase64);
    byte[] storedHash = Convert.FromBase64String(storedHashBase64);

    byte[] passwordBytes = SecureStringToByteArray(secureString);

    using (
      var pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 100000, HashAlgorithmName.SHA512)
    )
    {
      byte[] testHash = pbkdf2.GetBytes(64);

      return CryptographicOperations.FixedTimeEquals(testHash, storedHash);
    }
  }

  // Helper to convert SecureString to byte array
  private static byte[] SecureStringToByteArray(SecureString secureString)
  {
    IntPtr bstr = IntPtr.Zero;
    try
    {
      bstr = Marshal.SecureStringToBSTR(secureString);
      int length = Marshal.ReadInt32(bstr, -4);
      byte[] bytes = new byte[length];

      for (int i = 0; i < length; i++)
      {
        bytes[i] = Marshal.ReadByte(bstr, i);
      }

      return bytes;
    }
    finally
    {
      if (bstr != IntPtr.Zero)
      {
        Marshal.ZeroFreeBSTR(bstr);
      }
    }
  }
}

public static class StringExtensions
{
  public static string Align(
    this string input,
    int width,
    Alignment alignment = Alignment.Left,
    char paddingChar = ' '
  )
  {
    if (string.IsNullOrEmpty(input))
      return new string(paddingChar, width);

    if (input.Length > width)
      return input.Substring(0, width - 3) + "..."; // Truncate

    switch (alignment)
    {
      case Alignment.Right:
        return input.PadLeft(width, paddingChar);
      case Alignment.Center:
        int leftPadding = (width - input.Length) / 2;
        int rightPadding = width - input.Length - leftPadding;
        return new string(paddingChar, leftPadding) + input + new string(paddingChar, rightPadding);
      case Alignment.Left:
      default:
        return input.PadRight(width, paddingChar);
    }
  }
}

public enum Alignment
{
  Left,
  Right,
  Center,
}
