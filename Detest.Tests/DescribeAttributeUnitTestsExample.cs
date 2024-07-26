namespace Detest.Tests;

public class DescribeAttributeUnitTestsExample
{
  [Describe("My tests with the DescribeAttribute")]
  public void Spec()
  {
    BeforeAll(() => {
      // Runs before all tests in this and nested scopes
    });

    AfterEach(
      (ctx) => {
        // Runs after each of the tests in this and nested scopes
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
  }
}
