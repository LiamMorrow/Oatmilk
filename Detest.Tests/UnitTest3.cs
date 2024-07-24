namespace Detest.Tests;

using Xunit.Sdk;
using static Detest.TestBuilder;

public class UnitTest3 : DetestTestBase
{
    public override void TestScope() =>
        Describe("My Sick test")
            .As(() =>
            {
                BeforeAll(() =>
                {
                    Console.WriteLine("BeforeAll");
                });

                It("Should pass")
                    .When(() =>
                    {
                        Assert.True(true);
                    });

                Describe("Nested")
                    .As(() =>
                    {
                        It("Should pass")
                            .When(() =>
                            {
                                Assert.True(true);
                            });

                        It("Should fail").When(() => Assert.True(false));
                    });

                It("Should fail").When(() => Assert.True(false));
            });
}
