﻿namespace Oatmilk;

public static partial class TestBuilder
{
  /// <summary>
  /// Adds a callback to run before all tests in the current scope.
  /// This will run only once, before any of the tests in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before all tests in the current scope.</param>
  public static void BeforeAll(Func<Task> body) =>
    CurrentScopeNotNull.TestBeforeAlls.Add(new TestSetupMethod(body));

  /// <summary>
  /// Adds a callback to run before all tests in the current scope.
  /// This will run only once, before any of the tests in the current, and nested scopes.
  /// </summary>
  /// <param name="body">The callback to run before all tests in the current scope.</param>
  public static void BeforeAll(Action body) =>
    BeforeAll(() =>
    {
      body();
      return Task.CompletedTask;
    });
}