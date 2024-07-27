using System.Runtime.CompilerServices;

namespace Detestable;

/// <summary>
/// Provides methods for building test suites using a declarative syntax.
/// Generally should be used with the <c>using static Detestable.TestBuilder</c> directive.
/// </summary>
public static partial class TestBuilder
{
  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Func<Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="body">The test body. Assertions should go in here</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static void It(
    string description,
    Action body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => It(description, lineNumber, filePath).When(body);

  /// <summary>
  /// Adds a test to the current scope.
  /// The body of the test will be run when the test is executed.
  /// </summary>
  /// <param name="description">The description of the test</param>
  /// <param name="lineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="filePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public static ItBlock It(
    string description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => new(description, false, false, lineNumber, filePath);

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Description">The description of the test</param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record ItBlock(
    string Description,
    bool IsOnly,
    bool IsSkipped,
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
      When(() =>
      {
        body();
        return Task.CompletedTask;
      });
    }

    /// <summary>
    /// Describes the test body.
    /// This will run when the test is executed.
    /// </summary>
    /// <param name="body">The test body. Assertions should go in here.</param>
    public void When(Func<Task> body)
    {
      var tm = new TestBlock(
        body,
        new(
          Description: Description,
          ScopeIndex: CurrentScopeNotNull.TestMethods.Count,
          LineNumber: LineNumber,
          FilePath: FilePath,
          IsOnly: IsOnly,
          IsSkipped: IsSkipped
        )
      );
      CurrentScopeNotNull.TestMethods.Add(tm);
    }
  }

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Description">The description of the tests. This supports a format string taking</param>
  /// <param name="IsOnly">Should be the only test run in the suite</param>
  /// <param name="IsSkipped">Should be skipped</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record ItEachBlock<T>(
    IEnumerable<T> Values,
    Func<T, string> Description,
    bool IsOnly,
    bool IsSkipped,
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
        (val) =>
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
    public void When(Func<T, Task> body)
    {
      foreach (var value in Values)
      {
        var val = value;
        var tm = new TestBlock(
          () => body(val),
          new(
            Description: Description(val),
            ScopeIndex: CurrentScopeNotNull.TestMethods.Count,
            LineNumber: LineNumber,
            FilePath: FilePath,
            IsOnly: IsOnly,
            IsSkipped: IsSkipped
          )
        );
        CurrentScopeNotNull.TestMethods.Add(tm);
      }
    }
  }
}
