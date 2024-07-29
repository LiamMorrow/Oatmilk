using System.Runtime.CompilerServices;

namespace Oatmilk;

public static partial class TestBuilder
{
  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Func<Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Func<TestInput, Task> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Action body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Action<TestInput> body,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, timeout, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="timeout">The timeout for the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static ItBlock It(
    string description,
    TimeSpan? timeout = null,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => new(description, false, false, timeout, lineNumber, filePath);

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Description">The description of the test</param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="Timeout">The timeout for the test</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record ItBlock(
    string Description,
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
    public void When(Action body)
    {
      When(_ => body());
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Action<TestInput> body)
    {
      When(
        (input) =>
        {
          body(input);
          return Task.CompletedTask;
        }
      );
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Func<Task> body) => When((input) => body());

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Func<TestInput, Task> body)
    {
      var tm = new TestBlock(
        body,
        new(
          Description: Description,
          ScopeIndex: CurrentScopeNotNull.TestBlocks.Count,
          LineNumber: LineNumber,
          FilePath: FilePath,
          IsOnly: IsOnly,
          IsSkipped: IsSkipped,
          Timeout: Timeout ?? CurrentScopeNotNull.Metadata.Timeout
        )
      );
      CurrentScopeNotNull.TestBlocks.Add(tm);
    }
  }

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Values">The values to enumerate. An It block will be generated for every element in the list.</param>
  /// <param name="DescriptionResolver">A callback function to generate the description of the tests.  It is passed each value from <paramref name="Values"/></param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="Timeout">The timeout for the test</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record ItEachBlock<T>(
    IEnumerable<T> Values,
    Func<T, string> DescriptionResolver,
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
    public void When(Action<T> body)
    {
      When(
        (val, testInput) =>
        {
          body(val);
          return Task.CompletedTask;
        }
      );
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Action<T, TestInput> body)
    {
      When(
        (val, testInput) =>
        {
          body(val, testInput);
          return Task.CompletedTask;
        }
      );
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Func<T, Task> body)
    {
      When((val, testInput) => body(val));
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Func<T, TestInput, Task> body)
    {
      foreach (var value in Values)
      {
        var val = value;
        var tm = new TestBlock(
          (testInput) => body(val, testInput),
          new(
            Description: DescriptionResolver(val),
            ScopeIndex: CurrentScopeNotNull.TestBlocks.Count,
            LineNumber: LineNumber,
            FilePath: FilePath,
            IsOnly: IsOnly,
            IsSkipped: IsSkipped,
            Timeout: Timeout ?? CurrentScopeNotNull.Metadata.Timeout
          )
        );
        CurrentScopeNotNull.TestBlocks.Add(tm);
      }
    }
  }
}
