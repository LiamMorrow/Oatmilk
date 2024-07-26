using System.Runtime.CompilerServices;

namespace Detestable;

public static partial class It
{
  /// <returns></returns>
  public static void Skip<T>(
    IEnumerable<T> values,
    string description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }

  public static void Skip<T>(
    IEnumerable<T> values,
    string description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }

  public static void Skip<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }

  public static void Skip<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }

  public static void Skip(
    string description,
    Func<Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }

  public static void Skip(
    string description,
    Action body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) { }
}
