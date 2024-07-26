using System.Runtime.CompilerServices;

namespace Detestable;

public static partial class It
{
  public static void Each<T>(
    IEnumerable<T> values,
    string description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  )
  {
    foreach (var value in values)
    {
      // Make sure to copy the value to avoid closure issues
      var val = value;
      TestBuilder.It(description + " " + value, lineNumber, filePath).When(() => body(val));
    }
  }

  public static void Each<T>(
    IEnumerable<T> values,
    string description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  )
  {
    foreach (var value in values)
    {
      // Make sure to copy the value to avoid closure issues
      var val = value;
      TestBuilder.It(description + " " + value, lineNumber, filePath).When(() => body(val));
    }
  }

  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  )
  {
    foreach (var value in values)
    {
      // Make sure to copy the value to avoid closure issues
      var val = value;
      TestBuilder.It(description(value), lineNumber, filePath).When(() => body(val));
    }
  }

  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  )
  {
    foreach (var value in values)
    {
      // Make sure to copy the value to avoid closure issues
      var val = value;
      TestBuilder.It(description(value), lineNumber, filePath).When(() => body(val));
    }
  }
}
