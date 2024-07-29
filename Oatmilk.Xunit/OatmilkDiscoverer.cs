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
public sealed class OatmilkAttribute : FactAttribute;

internal class OatmilkDiscoverer : IXunitTestCaseDiscoverer
{
  internal static IEnumerable<OatmilkXunitTestCase> TraverseScopesAndYieldTestCases(
    TestScope testScope,
    ITestMethod callingMethod,
    bool anyOnlyTestsInEntireScope
  ) =>
    TraverseScopesAndYieldTestBlocks(testScope, anyOnlyTestsInEntireScope)
      .Select(x => new OatmilkXunitTestCase(
        x.TestScope,
        x.TestBlock,
        callingMethod,
        anyOnlyTestsInEntireScope
      ));

  internal static IEnumerable<(
    TestScope TestScope,
    TestBlock TestBlock
  )> TraverseScopesAndYieldTestBlocks(TestScope testScope, bool anyOnlyTestsInEntireScope)
  {
    foreach (var testBlock in testScope.TestBlocks)
    {
      yield return (testScope, testBlock);
    }

    foreach (var childScope in testScope.Children)
    {
      foreach (
        var testBlock in TraverseScopesAndYieldTestBlocks(childScope, anyOnlyTestsInEntireScope)
      )
      {
        yield return testBlock;
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
