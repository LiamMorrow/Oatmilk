using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Detest;

internal class DetestXunitTestCaseRunner(
    TestScope testScope,
    DetestXunitTestCase detestXunitTestCase,
    IMessageBus messageBus,
    ExceptionAggregator aggregator
)
{
    private readonly TestScope testScope = testScope;
    private readonly DetestXunitTestCase detestXunitTestCase = detestXunitTestCase;

    public async Task<RunSummary> RunAsync()
    {
        var result = new RunSummary();

        await RunBeforeAllsIncludingParentsAsync(testScope);

        result.Aggregate(await RunTestsInScopeAsync(testScope));

        foreach (var child in testScope.Children)
        {
            result.Aggregate(await RunTestsInScopeAsync(child));
        }

        await RunAfterAllsIncludingParentsAsync(testScope);

        return result;
    }

    private async Task RunBeforeAllsIncludingParentsAsync(TestScope testScope)
    {
        if (testScope.Parent != null)
        {
            await RunBeforeAllsIncludingParentsAsync(testScope.Parent);
        }
        foreach (var beforeAll in testScope.TestBeforeAlls)
        {
            await beforeAll.Body.Invoke();
        }
    }

    private async Task RunAfterAllsIncludingParentsAsync(TestScope testScope)
    {
        foreach (var afterAll in testScope.TestAfterAlls)
        {
            await afterAll.Body.Invoke();
        }
        if (testScope.Parent != null)
        {
            await RunAfterAllsIncludingParentsAsync(testScope.Parent);
        }
    }

    private async Task<RunSummary> RunTestsInScopeAsync(TestScope testScope)
    {
        var result = new RunSummary();
        foreach (var t in testScope.TestMethods)
        {
            result.Total++;
            await RunBeforeEachesIncludingParentsAsync(testScope);
            ITest test = new XunitTest(detestXunitTestCase, GetDescription(t, testScope));
            messageBus.QueueMessage(new TestStarting(test));
            var sw = Stopwatch.StartNew();
            try
            {
                await t.Body.Invoke();
                result.Time += sw.ElapsedMilliseconds / 1000m;
                messageBus.QueueMessage(new TestPassed(test, sw.ElapsedMilliseconds / 1000m, ""));
                messageBus.QueueMessage(new TestFinished(test, sw.ElapsedMilliseconds / 1000m, ""));
            }
            catch (Exception ex)
            {
                messageBus.QueueMessage(
                    new TestFailed(test, sw.ElapsedMilliseconds / 1000m, "", ex)
                );
                aggregator.Add(ex);
                result.Failed++;
            }
            await RunAfterEachesIncludingParentsAsync(testScope);
        }

        foreach (var child in testScope.Children)
        {
            result.Aggregate(await RunTestsInScopeAsync(child));
        }
        return result;
    }

    private async Task RunAfterEachesIncludingParentsAsync(TestScope testScope)
    {
        foreach (var afterAll in testScope.TestAfterEachs)
        {
            await afterAll.Body.Invoke();
        }
        if (testScope.Parent != null)
        {
            await RunAfterEachesIncludingParentsAsync(testScope.Parent);
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

    private string GetDescription(TestExecutionMethod t, TestScope scope)
    {
        var sb = new StringBuilder();
        var parent = scope.Parent;
        while (parent != null)
        {
            sb.Insert(0, parent.Description + " ");
            parent = parent.Parent;
        }
        sb.Append(scope.Description).Append(' ').Append(t.Description);
        return sb.ToString();
    }
}
