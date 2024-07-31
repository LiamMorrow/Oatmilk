using System.Reflection;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Oatmilk.Internal;

namespace Oatmilk.Nunit;

/// <summary>
/// An Nunit TestMethod which is described by a TestScope, it is not a test in itself, just contains children tests.
/// </summary>
internal class OatmilkNunitTestScopeTest : TestMethod
{
  internal readonly TestScope TestScope;

  public OatmilkNunitTestScopeTest(TestScope testScope)
    : base(GetMethod())
  {
    Console.WriteLine("OatmilkNunitTestScopeTest");
    this.TestScope = testScope;
  }

  public override object?[] Arguments => [this];

  private static MethodWrapper GetMethod()
  {
    return new MethodWrapper(typeof(OatmilkTestBase), nameof(OatmilkTestBase.RunScopeAsync));
  }

  public override bool HasChildren => TestScope.EnumerateTests().Any();

  private IList<ITest>? _tests;
  public override IList<ITest> Tests =>
    _tests ??= TestScope
      .TestBlocks.Select(x => new OatmilkNunitTestBlockTest(TestScope, x) as ITest)
      .Concat(TestScope.Children.Select(x => new OatmilkNunitTestScopeTest(x)))
      .ToList();

  public override int TestCaseCount => Tests.Count;

  public override TNode AddToXml(TNode parentNode, bool recursive)
  {
    return base.AddToXml(parentNode, recursive);
  }
}
