using NUnit.Framework;
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

  public override object?[] Arguments => [TestScope, TestBlock];

  public OatmilkNunitTestBlockTest(TestScope testScope, TestBlock testBlock)
    : base(MakeMethodInfo())
  {
    TestScope = testScope;
    TestBlock = testBlock;
    this.FullName = testBlock.GetDescription(testScope);
    this.Name = testBlock.GetDescription(testScope);

    this.Properties.Add("_CodeFilePath", TestBlock.Metadata.FilePath);
    this.Properties.Add("_LineNumber", TestBlock.Metadata.LineNumber);
    this.Properties.Add("TestBlock.ScopeIndex", TestBlock.Metadata.ScopeIndex);
    this.Properties.Add("TestBlock.Description", TestBlock.Metadata.Description);

    if (TestBlock.GetSkipReason(TestScope) != SkipReason.DoNotSkip)
    {
      this.RunState = RunState.Ignored;
    }
  }

  private static MethodWrapper MakeMethodInfo()
  {
    return new MethodWrapper(typeof(OatmilkNunitTestBlockTest), nameof(RunAsync));
  }

  static Task<OatmilkRunSummary> RunAsync(TestScope scope, TestBlock testBlock)
  {
    return new OatmilkTestBlockRunner(scope, testBlock, new DummyMessageBus()).RunAsync();
  }
}
