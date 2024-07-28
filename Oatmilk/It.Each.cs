using System.Runtime.CompilerServices;
using static Oatmilk.TestBuilder;

namespace Oatmilk;

public static partial class It
{
  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Action<T, TestInput> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, TestInput, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Action<T, TestInput> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionResolver, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Func<T, TestInput, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionResolver, timeout, lineNumber, filePath).When(body);

  ///

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Action<T> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    Func<T, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionFormatString, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Action<T> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionResolver, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Creates a suite of tests for every element in the <paramref name="values"/> collection.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="body">The method body of the test where assertions should be put. Each value from <paramref name="values"/> is passed to this.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    Func<T, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Each(values, descriptionResolver, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// A fluent api for creating a suite of tests for every element in the <paramref name="values"/> collection.
  /// See <see cref="ItEachBlock{T}.When(Action{T})"/>.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionFormatString">A format string that is used to generate the test's description.  Each value from <paramref name="values"/> is used as the 0th param.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static ItEachBlock<T> Each<T>(
    IEnumerable<T> values,
    string descriptionFormatString,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      v => SafeFormat(descriptionFormatString, v),
      IsOnly: false,
      IsSkipped: false,
      Timeout: timeout,
      lineNumber,
      filePath
    );

  /// <summary>
  /// A fluent api for creating a suite of tests for every element in the <paramref name="values"/> collection.
  /// See <see cref="ItEachBlock{T}.When(Action{T})"/>.
  /// </summary>
  /// <typeparam name="T">The type of the data to be passed to the test's method body</typeparam>
  /// <param name="values">A list of values to pass to the test</param>
  /// <param name="descriptionResolver">A function that is used to generate the test's description.  Each value from <paramref name="values"/> is passed to it.</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static ItEachBlock<T> Each<T>(
    IEnumerable<T> values,
    Func<T, string> descriptionResolver,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      values,
      descriptionResolver,
      IsOnly: false,
      IsSkipped: false,
      timeout,
      lineNumber,
      filePath
    );
}
