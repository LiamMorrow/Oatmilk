namespace Detestable.Tests;

public class MultiArgUnitTestsExample
{
  [Detestable]
  public static void TestScope() =>
    Describe(
      "My tests using the multi arg syntax",
      () =>
      {
        BeforeAll(() => {
          // Runs before all tests in this and nested scopes
        });

        AfterEach(
          (ctx) => {
            // Runs after each of the tests in this and nested scopes
          }
        );

        It(
          "Should pass",
          () =>
          {
            Assert.True(true);
          }
        );
        It.Each(
          ["element", "in", "an", "enumerated", "test"],
          x => $"Should pass for {x}",
          (i) => {
            //
          }
        );
      }
    );
}
