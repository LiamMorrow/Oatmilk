using System.Runtime.CompilerServices;
using static Detestable.TestBuilder;

namespace Detestable;

public static partial class It
{
  public static void Skip<T>(
    IEnumerable<T> values,
    string description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(values, description, lineNumber, filePath).When(body);

  public static void Skip<T>(
    IEnumerable<T> values,
    string description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(values, description, lineNumber, filePath).When(body);

  public static void Skip<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(values, description, lineNumber, filePath).When(body);

  public static void Skip<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(values, description, lineNumber, filePath).When(body);

  public static void Skip(
    string description,
    Func<Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(description, lineNumber, filePath).When(body);

  public static void Skip(
    string description,
    Action body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(description, lineNumber, filePath).When(body);

  public static ItBlock Skip(
    string description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      Description: description,
      IsOnly: false,
      IsSkipped: true,
      LineNumber: lineNumber,
      FilePath: filePath
    );

  public static ItEachBlock<T> Skip<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Skip(values, x => SafeFormat(descriptionFormatString, x), lineNumber, filePath);

  public static ItEachBlock<T> Skip<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => new(values, description, IsOnly: false, IsSkipped: true, lineNumber, filePath);
}
