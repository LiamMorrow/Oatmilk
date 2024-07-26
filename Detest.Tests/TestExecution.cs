using FluentAssertions;

namespace Detest.Tests;

public class TestExecution
{
  [Describe("TestExecution")]
  public void Spec()
  {
    var beforeAllRunCount = 0;
    var lastLifecycleMethod = "UNSET";

    Action expectLastLifecycleToBe(string expectedLastLifecycleMethod) =>
      () =>
      {
        lastLifecycleMethod.Should().Be(expectedLastLifecycleMethod);
        beforeAllRunCount.Should().Be(1);
      };

    BeforeAll(() =>
    {
      lastLifecycleMethod = "BeforeAll";
      beforeAllRunCount++;
    });

    BeforeEach(() =>
    {
      lastLifecycleMethod = "BeforeEach";
    });

    AfterEach(
      (ctx) =>
      {
        lastLifecycleMethod = "AfterEach";
      }
    );

    It("Should have run beforeAll once", () => expectLastLifecycleToBe("BeforeEach"));

    It("Should have run beforeEach after beforeAll", expectLastLifecycleToBe("BeforeEach"));

    Describe(
      "when nested",
      () =>
      {
        BeforeEach(() =>
        {
          lastLifecycleMethod = "NestedBeforeEach";
        });

        It(
          "Should have run beforeEach before nested beforeEach",
          expectLastLifecycleToBe("NestedBeforeEach")
        );

        Describe(
          "and nested again but this time with no tests",
          () =>
          {
            BeforeEach(() =>
            {
              lastLifecycleMethod = "NestedNestedBeforeEach";
            });
            Describe(
              "and nested again",
              () =>
                It(
                  "Should have run beforeEach before nested beforeEach",
                  expectLastLifecycleToBe("NestedNestedBeforeEach")
                )
            );
          }
        );
      }
    );

    Describe(
      "when nested again",
      () =>
        It(
          "Should have run beforeEach before nested beforeEach - ignoring the other nests",
          expectLastLifecycleToBe("BeforeEach")
        )
    );
  }
}
