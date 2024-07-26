using System.Runtime.CompilerServices;

namespace Detestable;

#pragma warning disable RS0026 // Do not add multiple public overloads with optional parameters - we need them to get the line number and file path of the caller
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
  ) => new(description, lineNumber, filePath);

  /// <summary>
  /// Adds a test to the current scope, configured with a fluent API.
  /// </summary>
  /// <param name="Description">The description of the test</param>
  /// <param name="LineNumber">Leave unset, used by the runtime to support running tests via the IDE</param>
  /// <param name="FilePath">Leave unset, used by the runtime to support running tests via the IDE</param>
  public record ItBlock(string Description, int LineNumber, string FilePath)
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
          FilePath: FilePath
        )
      );
      CurrentScopeNotNull.TestMethods.Add(tm);
    }
  }
}
#pragma warning restore RS0026 // Do not add multiple public overloads with optional parameters
