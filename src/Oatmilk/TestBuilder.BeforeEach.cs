namespace Oatmilk;

public static partial class TestBuilder
{
  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Func<TestInput, Task> body) =>
    CurrentScopeNotNull.TestBeforeEachs.Add(new TestSetupMethod(body));

  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Action<TestInput> body) =>
    BeforeEach(
      (testInput) =>
      {
        body(testInput);
        return Task.CompletedTask;
      }
    );

  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Func<Task> body) =>
    CurrentScopeNotNull.TestBeforeEachs.Add(new TestSetupMethod(_ => body()));

  /// <summary>
  /// Adds a callback to run before each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before each test in the current scope.</param>
  public static void BeforeEach(Action body) => BeforeEach(t => body());
}
