namespace Detest.Tests;

using static Detest.TestBuilder;

public class MultiArgUnitTestsExample
{
  [Detest]
  public static void TestScope() =>
    Describe(
      "My tests using the multi arg syntax",
      () =>
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

        It(
          "Should pass",
          () =>
          {
            Assert.True(true);
          }
        );
      }
    );
}
