using System.Runtime.CompilerServices;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Oatmilk.Xunit;

/// <summary>
/// Marks a test method as a method containing Oatmilk tests described with the various
/// <see cref="TestBuilder"/> methods. Implicitly begins a describe block.
/// </summary>
/// <param name="Description">The description of the test suite.</param>
/// <param name="FileName">The file path of the file containing the test suite.</param>
/// <param name="LineNumber">The line number of the test suite.</param>
///
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
[XunitTestCaseDiscoverer("Oatmilk.Xunit.DescribeDiscoverer", "Oatmilk.Xunit")]
public sealed class DescribeAttribute(
  string Description,
  [CallerFilePath] string FileName = "",
  [CallerLineNumber] int LineNumber = 0
) : FactAttribute
{
  /// <summary>
  /// The description of the test suite.
  /// This is passed to the <see cref="TestBuilder.Describe(string, Action, TimeSpan?, int,string)"/> method.
  /// </summary>
  public string Description { get; } = Description;

  /// <summary>
  /// The file path of the file containing the test suite.
  /// </summary>
  public string FileName { get; } = FileName;

  /// <summary>
  /// The line number of the test suite.
  /// </summary>
  public int LineNumber { get; } = LineNumber;

  /// <inheritdoc/>
  public override int Timeout { get; set; } = TestBuilder.DefaultTimeout.Seconds;
}

internal class DescribeDiscoverer : OatmilkDiscoverer
{
  protected override TestScope GetRootScope(ITestMethod tm, IAttributeInfo attribute)
  {
    var instance = Activator.CreateInstance(tm.TestClass.Class.ToRuntimeType());
    var timeoutSeconds = attribute.GetNamedArgument<int>(nameof(DescribeAttribute.Timeout));
    TestBuilder.Describe(
      attribute.GetNamedArgument<string>("Description"),
      () => tm.Method.ToRuntimeMethod().Invoke(instance, null),
      TimeSpan.FromSeconds(timeoutSeconds),
      attribute.GetNamedArgument<int>(nameof(DescribeAttribute.LineNumber)),
      attribute.GetNamedArgument<string>(nameof(DescribeAttribute.FileName))
    );
    return TestBuilder.ConsumeRootScope();
  }
}
