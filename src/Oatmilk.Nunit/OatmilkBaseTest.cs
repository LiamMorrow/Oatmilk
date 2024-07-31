using Oatmilk.Internal;

namespace Oatmilk.Nunit;

public class OatmilkTestBase
{
  internal Task RunAsync(TestScope scope, TestBlock block)
  {
    return new OatmilkTestBlockRunner(scope, block, new DummyMessageBus()).RunAsync();
  }

  internal async Task RunScopeAsync(OatmilkNunitTestScopeTest test)
  {
    foreach (var child in test.Tests)
    {
      if (child is OatmilkNunitTestBlockTest block)
      {
        await RunAsync(test.TestScope, block.TestBlock);
      }
      else if (child is OatmilkNunitTestScopeTest scope)
      {
        await RunScopeAsync(scope);
      }
    }
  }
}
