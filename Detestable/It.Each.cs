using System.Runtime.CompilerServices;
using static Detestable.TestBuilder;

namespace Detestable;

public static partial class It
{
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, lineNumber, filePath).When(body);

  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, lineNumber, filePath).When(body);

  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, description, lineNumber, filePath).When(body);

  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, description, lineNumber, filePath).When(body);

  public static ItEachBlock<T> Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      v => SafeFormat(descriptionFormatString, v),
      IsOnly: false,
      IsSkipped: false,
      lineNumber,
      filePath
    );

  public static ItEachBlock<T> Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => new(values, description, IsOnly: false, IsSkipped: false, lineNumber, filePath);
}
