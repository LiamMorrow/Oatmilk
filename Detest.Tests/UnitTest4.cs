namespace Detest.Tests;

using Xunit.Sdk;
using static Detest.TestBuilder;

public class UnitTest4
{
  [Describe("My tests with the DescribeAttribute")]
  public void Spec()
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
  }
}
