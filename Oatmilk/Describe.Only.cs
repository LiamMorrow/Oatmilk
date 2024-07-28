using System.Runtime.CompilerServices;
using static Oatmilk.TestBuilder;

namespace Oatmilk;

public static partial class Describe
{
  /// <summary>
  /// Creates a suite of tests that will be run exclusively.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the descriptions's body</typeparam>
  /// <param name="values">A list of values to pass to the description</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the description. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Only<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Action<T> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, descriptionFormatString, timeout, lineNumber, filePath).As(body);

  /// <summary>
  /// Creates a suite of tests that will be run exclusively.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the descriptions's body</typeparam>
  /// <param name="values">A list of values to pass to the description</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="body">The method body of the description. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Only<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Action<T> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, descriptionResolver, timeout, lineNumber, filePath).As(body);

  /// <summary>
  /// Creates a suite of tests that will be run exclusively.
  /// </summary>
  /// <param name="description">The description of the describe block</param>
  /// <param name="body">The method body of the description</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Only(
    string description,
    Action body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description, timeout, lineNumber, filePath).As(body);

  /// <summary>
  /// A fluent api for creating a describe block that will be run exclusively.
  /// </summary>
  /// <param name="description">The description of the describe block</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static DescribeBlock Only(
    string description,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      Description: description,
      IsOnly: true,
      IsSkipped: false,
      Timeout: timeout,
      LineNumber: lineNumber,
      FilePath: filePath
    );

  /// <summary>
  /// A fluent api for creating a suite of tests that will be run exclusively.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the descriptions's body</typeparam>
  /// <param name="values">A list of values to pass to the description</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <returns></returns>
  public static DescribeEachBlock<T> Only<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, x => SafeFormat(descriptionFormatString, x), timeout, lineNumber, filePath);

  /// <summary>
  /// A fluent api for creating a suite of tests that will be run exclusively.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the descriptions's body</typeparam>
  /// <param name="values">A list of values to pass to the description</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description. Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <returns></returns>
  public static DescribeEachBlock<T> Only<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      descriptionResolver,
      IsOnly: true,
      IsSkipped: false,
      Timeout: timeout,
      lineNumber,
      filePath
    );

  // Invalid Async Methods:

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Only(string,Action,TimeSpan?,int,string)"/> instead.
  /// </summary>
  /// <param name="description"></param>
  /// <param name="body"></param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
  public static void Only(
    string description,
    Func<Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description).As(body);

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Each{T}(IEnumerable{T},string,Action{T},TimeSpan?,int,string)"/> instead.
  /// </summary>
  /// <param name="values"></param>
  /// <param name="descriptionFormatString"></param>
  /// <param name="body"></param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
  public static void Only<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, descriptionFormatString).As(body);

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Only{T}(IEnumerable{T},Func{T,string},Action{T},TimeSpan?,int,string)"/> instead.
  /// </summary>
  /// <param name="values"></param>
  /// <param name="descriptionResolver"></param>
  /// <param name="body"></param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
  public static void Only<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Func<T, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(values, descriptionResolver).As(body);
}
