using System.Runtime.CompilerServices;
using static Detestable.TestBuilder;

namespace Detestable;

public static partial class Describe
{
  public static void Only<T>(
    IEnumerable<T> values,
    string description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, description, lineNumber, filePath).As(body);

  public static void Only<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Action<T> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, description, lineNumber, filePath).As(body);

  public static void Only(
    string description,
    Action body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description, lineNumber, filePath).As(body);

  public static DescribeBlock Only(
    string description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      Description: description,
      IsOnly: true,
      IsSkipped: false,
      LineNumber: lineNumber,
      FilePath: filePath
    );

  public static DescribeEachBlock<T> Only<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, x => SafeFormat(descriptionFormatString, x), lineNumber, filePath);

  public static DescribeEachBlock<T> Only<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => new(values, description, IsOnly: true, IsSkipped: false, lineNumber, filePath);

  // Invalid Async Methods:

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Only(string,Action,int,string)"/> instead.
  /// </summary>
  /// <param name="description"></param>
  /// <param name="body"></param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage)]
  public static void Only(
    string description,
    Func<Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description).As(body);

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Each{T}(IEnumerable{T},string,Action{T},int,string)"/> instead.
  /// </summary>
  /// <param name="descriptionFormatString"></param>
  /// <param name="body"></param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage)]
  public static void Only<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, descriptionFormatString).As(body);

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeOnly(Func{Task})" />.
  /// Use <see cref="Only{T}(IEnumerable{T},Func{T,string},Action{T},int,string)"/> instead.
  /// </summary>
  /// <param name="description"></param>
  /// <param name="body"></param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage)]
  public static void Only<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, description).As(body);
}
