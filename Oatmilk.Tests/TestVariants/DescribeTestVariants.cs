namespace Oatmilk.Tests.TestVariants;

public class DescribeTestVariants
{
  [Describe("A test suite of many of the Describe variants")]
  public void EachSpec()
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
  public void OnlySpec()
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

  [Oatmilk]
  public void Spec2()
  {
    Describe(
      "A test suite with the oatmilk annotation",
      () =>
      {
        It("should pass", () => Assert.True(true));
      }
    );
  }
}
