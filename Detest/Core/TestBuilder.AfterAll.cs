using System.Runtime.CompilerServices;

namespace Detest.Core;

/// <summary>
/// Provides methods for building test suites using a declarative syntax.
/// Generally should be used with the <c>using static Detest.Core.TestBuilder</c> directive.
/// </summary>
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
