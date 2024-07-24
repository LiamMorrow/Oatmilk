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

                Describe(
                    "Nested",
                    () =>
                    {
                        It(
                            "Should pass",
                            async () =>
                            {
                                await Task.Delay(5000);
                                Assert.True(true);
                            }
                        );

                        It("Should fail", () => Assert.True(false));
                    }
                );

                It("Should fail", () => Assert.True(false));
            }
        );
}
