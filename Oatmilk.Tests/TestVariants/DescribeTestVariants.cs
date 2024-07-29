using FluentAssertions;
using Oatmilk.Internal;

namespace Oatmilk.Tests.TestVariants;

public class DescribeTestVariants
{
  [Describe("A test suite of many of the Describe variants")]
  public void MainDescribeVariants()
  {
    It("should pass", () => Assert.True(true));

    Describe.Each(
      [1, 2, 3],
      "with a nested block at index {0}",
      (i) =>
      {
        It("should pass for index " + i, () => Assert.True(true));
      }
    );

    Describe.Each(
      ["a", "b", "c"],
      s => $"with a nested block for string {s}",
      s =>
      {
        It($"should pass for string {s}", () => Assert.True(true));
      }
    );

    Describe.Skip(
      "a skipped suite",
      () =>
      {
        It("should be skipped", () => Assert.True(false));

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => Assert.True(false));
          }
        );
      }
    );
    Describe.Skip(
      [1, 2, 3],
      "a skipped suite with variants {0}",
      (i) =>
      {
        It("should be skipped", () => Assert.True(false));

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => Assert.True(false));
          }
        );
      }
    );
    Describe.Skip(
      [1, 2, 3],
      x => $"a skipped suite with variants {x}",
      (i) =>
      {
        It("should be skipped", () => Assert.True(false));

        Describe(
          "a nested skipped suite",
          () =>
          {
            It("should be skipped", () => Assert.True(false));
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
        It("should pass", () => Assert.True(true));
      }
    );

    Describe.Only(
      [1, 2, 4],
      "This is an each using the only method {0}",
      (i) =>
      {
        It("should pass", () => Assert.True(true));
      }
    );

    Describe.Only(
      [1, 2, 4],
      x => $"This is an each using the only method {x}",
      (i) =>
      {
        It("should pass", () => Assert.True(true));
      }
    );

    Describe(
      "This one does not have an only, so tests inside should be skipped",
      () =>
      {
        It("should be skipped", () => Assert.True(false));
      }
    );
  }

  [Describe("OatmilkDiscoverer tests enumeration")]
  public void Spec2()
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
              .Count(x => x.TestBlock.ShouldSkipDueToIsSkippedOnThisOrParent(x.TestScope))
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

              var result = await testRunner.RunAsync(false);
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
  }
}
