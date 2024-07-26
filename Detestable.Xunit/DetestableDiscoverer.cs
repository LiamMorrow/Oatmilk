using Detestable.Internal;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Detestable.Xunit;

/// <summary>
/// Marks a test method as a method containing Detestable tests described with the various
/// <see cref="TestBuilder"/> methods.
/// Alternatively use the <see cref="DescribeAttribute"/> to begin a describe block implicitly.
/// </summary>
/// <example>
/// <code>
/// [Detestable]
/// public void Spec()
/// {
///  Describe("A test suite", () =>
///  {
///     It("should pass", () => Assert.True(true));
///  });
///  }
///  </code>
///  </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Detestable.Xunit.DetestableDiscoverer", "Detestable.Xunit")]
public sealed class DetestableAttribute : FactAttribute { }

/// <summary>
/// Marks a test method as a method containing Detestable tests described with the various
/// <see cref="TestBuilder"/> methods. Implicitly begins a describe block.
/// </summary>
/// <example>
/// <code>
/// [Describe("A test suite")]
/// public void Spec()
/// {
///    It("should pass", () => Assert.True(true));
/// }
/// </code>
/// </example>

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("Detestable.Xunit.DescribeDiscoverer", "Detestable.Xunit")]
public sealed class DescribeAttribute(string Description) : FactAttribute
{
  /// <summary>
  /// The description of the test suite.
  /// This is passed to the <see cref="TestBuilder.Describe(string, Action)"/> method.
  /// </summary>
  public string Description { get; } = Description;
}

internal class DescribeDiscoverer : DetestableDiscoverer
{
  protected override TestScope GetTestScope(ITestMethod tm, IAttributeInfo attribute)
  {
    var instance = Activator.CreateInstance(tm.TestClass.Class.ToRuntimeType());
    TestBuilder.Describe(
      attribute.GetNamedArgument<string>("Description"),
      () => tm.Method.ToRuntimeMethod().Invoke(instance, null)
    );
    return TestBuilder.ConsumeRootScope();
  }
}

internal class DetestableDiscoverer : IXunitTestCaseDiscoverer
{
  internal static IEnumerable<IXunitTestCase> TraverseScopesAndYieldTestCases(
    TestScope testScope,
    ITestMethod callingMethod
  )
  {
    foreach (var testMethod in testScope.TestMethods)
    {
      yield return new DetestableXunitTestCase(testScope, testMethod, callingMethod);
    }

    foreach (var childScope in testScope.Children)
    {
      foreach (var testCase in TraverseScopesAndYieldTestCases(childScope, callingMethod))
      {
        yield return testCase;
      }
    }
  }

  protected virtual TestScope GetTestScope(ITestMethod tm, IAttributeInfo attribute)
  {
    var instance = Activator.CreateInstance(tm.TestClass.Class.ToRuntimeType());
    tm.Method.ToRuntimeMethod().Invoke(instance, null);
    return TestBuilder.ConsumeRootScope();
  }

  public IEnumerable<IXunitTestCase> Discover(
    ITestFrameworkDiscoveryOptions discoveryOptions,
    ITestMethod tm,
    IAttributeInfo factAttribute
  )
  {
    var testScope = GetTestScope(tm, factAttribute);
    return TraverseScopesAndYieldTestCases(testScope, tm);
  }
}

[Serializable]
internal partial class DetestableXunitTestCase(
  TestScope testScope,
  TestBlock testExecutionMethod,
  ITestMethod callingMethod
) : IXunitTestCase
{
  [Obsolete("Here for serializable")]
  public DetestableXunitTestCase()
    : this(null!, null!, null!) { }

  public Exception? InitializationException { get; }
  public IMethodInfo Method => TestMethod.Method;
  public int Timeout { get; } = 1000;
  public string DisplayName => TestBlock.GetDescription(TestScope);
  public string? SkipReason { get; }
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
  public TestBlock TestBlock { get; set; } = testExecutionMethod;
  public ITestMethod TestMethod { get; set; } = callingMethod;
  public object[] TestMethodArguments { get; set; } = [];
  public Dictionary<string, List<string>> Traits { get; set; } = [];
  public string UniqueID =>
    $"{TestBlock.Metadata.FilePath}:{TestBlock.Metadata.LineNumber}:{TestBlock.Metadata.ScopeIndex}";

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
        describeAttribute.GetNamedArgument<string>("Description"),
        () => TestMethod.Method.ToRuntimeMethod().Invoke(instance, null)
      );
    }
    else
    {
      TestMethod.Method.ToRuntimeMethod().Invoke(instance, null);
    }

    var filePath = data.GetValue<string>("TestBlock.FilePath");
    var lineNumber = data.GetValue<int>("TestBlock.LineNumber");
    var ScopeIndex = data.GetValue<int>("TestBlock.ScopeIndex");
    var description = data.GetValue<string>("TestBlock.Description");

    (TestBlock, TestScope) = TestBuilder
      .ConsumeRootScope()
      .EnumerateTests()
      .Single(t =>
        t.TestBlock.Metadata.FilePath == filePath
        && t.TestBlock.Metadata.LineNumber == lineNumber
        && t.TestBlock.Metadata.ScopeIndex == ScopeIndex
        && t.TestBlock.Metadata.Description == description
      );
  }

  public async Task<RunSummary> RunAsync(
    IMessageSink diagnosticMessageSink,
    IMessageBus messageBus,
    object[] constructorArguments,
    ExceptionAggregator aggregator,
    CancellationTokenSource cancellationTokenSource
  )
  {
    var result = await new DetestableTestBlockRunner(
      TestScope,
      TestBlock,
      new XunitDetestableMessageBus(messageBus, this)
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
    data.AddValue("Timeout", Timeout);
    data.AddValue("TestBlock.FilePath", TestBlock.Metadata.FilePath);
    data.AddValue("TestBlock.LineNumber", TestBlock.Metadata.LineNumber);
    data.AddValue("TestBlock.ScopeIndex", TestBlock.Metadata.ScopeIndex);
    data.AddValue("TestBlock.Description", TestBlock.Metadata.Description);
  }
}
