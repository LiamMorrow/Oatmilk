using FluentAssertions;
using Oatmilk.Internal;

namespace Oatmilk.Tests.TestVariants;

public class DescribeTestVariants
{
  [Describe("A test suite of many of the Describe variants")]
  public void MainDescribeVariants()
  {
    It("should pass", () => true.Should().BeTrue());

    Describe.Each(
      [1, 2, 3],
      "with a nested block at index {0}",
      (i) =>
      {
        It("should pass for index " + i, () => true.Should().BeTrue());
      }
    );

    Describe.Each(
      ["a", "b", "c"],
      s => $"with a nested block for string {s}",
      s =>
      {
        It($"should pass for string {s}", () => true.Should().BeTrue());
      }
    );

    Describe.Skip(
      "a skipped suite",
      () =>
      {
        It("should be skipped", () => true.Should().BeFalse());

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => true.Should().BeFalse());
          }
        );
      }
    );
    Describe.Skip(
      [1, 2, 3],
      "a skipped suite with variants {0}",
      (i) =>
      {
        It("should be skipped", () => true.Should().BeFalse());

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => true.Should().BeFalse());
          }
        );
      }
    );
    Describe.Skip(
      [1, 2, 3],
      x => $"a skipped suite with variants {x}",
      (i) =>
      {
        It("should be skipped", () => true.Should().BeFalse());

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => true.Should().BeFalse());
          }
        );
      }
    );

    It(
      "should throw an error when trying to use the async variants of Describe",
      () =>
      {
#pragma warning disable CS0618
        Assert.Throws<InvalidOperationException>(
          () => Describe("A test suite", async () => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Each([1], "A test suite", async (a) => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Each([1], a => "A test suite" + a, async (a) => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Skip("A test suite", async () => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Skip([1], "A test suite", async (a) => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Skip([1], x => $"A test suite {x}", async (a) => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Only("A test suite", async () => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Only([1], "A test suite", async (a) => await Task.CompletedTask)
        );
        Assert.Throws<InvalidOperationException>(
          () => Describe.Only([1], x => $"A test suite {x}", async (a) => await Task.CompletedTask)
        );
#pragma warning restore CS0618
      }
    );
  }

  [Describe("A test suite containing Describe.Only calls")]
  public void DescribeOnlyVariants()
  {
    // Note that any describe with an only will be run, not just the first
    Describe.Only(
      "Here we have a top level only",
      () =>
      {
        It("should pass", () => true.Should().BeTrue());
      }
    );

    Describe.Only(
      [1, 2, 4],
      "This is an each using the only method {0}",
      (i) =>
      {
        It("should pass", () => true.Should().BeTrue());
      }
    );

    Describe.Only(
      [1, 2, 4],
      x => $"This is an each using the only method {x}",
      (i) =>
      {
        It("should pass", () => true.Should().BeTrue());
      }
    );

    Describe(
      "This one does not have an only, so tests inside should be skipped",
      () =>
      {
        It("should be skipped", () => true.Should().BeFalse());
      }
    );
  }

  [Describe("OatmilkDiscoverer tests enumeration")]
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
            discoveredTests.Count.Should().Be(22);
          }
        );

        It(
          "should have the correct number of skipped tests",
          () =>
          {
            discoveredTests
              .Count(x => x.TestBlock.GetSkipReason(x.TestScope) != SkipReason.DoNotSkip)
              .Should()
              .Be(14);

            // No tests were directly skipped
            discoveredTests.Select(x => x.TestBlock.Metadata.IsSkipped).Should().NotContain(true);
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
              if (test.TestBlock.GetSkipReason(test.TestScope) != SkipReason.DoNotSkip)
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
              .Count(x => x.TestBlock.GetSkipReason(x.TestScope) == SkipReason.SkippedBySkipMethod)
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
                  () => Task.Delay(TimeSpan.FromMilliseconds(100))
                );

                It("should pass", () => Task.Delay(TimeSpan.FromMilliseconds(5)));
              },
              new(Timeout: TimeSpan.FromMilliseconds(10))
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
