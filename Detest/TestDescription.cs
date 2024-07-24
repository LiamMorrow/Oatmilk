namespace Detest;

internal record TestScope(string Description, TestScope? Parent)
{
    internal List<TestSetupMethod> TestBeforeAlls { get; } = [];
    internal List<TestSetupMethod> TestBeforeEachs { get; } = [];
    internal List<TestAfterEachMethod> TestAfterEachs { get; } = [];
    internal List<TestSetupMethod> TestAfterAlls { get; } = [];
    internal List<TestExecutionMethod> TestMethods { get; } = [];
    internal List<TestScope> Children { get; } = [];
}

internal record TestSetupMethod(Func<Task> Body);

internal record TestAfterEachMethod(Func<FinishedTestContext, Task> Body);

internal record TestExecutionMethod(string Description, Func<Task> Body);

public record FinishedTestContext(bool Passed, string Description);
