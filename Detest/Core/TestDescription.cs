using System.Text;

namespace Detest.Core;

internal record TestScope(string Description, TestScope? Parent)
{
  internal bool HasRunBeforeAlls { get; set; }
  internal bool HasRunAfterAlls { get; set; }
  internal List<TestSetupMethod> TestBeforeAlls { get; } = [];
  internal List<TestSetupMethod> TestBeforeEachs { get; } = [];
  internal List<TestAfterEachMethod> TestAfterEachs { get; } = [];
  internal List<TestSetupMethod> TestAfterAlls { get; } = [];
  internal List<TestBlock> TestMethods { get; } = [];
  internal List<TestScope> Children { get; } = [];

  internal IEnumerable<(TestBlock TestBlock, TestScope TestScope)> EnumerateTests()
  {
    foreach (var test in TestMethods)
    {
      yield return (test, this);
    }

    foreach (var child in Children)
    {
      foreach (var test in child.EnumerateTests())
      {
        yield return test;
      }
    }
  }
}

internal record TestSetupMethod(Func<Task> Body);

internal record TestAfterEachMethod(Func<FinishedTestContext, Task> Body);

internal record TestMetadata(string Description, int ScopeIndex, int LineNumber, string FilePath);

internal record TestBlock(Func<Task> Body, TestMetadata Metadata)
{
  public string GetDescription(TestScope scope)
  {
    var sb = new StringBuilder();
    var parent = scope.Parent;
    while (parent != null)
    {
      sb.Insert(0, parent.Description + " ");
      parent = parent.Parent;
    }
    sb.Append(scope.Description).Append(' ').Append(Metadata.Description);
    return sb.ToString();
  }
}

public record FinishedTestContext(bool Passed, string Description);
