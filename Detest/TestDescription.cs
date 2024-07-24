namespace Detest;

public record TestScope(string Description, TestScope? Parent)
{
    public List<TestSetupMethod> TestBeforeAlls { get; } = [];
    public List<TestSetupMethod> TestBeforeEachs { get; } = [];
    public List<TestSetupMethod> TestAfterEachs { get; } = [];
    public List<TestSetupMethod> TestAfterAlls { get; } = [];
    public List<TestExecutionMethod> TestMethods { get; } = [];
    public List<TestScope> Children { get; } = [];
}

public record TestSetupMethod(Func<Task> Body);

public record TestExecutionMethod(string Description, Func<Task> Body);
