using FluentAssertions;
using Oatmilk.Internal;

namespace Oatmilk.Tests.TestVariants;

public class ItTestVariants
{
  [Describe("A test suite of many of the It variants")]
  public void MainDescribeVariants()
  {
    It("should pass", () => true.Should().BeTrue());

    It.Each([1, 2, 3], "with an each at index {0}", (i) => true.Should().BeTrue());

    It.Each(
      ["a", "b", "c"],
      s => $"with an each block for string {s}",
      s => true.Should().BeTrue()
    );

    It.Skip("a skipped test should skip", () => true.Should().BeFalse());

    It.Skip(
      [1, 2, 3],
      "a skipped test with variants and string format name {0}",
      (i) => true.Should().BeFalse()
    );

    It.Skip(
      [1, 2, 3],
      x => $"a skipped suite with variants and functioned name {x}",
      (i) => true.Should().BeFalse()
    );
  }

  [Describe("A test suite containing It.Only calls")]
  public void DescribeOnlyVariants()
  {
    // Note that any it with an only will be run, not just the first
    It.Only("Here we have a top level only", () => true.Should().BeTrue());

    It.Only([1, 2, 4], "This is an each using the only method {0}", (i) => true.Should().BeTrue());

    It.Only(
      [1, 2, 4],
      x => $"This is an each using the only method {x}",
      (i) => true.Should().BeTrue()
    );

    It("This one does not have an only, so should be skipped", () => true.Should().BeFalse());
  }

  [Describe("OatmilkDiscoverer tests enumeration of Its")]
  public void TestEnumerationAndRunning()
  {
    TestScope rootScope = null!;
    List<(TestScope TestScope, TestBlock TestBlock)> discoveredTests = null!;

    void Setup(Action testMethod)
    {
      TestBuilder.Describe("A root node", testMethod);
      rootScope = TestBuilder.ConsumeRootScope();
      discoveredTests = rootScope.EnumerateTests().ToList();
    }

    Describe(
      "For the main test scopes",
      () =>
      {
        BeforeAll(() =>
        {
          Setup(MainDescribeVariants);
        });

        It(
          "should have the correct number of tests",
          () =>
          {
            discoveredTests.Count.Should().Be(14);
          }
        );

        It(
          "should have the correct number of skipped tests",
          () =>
          {
            discoveredTests
              .Count(x => x.TestBlock.ShouldSkipDueToIsSkippedOnThisOrParent(x.TestScope))
              .Should()
              .Be(7);
          }
        );

        It(
          "should pass or skip tests",
          async (input) =>
          {
            var messageBus = new DummyMessageBus();
            foreach (var test in discoveredTests)
            {
              var testRunner = new OatmilkTestBlockRunner(
                test.TestScope,
                test.TestBlock,
                messageBus
              );

              var result = await testRunner.RunAsync();
              if (test.TestBlock.ShouldSkipDueToIsSkippedOnThisOrParent(test.TestScope))
              {
                result.Skipped.Should().Be(1);
              }
              else
              {
                result.Passed.Should().Be(1);
              }

              result.Failed.Should().Be(0);
              result.Total.Should().Be(1);
            }
          }
        );
      }
    );
    Describe(
      "For the 'Only' test scopes",
      () =>
      {
        BeforeAll(() =>
        {
          Setup(DescribeOnlyVariants);
        });

        It(
          "should have the correct number of tests",
          () =>
          {
            discoveredTests.Count.Should().Be(8);
          }
        );

        It(
          "should have no directly skipped tests",
          () =>
          {
            discoveredTests
              .Count(x => x.TestBlock.ShouldSkipDueToIsSkippedOnThisOrParent(x.TestScope))
              .Should()
              .Be(0);

            // No tests were directly skipped
            discoveredTests.Select(x => x.TestBlock.Metadata.IsSkipped).Should().NotContain(true);
          }
        );
      }
    );

    Describe(
      "For a describe with a timeout",
      () =>
      {
        BeforeAll(() =>
        {
          Setup(() =>
          {
            Describe(
              "A test suite with a timeout",
              () =>
              {
                It(
                  "should fail because the timeout is too short",
                  () => Task.Delay(TimeSpan.FromMilliseconds(200))
                );

                It("should pass", () => true.Should().BeTrue());
              },
              new(Timeout: TimeSpan.FromMilliseconds(100))
            );
          });
        });

        It(
          "should pass the test which runs within the timeout",
          () =>
          {
            var test = discoveredTests.Single(x =>
              x.TestBlock.Metadata.Description == "should pass"
            );
            var testRunner = new OatmilkTestBlockRunner(
              test.TestScope,
              test.TestBlock,
              new DummyMessageBus()
            );

            var result = testRunner.RunAsync().Result;
            result.Passed.Should().Be(1);
            result.Failed.Should().Be(0);
            result.Skipped.Should().Be(0);
          }
        );

        It(
          "should fail the test which runs outside the timeout",
          () =>
          {
            var test = discoveredTests.Single(x =>
              x.TestBlock.Metadata.Description == "should fail because the timeout is too short"
            );
            var testRunner = new OatmilkTestBlockRunner(
              test.TestScope,
              test.TestBlock,
              new DummyMessageBus()
            );

            var result = testRunner.RunAsync().Result;
            result.Passed.Should().Be(0);
            result.Failed.Should().Be(1);
            result.Skipped.Should().Be(0);
          }
        );
      }
    );
  }
}
