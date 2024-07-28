namespace Oatmilk;

public static partial class TestBuilder
{
  /// <summary>
  /// Adds a callback to run after all tests in the current scope.
  /// This will run only once, after all of the tests in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run after all tests in the current scope.</param>
  public static void AfterAll(Func<Task> body) =>
    CurrentScopeNotNull.TestAfterAlls.Add(new TestSetupMethod(body));

  /// <summary>
  /// Adds a callback to run after all tests in the current scope.
  /// This will run only once, after all of the tests in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run after all tests in the current scope.</param>
  public static void AfterAll(Action body) =>
    AfterAll(() =>
    {
      body();
      return Task.CompletedTask;
    });
}
