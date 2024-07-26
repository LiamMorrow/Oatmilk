namespace Detest.Core;

internal interface IDetestMessageBus
{
  public void OnTestFailed(
    TestBlock testBlock,
    TestScope testScope,
    Exception ex,
    TimeSpan executionTime,
    string output
  );
  public void OnTestFinished(
    TestBlock testBlock,
    TestScope testScope,
    TimeSpan executionTime,
    string output
  );

  public void OnTestOutput(TestBlock testBlock, TestScope testScope, string output);

  public void OnTestPassed(
    TestBlock testBlock,
    TestScope testScope,
    TimeSpan executionTime,
    string output
  );

  public void OnTestSkipped(TestBlock testBlock, TestScope testScope, string reason);

  public void OnTestStarting(TestBlock testBlock, TestScope testScope);
}
