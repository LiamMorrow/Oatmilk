using System.Text;

namespace Detestable;

internal record TestScope(TestScope? Parent, TestMetadata Metadata)
{
  internal bool HasRunBeforeAlls { get; set; }
  internal bool HasRunAfterAlls { get; set; }
  internal List<TestSetupMethod> TestBeforeAlls { get; } = [];
  internal List<TestSetupMethod> TestBeforeEachs { get; } = [];
  internal List<TestAfterEachMethod> TestAfterEachs { get; } = [];
  internal List<TestSetupMethod> TestAfterAlls { get; } = [];
  internal List<TestBlock> TestMethods { get; } = [];
  internal List<TestScope> Children { get; } = [];

  internal bool AnyParentsOrThis(Func<TestScope, bool> predicate)
  {
    var current = this;
    while (current != null)
    {
      if (predicate(current))
      {
        return true;
      }
      current = current.Parent;
    }
    return false;
  }

  internal bool AnyChildrenOrThis(Func<TestScope, bool> predicate)
  {
    if (predicate(this))
    {
      return true;
    }

    foreach (var child in Children)
    {
      if (child.AnyChildrenOrThis(predicate))
      {
        return true;
      }
    }

    return false;
  }

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

  internal bool AnyScopesOrTestsAreOnly =>
    AnyChildrenOrThis(sc => sc.Metadata.IsOnly || sc.TestMethods.Any(tm => tm.Metadata.IsOnly));
}

internal record TestSetupMethod(Func<Task> Body);

internal record TestAfterEachMethod(Func<FinishedTestContext, Task> Body);

internal record TestMetadata(
  string Description,
  int ScopeIndex,
  int LineNumber,
  string FilePath,
  bool IsOnly,
  bool IsSkipped
);

internal record TestBlock(Func<Task> Body, TestMetadata Metadata)
{
  public string GetDescription(TestScope scope)
  {
    var sb = new StringBuilder();
    var parent = scope.Parent;
    while (parent != null)
    {
      sb.Insert(0, parent.Metadata.Description + " ");
      parent = parent.Parent;
    }
    sb.Append(scope.Metadata.Description).Append(' ').Append(Metadata.Description);
    return sb.ToString();
  }
}
