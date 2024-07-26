using System;
using System.Diagnostics;
using System.Text;

namespace Detest.Core;

internal class DetestTestBlockRunner(
  TestScope testScope,
  TestBlock testBlock,
  IDetestMessageBus messageBus
)
{
  private readonly TestScope testScope = testScope;

  public async Task<DetestRunSummary> RunAsync()
  {
    var result = new DetestRunSummary();

    await RunBeforeAllsIncludingParentsAsync(testScope);

    result.Aggregate(await RunTestAsync(testScope));

    await RunAfterAllsIncludingParentsAsync(testScope);

    return result;
  }

  private async Task RunBeforeAllsIncludingParentsAsync(TestScope testScope)
  {
    if (testScope.Parent != null)
    {
      await RunBeforeAllsIncludingParentsAsync(testScope.Parent);
    }
    if (testScope.HasRunBeforeAlls)
    {
      return;
    }
    foreach (var beforeAll in testScope.TestBeforeAlls)
    {
      await beforeAll.Body.Invoke();
    }
    testScope.HasRunBeforeAlls = true;
  }

  private async Task RunAfterAllsIncludingParentsAsync(TestScope testScope)
  {
    if (testScope.HasRunAfterAlls)
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

  private async Task<DetestRunSummary> RunTestAsync(TestScope testScope)
  {
    var result = new DetestRunSummary(Total: 1);
    await RunBeforeEachesIncludingParentsAsync(testScope);
    messageBus.OnTestStarting(testBlock, testScope);
    var sw = Stopwatch.StartNew();
    var finishedTestContext = new FinishedTestContext(true, testBlock.GetDescription(testScope));
    try
    {
      await testBlock.Body.Invoke();
      result = result with { Time = sw.Elapsed };
      messageBus.OnTestPassed(testBlock, testScope, result.Time, "");
    }
    catch (Exception ex)
    {
      result = result with { Time = sw.Elapsed, Failed = 1 };
      messageBus.OnTestFailed(testBlock, testScope, ex, result.Time, "");
      finishedTestContext = finishedTestContext with { Passed = false };
    }
    await RunAfterEachesIncludingParentsAsync(finishedTestContext, testScope);
    messageBus.OnTestFinished(testBlock, testScope, result.Time, "");

    return result;
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

  private async Task RunBeforeEachesIncludingParentsAsync(TestScope testScope)
  {
    if (testScope.Parent != null)
    {
      await RunBeforeEachesIncludingParentsAsync(testScope.Parent);
    }
    foreach (var beforeAll in testScope.TestBeforeEachs)
    {
      await beforeAll.Body.Invoke();
    }
  }
}
