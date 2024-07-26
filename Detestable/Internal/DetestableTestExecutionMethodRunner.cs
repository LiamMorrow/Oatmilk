using System.Diagnostics;

namespace Detestable.Internal;

internal class DetestableTestBlockRunner(
  TestScope testScope,
  TestBlock testBlock,
  IDetestableMessageBus messageBus
)
{
  private readonly TestScope testScope = testScope;

  public async Task<DetestableRunSummary> RunAsync()
  {
    var result = new DetestableRunSummary(Total: 1);
    var sw = Stopwatch.StartNew();

    messageBus.OnBeforeTestSetupStarting(testBlock, testScope);
    await RunBeforeAllsIncludingParentsAsync(testScope);
    await RunBeforeEachesIncludingParentsAsync(testScope);
    messageBus.OnBeforeTestSetupFinished(testBlock, testScope);

    messageBus.OnTestStarting(testBlock, testScope);
    var finishedTestContext = new FinishedTestContext(true);
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

    messageBus.OnTestFinished(testBlock, testScope, result.Time, "");

    messageBus.OnAfterTestSetupStarting(testBlock, testScope);
    await RunAfterEachesIncludingParentsAsync(finishedTestContext, testScope);
    await RunAfterAllsIncludingParentsAsync(testScope);
    messageBus.OnAfterTestSetupFinished(testBlock, testScope);

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
