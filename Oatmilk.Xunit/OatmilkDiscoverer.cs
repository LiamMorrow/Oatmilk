using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Oatmilk.Xunit;

/// <summary>
/// Marks a test method as a method containing Oatmilk tests described with the various
/// <see cref="TestBuilder"/> methods.
/// Alternatively use the <see cref="DescribeAttribute"/> to begin a describe block implicitly.
/// </summary>
/// <example>
/// <code>
/// [Oatmilk]
/// public void Spec()
/// {
///  Describe("A test suite", () =>
///  {
///     It("should pass", () => Assert.True(true));
///  });
///  }
///  </code>
///  </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Oatmilk.Xunit.OatmilkDiscoverer", "Oatmilk.Xunit")]
public sealed class OatmilkAttribute : FactAttribute { }

internal class OatmilkDiscoverer : IXunitTestCaseDiscoverer
{
  internal static IEnumerable<OatmilkXunitTestCase> TraverseScopesAndYieldTestCases(
    TestScope testScope,
    ITestMethod callingMethod,
    bool anyOnlyTestsInEntireScope
  )
  {
    foreach (var testMethod in testScope.TestMethods)
    {
      yield return new OatmilkXunitTestCase(
        testScope,
        testMethod,
        callingMethod,
        anyOnlyTestsInEntireScope
      );
    }

    foreach (var childScope in testScope.Children)
    {
      foreach (
        var testCase in TraverseScopesAndYieldTestCases(
          childScope,
          callingMethod,
          anyOnlyTestsInEntireScope
        )
      )
      {
        yield return testCase;
      }
    }
  }

  protected virtual TestScope GetRootScope(ITestMethod tm, IAttributeInfo attribute)
  {
    var instance = Activator.CreateInstance(tm.TestClass.Class.ToRuntimeType());
    tm.Method.ToRuntimeMethod().Invoke(instance, null);
    return TestBuilder.ConsumeRootScope();
  }

  public IEnumerable<IXunitTestCase> Discover(
    ITestFrameworkDiscoveryOptions discoveryOptions,
    ITestMethod tm,
    IAttributeInfo factAttribute
  )
  {
    var rootScope = GetRootScope(tm, factAttribute);
    return TraverseScopesAndYieldTestCases(rootScope, tm, rootScope.AnyScopesOrTestsAreOnly);
  }
}
