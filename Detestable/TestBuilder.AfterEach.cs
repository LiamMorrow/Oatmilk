namespace Detestable;

public static partial class TestBuilder
{
  /// <summary>
  /// Adds a callback to run after each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// The callback is passed a <see cref="FinishedTestContext" /> object which contains information about the test that just ran.
  /// </summary>
  /// <param name="body">The callback to run after each test in the current scope.</param>
  public static void AfterEach(Func<Task> body) => AfterEach(ctx => body());

  /// <summary>
  /// Adds a callback to run after each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// The callback is passed a <see cref="FinishedTestContext" /> object which contains information about the test that just ran.
  /// </summary>
  /// <param name="body">The callback to run after each test in the current scope.</param>
  public static void AfterEach(Action<FinishedTestContext> body) =>
    AfterEach(ctx =>
    {
      body(ctx);
      return Task.CompletedTask;
    });

  /// <summary>
  /// Adds a callback to run after each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// The callback is passed a <see cref="FinishedTestContext" /> object which contains information about the test that just ran.
  /// </summary>
  /// <param name="body">The callback to run after each test in the current scope.</param>
  public static void AfterEach(Func<FinishedTestContext, Task> body) =>
    CurrentScopeNotNull.TestAfterEachs.Add(new TestAfterEachMethod(body));

  /// <summary>
  /// Adds a callback to run after each test in the current scope.
  /// This will run once for each test in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run after each test in the current scope.</param>
  public static void AfterEach(Action body) => AfterEach(ctx => body());
}
