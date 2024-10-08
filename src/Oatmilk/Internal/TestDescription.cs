using System.Text;

namespace Oatmilk;

internal record TestScope(TestScope? Parent, TestMetadata Metadata)
{
  internal bool HasRunBeforeAlls { get; set; }
  internal bool HasRunAfterAlls { get; set; }
  internal List<TestSetupMethod> TestBeforeAlls { get; } = [];
  internal List<TestSetupMethod> TestBeforeEachs { get; } = [];
  internal List<TestAfterEachMethod> TestAfterEachs { get; } = [];
  internal List<TestTeardownMethod> TestAfterAlls { get; } = [];
  internal List<TestBlock> TestBlocks { get; } = [];
  internal List<TestScope> Children { get; } = [];

  internal TestScope RootScope => Parent == null ? this : Parent.RootScope;

  internal bool HasRunAllTests =>
    TestBlocks.All(x => x.HasRun) && Children.All(x => x.HasRunAllTests);

  internal string ScopeIndexPath =>
    Parent == null
      ? Metadata.ScopeIndex.ToString()
      : $"{Parent.ScopeIndexPath}.{Metadata.ScopeIndex}";

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

  internal IEnumerable<(TestScope TestScope, TestBlock TestBlock)> EnumerateTests()
  {
    foreach (var test in TestBlocks)
    {
      yield return (this, test);
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
    AnyChildrenOrThis(sc => sc.Metadata.IsOnly || sc.TestBlocks.Any(tm => tm.Metadata.IsOnly));
}

internal record TestSetupMethod(Func<TestInput, Task> Body);

internal record TestTeardownMethod(Func<Task> Body);

internal record TestAfterEachMethod(Func<FinishedTestContext, Task> Body);

internal record TestMetadata(
  string Description,
  int ScopeIndex,
  int LineNumber,
  string FilePath,
  bool IsOnly,
  bool IsSkipped,
  TimeSpan Timeout
);

internal record TestBlock(Func<TestInput, Task> Body, TestMetadata Metadata)
{
  internal bool HasRun { get; set; }

  internal SkipReason GetSkipReason(TestScope scope) =>
    Metadata.IsSkipped || scope.AnyParentsOrThis(x => x.Metadata.IsSkipped)
      ? SkipReason.SkippedBySkipMethod
      : scope.AnyParentsOrThis(x => x.AnyScopesOrTestsAreOnly)
      && !Metadata.IsOnly
      && !scope.AnyParentsOrThis(x => x.Metadata.IsOnly)
        ? SkipReason.OnlyTestsInScopeAndThisIsNotOnly
        : SkipReason.DoNotSkip;

  internal string GetDescription(TestScope scope)
  {
    var sb = new StringBuilder();
    var parent = scope.Parent;
    while (parent != null)
    {
      sb.Insert(0, parent.Metadata.Description + ".");
      parent = parent.Parent;
    }
    sb.Append(scope.Metadata.Description).Append('.').Append(Metadata.Description);
    return sb.ToString();
  }
}

internal enum SkipReason
{
  DoNotSkip,
  SkippedBySkipMethod,
  OnlyTestsInScopeAndThisIsNotOnly
}
