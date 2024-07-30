using System.ComponentModel;
using FluentAssertions;
using Oatmilk.Internal;

namespace Oatmilk.Tests.Internal;

public class TestRunnerTests
{
  [Describe("OatmilkTestRunner")]
  public void TestRunnerTestsSpec()
  {
    TestScope rootScope = null!;
    List<(TestScope TestScope, TestBlock TestBlock)> discoveredTests = null!;

    void Setup(Action testMethod)
    {
      TestBuilder.Describe("A root node", testMethod);
      rootScope = TestBuilder.ConsumeRootScope();
      discoveredTests = OatmilkDiscoverer
        .TraverseScopesAndYieldTestBlocks(rootScope, false)
        .ToList();
    }

    Describe(
      "For some tests with After and before methods",
      () =>
      {
        List<string> lifetimeMethods = [];

        BeforeAll(() =>
        {
          lifetimeMethods = [];
          Setup(() =>
          {
            AfterAll(() => lifetimeMethods.Add("AfterAll"));
            AfterEach((ctx) => lifetimeMethods.Add("AfterEach " + ctx.TestName));
            BeforeAll(() => lifetimeMethods.Add("BeforeAll"));
            BeforeEach(() => lifetimeMethods.Add("BeforeEach"));

            It("should pass", () => lifetimeMethods.Add("should pass"));
            It("should also pass", () => lifetimeMethods.Add("should also pass"));
            It(
              "should fail",
              () =>
              {
                lifetimeMethods.Add("should fail");
                true.Should().BeFalse();
              }
            );

            Describe(
              "A nested block",
              () =>
              {
                It("nested should pass", () => lifetimeMethods.Add("nested should pass"));
              }
            );
          });
        });

        It(
          "should run the lifecycle methods in the correct order",
          async () =>
          {
            var messageBus = new DummyMessageBus();
            foreach (var test in discoveredTests)
            {
              var testRunner = new OatmilkTestBlockRunner(
                test.TestScope,
                test.TestBlock,
                messageBus
              );

              var result = await testRunner.RunAsync(false);
            }

            lifetimeMethods
              .Should()
              .Equal(
                [
                  "BeforeAll",
                  "BeforeEach",
                  "should pass",
                  "AfterEach should pass",
                  "BeforeEach",
                  "should also pass",
                  "AfterEach should also pass",
                  "BeforeEach",
                  "should fail",
                  "AfterEach should fail",
                  "BeforeEach",
                  "nested should pass",
                  "AfterEach nested should pass",
                  "AfterAll",
                ]
              );
          }
        );
      }
    );
  }
}
