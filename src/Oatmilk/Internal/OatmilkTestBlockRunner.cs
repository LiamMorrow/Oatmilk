using System.Diagnostics;

namespace Oatmilk.Internal;

internal class OatmilkTestBlockRunner(
  TestScope testScope,
  TestBlock testBlock,
  IOatmilkMessageBus messageBus
)
{
  private readonly TestScope testScope = testScope;

  public async Task<OatmilkRunSummary> RunAsync(bool skipDueToParentScopeOnly)
  {
    var tokenTimeout = new CancellationTokenSource();
    var testOutputSink = new TestOutputSink();
    var testInput = new TestInput(testOutputSink, tokenTimeout.Token);

    var result = new OatmilkRunSummary(Total: 1);

    if (skipDueToParentScopeOnly)
    {
      messageBus.OnTestSkipped(testBlock, testScope, "Parent scope has an only block");
      return result with { Skipped = 1 };
    }
    if (testBlock.ShouldSkipDueToIsSkippedOnThisOrParent(testScope))
    {
      messageBus.OnTestSkipped(testBlock, testScope, "Skipped using a Skip method");
      return result with { Skipped = 1 };
    }

    var sw = Stopwatch.StartNew();

    messageBus.OnBeforeTestSetupStarting(testBlock, testScope);
    await RunBeforeAllsIncludingParentsAsync(testScope, testInput);
    await RunBeforeEachesIncludingParentsAsync(testScope, testInput);
    messageBus.OnBeforeTestSetupFinished(testBlock, testScope);

    messageBus.OnTestStarting(testBlock, testScope);
    var finishedTestContext = new FinishedTestContext(
      true,
      testOutputSink.GetOutput(),
      testBlock.Metadata.Description
    );
    try
    {
      tokenTimeout.CancelAfter(testBlock.Metadata.Timeout);
      var testRun = testBlock.Body.Invoke(testInput);
      await Task.WhenAny(testRun, Task.Delay(testBlock.Metadata.Timeout));
      if (!testRun.IsCompleted)
      {
        throw new TimeoutException(
          $"Test timed out after {testBlock.Metadata.Timeout.TotalMilliseconds}ms"
        );
      }
      await testRun;
      result = result with { Time = sw.Elapsed, Passed = 1 };
      messageBus.OnTestPassed(testBlock, testScope, result.Time, testOutputSink.GetOutput().Output);
    }
    catch (Exception ex)
    {
      result = result with { Time = sw.Elapsed, Failed = 1 };
      messageBus.OnTestFailed(
        testBlock,
        testScope,
        ex,
        result.Time,
        testOutputSink.GetOutput().Output
      );
      finishedTestContext = finishedTestContext with { Passed = false };
    }

    messageBus.OnTestFinished(testBlock, testScope, result.Time, testOutputSink.GetOutput().Output);
    testBlock.HasRun = true;

    messageBus.OnAfterTestSetupStarting(testBlock, testScope);
    await RunAfterEachesIncludingParentsAsync(finishedTestContext, testScope);
    await RunAfterAllsIncludingParentsAsync(testScope);
    messageBus.OnAfterTestSetupFinished(testBlock, testScope);

    return result;
  }

  private async Task RunBeforeAllsIncludingParentsAsync(TestScope testScope, TestInput testInput)
  {
    if (testScope.Parent != null)
    {
      await RunBeforeAllsIncludingParentsAsync(testScope.Parent, testInput);
    }
    if (testScope.HasRunBeforeAlls)
    {
      return;
    }
    foreach (var beforeAll in testScope.TestBeforeAlls)
    {
      await beforeAll.Body(testInput);
    }
    testScope.HasRunBeforeAlls = true;
  }

  private async Task RunAfterAllsIncludingParentsAsync(TestScope testScope)
  {
    if (testScope.HasRunAfterAlls || !testScope.HasRunAllTests)
    {
      return;
    }
    foreach (var afterAll in testScope.TestAfterAlls)
    {
      await afterAll.Body.Invoke();
    }
    if (testScope.Parent != null)
    {
      await RunAfterAllsIncludingParentsAsync(testScope.Parent);
    }
    testScope.HasRunAfterAlls = true;
  }

  private async Task RunAfterEachesIncludingParentsAsync(
    FinishedTestContext finishedTestContext,
    TestScope testScope
  )
  {
    foreach (var afterEach in testScope.TestAfterEachs)
    {
      await afterEach.Body.Invoke(finishedTestContext);
    }
    if (testScope.Parent != null)
    {
      await RunAfterEachesIncludingParentsAsync(finishedTestContext, testScope.Parent);
    }
  }

  private async Task RunBeforeEachesIncludingParentsAsync(TestScope testScope, TestInput testInput)
  {
    if (testScope.Parent != null)
    {
      await RunBeforeEachesIncludingParentsAsync(testScope.Parent, testInput);
    }
    foreach (var beforeEach in testScope.TestBeforeEachs)
    {
      await beforeEach.Body(testInput);
    }
  }
}
