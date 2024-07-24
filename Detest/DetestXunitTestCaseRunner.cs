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
    DetestXunitTestCase detestXunitTestCase
)
{
    private readonly TestScope testScope = testScope;
    private readonly DetestXunitTestCase detestXunitTestCase = detestXunitTestCase;

    public async Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource
    )
    {
        var result = new RunSummary();
        foreach (var t in testScope.TestMethods)
        {
            result.Total++;
            ITest test = new XunitTest(detestXunitTestCase, GetDescription(t));
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
        }

        return result;
    }

    private string GetDescription(TestExecutionMethod t)
    {
        var sb = new StringBuilder();
        var parent = testScope.Parent;
        while (parent != null)
        {
            sb.Insert(0, parent.Description + " ");
            parent = parent.Parent;
        }
        sb.Append(testScope.Description).Append(' ').Append(t.Description);
        return sb.ToString();
    }
}
