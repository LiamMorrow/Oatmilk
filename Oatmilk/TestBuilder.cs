namespace Oatmilk;

/// <summary>
/// Provides methods for building test suites using a declarative syntax.
/// Generally should be used with the <c>using static Oatmilk.TestBuilder</c> directive.
/// </summary>
public static partial class TestBuilder
{
  [ThreadStatic]
  internal static TestScope? CurrentScope;

  [ThreadStatic]
  internal static TestScope? RootScope;

  internal static TestScope ConsumeRootScope()
  {
    if (RootScope == null)
    {
      throw new InvalidOperationException("No root scope. Has Describe been called?");
    }
    var rs = RootScope;
    RootScope = null;
    return rs;
  }

  internal static TestScope CurrentScopeNotNull =>
    CurrentScope
    ?? throw new InvalidOperationException("No current scope. Has Describe been called?");
}
