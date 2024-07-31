using System.Runtime.CompilerServices;
using static Oatmilk.TestBuilder;

namespace Oatmilk;

public partial class Describe
{
  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Action<T> body,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, testOptions, lineNumber, filePath).As(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is given.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Action<T> body,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionResolver, testOptions, lineNumber, filePath).As(body);

  /// <summary>
  /// A fluent api for creating a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static DescribeEachBlock<T> Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      v => SafeFormat(descriptionFormatString, v),
      IsOnly: false,
      IsSkipped: false,
      TestOptions: testOptions,
      lineNumber,
      filePath
    );

  /// <summary>
  /// A fluent api for creating a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it</param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static DescribeEachBlock<T> Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      descriptionResolver,
      IsOnly: false,
      IsSkipped: false,
      TestOptions: testOptions,
      lineNumber,
      filePath
    );

  // Invalid Async Methods:

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Each{T}(IEnumerable{T},string,Action{T},TestOptions,int,string)"/> instead.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="values"></param>
  /// <param name="descriptionFormatString"></param>
  /// <param name="body"></param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage)]
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, Task> body,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, testOptions).As(body);

  /// <summary>
  /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Each{T}(IEnumerable{T},Func{T,string},Action{T},TestOptions,int,string)"/> instead.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  /// <param name="values"></param>
  /// <param name="description"></param>
  /// <param name="body"></param>
  /// <param name="testOptions">The options for each test in the test suite, including the timeout</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage)]
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> description,
    Func<T, Task> body,
    TestOptions testOptions = default,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, description).As(body);
}
