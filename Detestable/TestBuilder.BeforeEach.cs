namespace Detestable;

/// <summary>
/// Provides methods for building test suites using a declarative syntax.
/// Generally should be used with the <c>using static Detestable.TestBuilder</c> directive.
/// </summary>
public static partial class TestBuilder
{
  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Func<Task> body) =>
    CurrentScopeNotNull.TestBeforeEachs.Add(new TestSetupMethod(body));

  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Action body) =>
    BeforeEach(() =>
    {
      body();
      return Task.CompletedTask;
    });
}
