using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace Oatmilk.Nunit;

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
public sealed class DescribeAttribute(
  string Description,
  [CallerFilePath] string FileName = "",
  [CallerLineNumber] int LineNumber = 0
) : TheoryAttribute, ITestBuilder
{
  /// <summary>
  /// The description of the test suite.
  /// This is passed to the <see cref="TestBuilder.Describe(string, Action, TestOptions, int,string)"/> method.
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

  IEnumerable<TestMethod> ITestBuilder.BuildFrom(IMethodInfo method, Test? suite)
  {
    Console.WriteLine("Building tests from method");
    var instance = Activator.CreateInstance(method.TypeInfo.Type);
    TestBuilder.Describe(
      Description,
      () =>
      {
        method.Invoke(instance, null);
      }
    );
    var rootScope = TestBuilder.ConsumeRootScope();
    foreach (var test in rootScope.EnumerateTests())
    {
      yield return new OatmilkNunitTestBlockTest(test.TestScope, test.TestBlock);
    }
  }
}
