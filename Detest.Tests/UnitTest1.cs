namespace Detest.Tests;

using Xunit.Sdk;
using static Detest.TestBuilder;

public class UnitTest1
{
    [Detest()]
    public static void TestScope() =>
        Describe(
            "My Sick test",
            () =>
            {
                BeforeAll(() =>
                {
                    Console.WriteLine("BeforeAll");
                });

                It(
                    "Should pass",
                    () =>
                    {
                        Assert.True(true);
                    }
                );

                It("Should fail", () => Assert.True(false));
            }
        );
}
