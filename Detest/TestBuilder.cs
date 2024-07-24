using System;
using System.Threading.Tasks;

namespace Detest
{
    public static class TestBuilder
    {
        [ThreadStatic]
        private static TestScope CurrentScope;

        [ThreadStatic]
        private static TestScope RootScope;

        public static TestScope GetRootScope()
        {
            if (RootScope == null)
            {
                throw new InvalidOperationException("No root scope. Has Describe been called?");
            }
            var rs = RootScope;
            RootScope = null;
            return rs;
        }

        public static void ClearScope() => CurrentScope = null;

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

        public static void BeforeAll(Func<Task> body) =>
            CurrentScope.TestBeforeAlls.Add(new TestSetupMethod(body));

        public static void BeforeAll(Action body) =>
            BeforeAll(() =>
            {
                body();
                return Task.CompletedTask;
            });

        public static void BeforeEach(Func<Task> body) =>
            CurrentScope.TestBeforeEachs.Add(new TestSetupMethod(body));

        public static void BeforeEach(Action body) =>
            BeforeEach(() =>
            {
                body();
                return Task.CompletedTask;
            });

        public static void AfterEach(Func<Task> body) =>
            CurrentScope.TestAfterEachs.Add(new TestSetupMethod(body));

        public static void AfterEach(Action body) =>
            AfterEach(() =>
            {
                body();
                return Task.CompletedTask;
            });

        public static void AfterAll(Func<Task> body) =>
            CurrentScope.TestAfterAlls.Add(new TestSetupMethod(body));

        public static void AfterAll(Action body) =>
            AfterAll(() =>
            {
                body();
                return Task.CompletedTask;
            });

        public static void It(string description, Func<Task> body) =>
            CurrentScope.TestMethods.Add(new TestExecutionMethod(description, body));

        public static void It(string description, Action body) =>
            It(
                description,
                () =>
                {
                    body();
                    return Task.CompletedTask;
                }
            );
    }
}
