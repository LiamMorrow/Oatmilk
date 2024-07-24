namespace Detest;

public static class TestBuilder
{
  [ThreadStatic]
  private static TestScope? CurrentScope;

  [ThreadStatic]
  private static TestScope? RootScope;

  internal static TestScope ConsumeRootScope()
  {
    if (RootScope == null)
    {
      throw new InvalidOperationException("No root scope. Has Describe been called?");
    }
    var rs = RootScope;
    RootScope = null;
    return rs;
  }

  private static TestScope CurrentScopeNotNull =>
    CurrentScope
    ?? throw new InvalidOperationException("No current scope. Has Describe been called?");

  [Obsolete(
    "Do not call Describe with an async body. Use BeforeAll, BeforeEach to perform asynchronous test setup. Remove async from the method signature to fix this error."
  )]
  public static void Describe(string description, Func<Task> body) =>
    throw new InvalidOperationException(
      @"Must call Describe methods with a synchronous body.
             Use BeforeAll, BeforeEach to perform asynchronous test setup.
             Remove async from the method signature to fix this error."
    );

  public static void Describe(string description, Action body)
  {
    if (RootScope == null)
    {
      CurrentScope = new TestScope(description, null);
      RootScope = CurrentScope;
    }
    else
    {
      var parent = CurrentScope;
      CurrentScope = new TestScope(description, parent);
      parent?.Children.Add(CurrentScope);
    }

    body();
    // Pop back to the parent scope after running all the inner scopes
    CurrentScope = CurrentScope.Parent;
  }

  public static DescribeBlock Describe(string description)
  {
    if (RootScope == null)
    {
      CurrentScope = new TestScope(description, null);
      RootScope = CurrentScope;
    }
    else
    {
      var parent = CurrentScope;
      CurrentScope = new TestScope(description, parent);
      parent?.Children.Add(CurrentScope);
    }

    return new DescribeBlock(description);
  }

  public static void BeforeAll(Func<Task> body) =>
    CurrentScopeNotNull.TestBeforeAlls.Add(new TestSetupMethod(body));

  public static void BeforeAll(Action body) =>
    BeforeAll(() =>
    {
      body();
      return Task.CompletedTask;
    });

  public static void BeforeEach(Func<Task> body) =>
    CurrentScopeNotNull.TestBeforeEachs.Add(new TestSetupMethod(body));

  public static void BeforeEach(Action body) =>
    BeforeEach(() =>
    {
      body();
      return Task.CompletedTask;
    });

  public static void AfterEach(Func<Task> body) => AfterEach(ctx => body());

  public static void AfterEach(Action<FinishedTestContext> body) =>
    AfterEach(ctx =>
    {
      body(ctx);
      return Task.CompletedTask;
    });

  public static void AfterEach(Func<FinishedTestContext, Task> body) =>
    CurrentScopeNotNull.TestAfterEachs.Add(new TestAfterEachMethod(body));

  public static void AfterEach(Action body) => AfterEach(ctx => body());

  public static void AfterAll(Func<Task> body) =>
    CurrentScopeNotNull.TestAfterAlls.Add(new TestSetupMethod(body));

  public static void AfterAll(Action body) =>
    AfterAll(() =>
    {
      body();
      return Task.CompletedTask;
    });

  public static void It(string description, Func<Task> body) => It(description).When(body);

  public static void It(string description, Action body) => It(description).When(body);

  public static ItBlock It(string description) => new(description);

  public record DescribeBlock(string Description)
  {
    public void As(Action body)
    {
      body();
      // Pop back to the parent scope after running all the inner scopes
      CurrentScope = CurrentScopeNotNull.Parent;
    }
  }

  public record ItBlock(string Description)
  {
    public void When(Action body)
    {
      When(() =>
      {
        body();
        return Task.CompletedTask;
      });
    }

    public void When(Func<Task> body)
    {
      var tm = new TestExecutionMethod(Description, body);
      CurrentScopeNotNull.TestMethods.Add(tm);
    }
  }
}
