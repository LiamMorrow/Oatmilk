namespace Detest.Tests;

using Xunit.Sdk;
using static Detest.TestBuilder;

public class UnitTest2
{
    [Detest()]
    public static void TestScope() =>
        Describe(
            "My tests using the multi arg syntax",
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

                It("Should fail 2", () => Assert.True(false));
            }
        );
}
