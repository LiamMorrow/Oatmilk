namespace Oatmilk.Tests;

#if MSTEST
[TestClass]
#endif
public class FluentUnitTestsExample
{
  [Oatmilk]
  public void TestScope()
  {
    Describe("My tests using the fluent syntax")
      .As(() =>
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
            true.Should().BeTrue();
          });

        Describe("Nested")
          .As(() =>
          {
            It("Should pass")
              .When(() =>
              {
                true.Should().BeTrue();
              });
          });
      });
  }
}
