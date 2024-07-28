using Xunit.Abstractions;

namespace Oatmilk.Tests;

public class MultiArgUnitTestsExample
{
  [Oatmilk]
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
          async () =>
          {
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.True(true);
          },
          timeout: TimeSpan.FromSeconds(10)
        );
        It.Each<object>(
          [1, 2, 3, 15, "d", "hello", "world"],
          "Should pass for {0:x}",
          (i, output) =>
          {
            // This will output the value of i to the test output
            // The output is printed to the console when the test fails
            output.OutputSink.WriteLine($"Value: {i}");
          }
        );

        // Each blocks can be configured to be skipped
        It.Skip(
          [1, 2, 3, 15],
          (i) => $"Should skip for {i:x}",
          (i) =>
          {
            Assert.True(false);
          }
        );

        It.Skip(
          "Should skip this test",
          () =>
          {
            Assert.True(false);
          }
        );
      }
    );
}
