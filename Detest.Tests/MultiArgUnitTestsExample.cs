namespace Detest.Tests;

public class MultiArgUnitTestsExample
{
  [Detest]
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
      }
    );
}
