namespace Detest.Tests;

using Xunit.Sdk;
using static Detest.TestBuilder;

public class FluentUnitTestsExample
{
  [Detest]
  public static void TestScope() =>
    Describe("My tests using the fluent syntax")
      .As(() =>
      {
        BeforeAll(() =>
        {
          Console.WriteLine("BeforeAll");
        });

        AfterEach(
          (ctx) =>
          {
            Console.WriteLine("After " + ctx.Description);
          }
        );

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
          });
      });
}
