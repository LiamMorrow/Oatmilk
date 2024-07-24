using System.Diagnostics;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Detest;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Detest.DetestDiscoverer", "Detest")]
public sealed class DetestAttribute : FactAttribute { }

public class DetestDiscoverer() : IXunitTestCaseDiscoverer
{
    public IEnumerable<IXunitTestCase> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        ITestMethod tm,
        IAttributeInfo factAttribute
    )
    {
        var instance = Activator.CreateInstance(tm.TestClass.Class.ToRuntimeType());
        tm.Method.ToRuntimeMethod().Invoke(instance, null);
        var testScope = TestBuilder.ConsumeRootScope();

        yield return new DetestXunitTestCase(testScope, tm);
    }
}

internal partial class DetestXunitTestCase(TestScope testScope, ITestMethod callingMethod)
    : IXunitTestCase
{
    public Exception? InitializationException { get; }
    public IMethodInfo Method { get; } = callingMethod.Method;
    public int Timeout { get; } = 1000;
    public string DisplayName { get; } = testScope.Description;
    public string? SkipReason { get; }
    public ISourceInformation? SourceInformation { get; set; }
    public ITestMethod TestMethod => callingMethod;
    public object[] TestMethodArguments { get; } = [];
    public Dictionary<string, List<string>> Traits { get; } = new();
    public string UniqueID { get; } = Guid.NewGuid().ToString();

    public void Deserialize(IXunitSerializationInfo info)
    {
        throw new NotImplementedException();
    }

    public Task<RunSummary> RunAsync(
        IMessageSink diagnosticMessageSink,
        IMessageBus messageBus,
        object[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource
    )
    {
        return new DetestXunitTestCaseRunner(testScope, this, messageBus, aggregator).RunAsync();
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        throw new NotImplementedException();
    }
}
