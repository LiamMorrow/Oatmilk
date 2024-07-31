using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using Oatmilk.Internal;

namespace Oatmilk.Nunit;

/// <summary>
/// An Nunit TestMethod which is described by a TestScope, it is not a test in itself, just contains children tests.
/// </summary>
internal class OatmilkNunitTestBlockTest : TestMethod
{
  internal readonly TestScope TestScope;
  internal readonly TestBlock TestBlock;

  public OatmilkNunitTestBlockTest(TestScope testScope, TestBlock testBlock)
    : base(MakeMethodInfo())
  {
    Console.WriteLine("OatmilkNunitTestBlockTest");
    TestScope = testScope;
    TestBlock = testBlock;
  }

  private static IMethodInfo MakeMethodInfo()
  {
    return new MethodWrapper(typeof(OatmilkTestBase), nameof(OatmilkTestBase.RunAsync));
  }
}
