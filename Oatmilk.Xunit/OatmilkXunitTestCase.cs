using Oatmilk.Internal;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Oatmilk.Xunit;

[Serializable]
internal partial class OatmilkXunitTestCase(
  TestScope testScope,
  TestBlock testBlock,
  ITestMethod callingMethod,
  bool AnyOnlyTestsInEntireScope
) : IXunitTestCase
{
  [Obsolete("Here for serializable")]
  public OatmilkXunitTestCase()
    : this(null!, null!, null!, false) { }

  public Exception? InitializationException { get; }
  public IMethodInfo Method => TestMethod.Method;
  public int Timeout => (int)TestBlock.Metadata.Timeout.TotalSeconds;
  public string DisplayName => TestBlock.GetDescription(TestScope);
  public string? SkipReason =>
    TestBlock.Metadata.IsSkipped || TestScope.AnyParentsOrThis(x => x.Metadata.IsSkipped)
      ? "Used a Skip method"
      : AnyOnlyTestsInEntireScope
      && !TestBlock.Metadata.IsOnly
      && !TestScope.AnyParentsOrThis(x => x.Metadata.IsOnly)
        ? "Only tests are present in this scope"
        : null;
  public ISourceInformation? SourceInformation
  {
    get
    {
      return new SourceInformation
      {
        FileName = TestBlock.Metadata.FilePath,
        LineNumber = TestBlock.Metadata.LineNumber
      };
    }
    set { }
  }
  public TestScope TestScope { get; set; } = testScope;
  public TestBlock TestBlock { get; set; } = testBlock;
  public ITestMethod TestMethod { get; set; } = callingMethod;
  public object[] TestMethodArguments { get; set; } = [];
  public Dictionary<string, List<string>> Traits
  {
    get =>
      new()
      {
        {
          "Category",
          new List<string> { "Oatmilk" }
        },
        {
          "Name",
          new List<string> { DisplayName }
        }
      };
    set { }
  }

  public string UniqueID =>
    $"{TestBlock.Metadata.FilePath}:{TestBlock.Metadata.LineNumber}:{TestBlock.Metadata.ScopeIndex}";

  public async Task<RunSummary> RunAsync(
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    object[] constructorArguments,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource
  )
  {
    var oatmilkMessabeBus = new XunitOatmilkMessageBus(messageBus, this);
    if (SkipReason != null)
    {
      oatmilkMessabeBus.OnTestSkipped(TestBlock, TestScope, SkipReason);
      return new RunSummary { Total = 1, Skipped = 1 };
    }
    var result = await new OatmilkTestBlockRunner(
      TestScope,
      TestBlock,
      oatmilkMessabeBus
    ).RunAsync();

    return new RunSummary
    {
      Total = result.Total,
      Failed = result.Failed,
      Skipped = result.Skipped,
      Time = (decimal)result.Time.TotalSeconds
    };
  }

  public void Serialize(IXunitSerializationInfo data)
  {
    data.AddValue("TestMethod", TestMethod);
    data.AddValue("TestMethodArguments", TestMethodArguments);

    data.AddValue("TestBlock.FilePath", TestBlock.Metadata.FilePath);
    data.AddValue("TestBlock.LineNumber", TestBlock.Metadata.LineNumber);
    data.AddValue("TestBlock.ScopeIndex", TestBlock.Metadata.ScopeIndex);
    data.AddValue("TestBlock.Description", TestBlock.Metadata.Description);
  }

  public void Deserialize(IXunitSerializationInfo data)
  {
    TestMethod = data.GetValue<ITestMethod>("TestMethod");
    TestMethodArguments = data.GetValue<object[]>("TestMethodArguments");

    var instance = Activator.CreateInstance(TestMethod.TestClass.Class.ToRuntimeType());
    var describeAttribute = TestMethod
      .Method.GetCustomAttributes(typeof(DescribeAttribute))
      .FirstOrDefault();
    if (describeAttribute != null)
    {
      TestBuilder.Describe(
        describeAttribute.GetNamedArgument<string>(nameof(DescribeAttribute.Description)),
        () => TestMethod.Method.ToRuntimeMethod().Invoke(instance, null),
        TimeSpan.FromSeconds(
          describeAttribute.GetNamedArgument<int>(nameof(DescribeAttribute.Timeout))
        ),
        describeAttribute.GetNamedArgument<int>(nameof(DescribeAttribute.LineNumber)),
        describeAttribute.GetNamedArgument<string>(nameof(DescribeAttribute.FileName))
      );
    }
    else
    {
      TestMethod.Method.ToRuntimeMethod().Invoke(instance, null);
    }

    var rootScope = TestBuilder.ConsumeRootScope();

    AnyOnlyTestsInEntireScope = rootScope.AnyScopesOrTestsAreOnly;

    var filePath = data.GetValue<string>("TestBlock.FilePath");
    var lineNumber = data.GetValue<int>("TestBlock.LineNumber");
    var ScopeIndex = data.GetValue<int>("TestBlock.ScopeIndex");
    var description = data.GetValue<string>("TestBlock.Description");

    (TestBlock, TestScope) = rootScope
      .EnumerateTests()
      .Single(t =>
        t.TestBlock.Metadata.FilePath == filePath
        && t.TestBlock.Metadata.LineNumber == lineNumber
        && t.TestBlock.Metadata.ScopeIndex == ScopeIndex
        && t.TestBlock.Metadata.Description == description
      );
  }
}
