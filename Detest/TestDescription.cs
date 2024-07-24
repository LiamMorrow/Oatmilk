namespace Detest;

internal record TestScope(string Description, TestScope? Parent)
{
    internal List<TestSetupMethod> TestBeforeAlls { get; } = [];
    internal List<TestSetupMethod> TestBeforeEachs { get; } = [];
    internal List<TestSetupMethod> TestAfterEachs { get; } = [];
    internal List<TestSetupMethod> TestAfterAlls { get; } = [];
    internal List<TestExecutionMethod> TestMethods { get; } = [];
    internal List<TestScope> Children { get; } = [];
}

internal record TestSetupMethod(Func<Task> Body);

internal record TestExecutionMethod(string Description, Func<Task> Body);
