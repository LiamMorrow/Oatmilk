using Oatmilk.Internal;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Oatmilk.Xunit;

internal class XunitOatmilkMessageBus(IMessageBus messageBus, IXunitTestCase xunitTestMethod)
  : IOatmilkMessageBus
{
  public void OnAfterTestSetupFinished(TestBlock testBlock, TestScope testScope)
  {
    messageBus.QueueMessage(new AfterTestStarting(GetTest(testBlock, testScope), "AfterTest"));
  }

  public void OnAfterTestSetupStarting(TestBlock testBlock, TestScope testScope)
  {
    messageBus.QueueMessage(new AfterTestStarting(GetTest(testBlock, testScope), "AfterTest"));
  }

  public void OnBeforeTestSetupFinished(TestBlock testBlock, TestScope testScope)
  {
    messageBus.QueueMessage(new BeforeTestFinished(GetTest(testBlock, testScope), "BeforeTest"));
  }

  public void OnBeforeTestSetupStarting(TestBlock testBlock, TestScope testScope)
  {
    messageBus.QueueMessage(new BeforeTestStarting(GetTest(testBlock, testScope), "BeforeTest"));
  }

  public void OnTestFailed(
    TestBlock testBlock,
    TestScope testScope,
    Exception ex,
    TimeSpan executionTime,
    string output
  )
  {
    messageBus.QueueMessage(
      new TestFailed(GetTest(testBlock, testScope), (decimal)executionTime.TotalSeconds, output, ex)
    );
  }

  public void OnTestFinished(
    TestBlock testBlock,
    TestScope testScope,
    TimeSpan executionTime,
    string output
  )
  {
    messageBus.QueueMessage(
      new TestFinished(GetTest(testBlock, testScope), (decimal)executionTime.TotalSeconds, output)
    );
  }

  public void OnTestOutput(TestBlock testBlock, TestScope testScope, string output)
  {
    messageBus.QueueMessage(
      new global::Xunit.Sdk.TestOutput(GetTest(testBlock, testScope), output)
    );
  }

  public void OnTestPassed(
    TestBlock testBlock,
    TestScope testScope,
    TimeSpan executionTime,
    string output
  )
  {
    messageBus.QueueMessage(
      new TestPassed(GetTest(testBlock, testScope), (decimal)executionTime.TotalSeconds, output)
    );
  }

  public void OnTestSkipped(TestBlock testBlock, TestScope testScope, string reason)
  {
    messageBus.QueueMessage(new TestSkipped(GetTest(testBlock, testScope), reason));
  }

  public void OnTestStarting(TestBlock testBlock, TestScope testScope)
  {
    messageBus.QueueMessage(new TestStarting(GetTest(testBlock, testScope)));
  }

  private ITest GetTest(TestBlock testBlock, TestScope testScope) =>
    new XunitTest(xunitTestMethod, testBlock.GetDescription(testScope));
}
