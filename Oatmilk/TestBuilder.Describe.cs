using System.Runtime.CompilerServices;

namespace Oatmilk;

public static partial class TestBuilder
{
  internal const string InvalidDescribeAsyncMethodCallMessage = """
    Must call Describe methods with a synchronous body.
    Use BeforeAll, BeforeEach to perform asynchronous test setup.
    Remove async from the method signature to fix this error.
    """;

  /// <summary>
  /// Obsolete. Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
  /// Use <see cref="Describe(string,Action,TimeSpan?,int,string)"/> instead.
  /// </summary>
  /// <param name="description"></param>
  /// <param name="body"></param>
  /// <param name="timeout"></param>
  /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
  [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
  public static void Describe(string description, Func<Task> body, TimeSpan? timeout = null) =>
    throw new InvalidOperationException(InvalidDescribeAsyncMethodCallMessage);

  /// <summary>
  /// Describes a suite of tests.
  /// This method runs synchronously, and allows for the creation of nested test suites.
  /// </summary>
  /// <param name="description">The description of this block of the test suite.</param>
  /// <param name="body">A callback which is immediately invoked to describe tests</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void Describe(
    string description,
    Action body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Describe(description, timeout, lineNumber, filePath).As(body);

  /// <summary>
  /// Describes a suite of tests using a fluent syntax.  Specify the body of the suite using the <see cref="DescribeBlock.As(Action)" /> method.
  /// </summary>
  /// <param name="description">The description of this block of the test suite.</param>
  /// <param name="timeout">The timeout for each test in the test suite</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <returns>A <see cref="DescribeBlock" /> object which allows for the creation of nested test suites.</returns>
  public static DescribeBlock Describe(
    string description,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  )
  {
    return new DescribeBlock(
      description,
      IsOnly: false,
      IsSkipped: false,
      Timeout: timeout,
      lineNumber,
      filePath
    );
  }

  /// <summary>
  /// A builder object for creating nested test suites using a fluent syntax.
  /// </summary>
  /// <param name="Description">The description for the block of tests.</param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="Timeout">The timeout for each test in the test suite</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record DescribeBlock(
    string Description,
    bool IsOnly,
    bool IsSkipped,
    TimeSpan? Timeout,
    int LineNumber,
    string FilePath
  )
  {
    /// <summary>
    /// Describes a suite of tests within the current scope.
    /// </summary>
    /// <param name="body">An action containing the tests to run in this scope.</param>
    public void As(Action body)
    {
      var scopeIndex = CurrentScope?.Children.Count ?? 0;
      var metadata = new TestMetadata(
        Description: Description,
        ScopeIndex: scopeIndex,
        LineNumber: LineNumber,
        FilePath: FilePath,
        IsOnly: IsOnly,
        IsSkipped: IsSkipped,
        Timeout: Timeout ?? CurrentScope?.Metadata.Timeout ?? DefaultTimeout
      );
      if (RootScope == null)
      {
        CurrentScope = new TestScope(null, metadata);
        RootScope = CurrentScope;
      }
      else
      {
        var parent = CurrentScope;
        CurrentScope = new TestScope(parent, metadata);
        parent?.Children.Add(CurrentScope);
      }

      body();
      // Pop back to the parent scope after running all the inner scopes
      CurrentScope = CurrentScopeNotNull.Parent;
    }

    /// <summary>
    /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
    /// Use <see cref="As(Action)"/> instead.
    /// </summary>
    /// <param name="body"></param>
    /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
    [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
    public void As(Func<Task> body)
    {
      throw new InvalidOperationException(InvalidDescribeAsyncMethodCallMessage);
    }
  }

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Values">The values to enumerate. A Describe block will be generated for every element in the list.</param>
  /// <param name="Description">The description of the tests. This supports a format string taking</param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="Timeout">The timeout for the test</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record DescribeEachBlock<T>(
    IEnumerable<T> Values,
    Func<T, string> Description,
    bool IsOnly,
    bool IsSkipped,
    TimeSpan? Timeout,
    int LineNumber,
    string FilePath
  )
  {
    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void As(Action<T> body)
    {
      foreach (var value in Values)
      {
        // Capture the value in a local variable to avoid it changing in the closure
        var val = value;
        new DescribeBlock(
          Description: Description(val),
          LineNumber: LineNumber,
          FilePath: FilePath,
          IsOnly: IsOnly,
          IsSkipped: IsSkipped,
          Timeout: Timeout
        ).As(() => body(val));
      }
    }

    /// <summary>
    /// Descriptions must be synchronous, and async bodies should be moved to <see cref="BeforeAll(Func{Task})"/>, <see cref="BeforeEach(Func{Task})" />.
    /// Use <see cref="As(Action{T})"/> instead.
    /// </summary>
    /// <param name="body"></param>
    /// <exception cref="InvalidOperationException">This method will always throw an exception.</exception>
    [Obsolete(InvalidDescribeAsyncMethodCallMessage, error: true)]
    public void As(Func<T, Task> body)
    {
      throw new InvalidOperationException(InvalidDescribeAsyncMethodCallMessage);
    }
  }
}
