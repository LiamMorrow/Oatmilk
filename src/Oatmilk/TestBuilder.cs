using Oatmilk.Internal;

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

  /// <summary>
  /// The default timeout for tests. Defaults to 5 seconds.
  /// Can be configured by setting the OATMILK_DEFAULT_TIMEOUT_SECONDS environment variable.
  /// </summary>
  public static TimeSpan DefaultTimeout { get; } = OatmilkConfiguration.DefaultTimeout;

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
