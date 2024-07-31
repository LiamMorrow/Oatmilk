namespace Oatmilk.Tests;

#if MSTEST
[TestClass]
#endif
public class TestBuilderTests
{
  [Describe("TestBuilder")]
  public void Spec()
  {
    static void emptyItBody() { }

    Describe(
      "When there are no tests",
      () =>
      {
        It(
          "Should throw when trying to consume the root scope",
          () =>
          {
            Action act = () => TestBuilder.ConsumeRootScope();
            act.Should().Throw<InvalidOperationException>();
          }
        );
        It.Each(
          [1, 3, 5],
          "{0} is odd", // Can either use a format string, or a callback like val => $"{val} is odd"
          (value) =>
          {
            (value % 2).Should().Be(1);
          }
        );
      }
    );

    Describe(
      "When there are tests",
      () =>
      {
        TestBuilder.DescribeBlock rootBlock = null!;

        BeforeEach(() =>
        {
          try
          {
            TestBuilder.ConsumeRootScope();
          }
          catch { }
          rootBlock = TestBuilder.Describe("Root");
        });

        Describe(
          "with no nesting",
          () =>
          {
            BeforeEach(() =>
            {
              rootBlock.As(() =>
              {
                TestBuilder.It("top level it", emptyItBody);
              });
            });

            It(
              "should add a test to the root scope",
              () =>
              {
                var rootScope = TestBuilder.ConsumeRootScope();
                rootScope.TestBlocks.Should().HaveCount(1);
                rootScope.TestBlocks.Single().Metadata.Description.Should().Be("top level it");
              }
            );
          }
        );

        Describe(
          "with a single nest",
          () =>
          {
            BeforeEach(() =>
            {
              rootBlock.As(() =>
              {
                TestBuilder
                  .Describe("Nested")
                  .As(() =>
                  {
                    TestBuilder.BeforeEach(() => { });
                    TestBuilder.It("Nested It", emptyItBody);
                  });
              });
            });

            It(
              "should add a test to the nested scope",
              () =>
              {
                var rootScope = TestBuilder.ConsumeRootScope();
                rootScope.TestBlocks.Should().HaveCount(0);
                rootScope.Children.Should().HaveCount(1);
                var nestedScope = rootScope.Children.Single();
                nestedScope.TestBlocks.Should().HaveCount(1);
                nestedScope.TestBlocks.Single().Metadata.Description.Should().Be("Nested It");
              }
            );

            It(
              "should add a beforeEach to the nested scope",
              () =>
              {
                var rootScope = TestBuilder.ConsumeRootScope();
                var nestedScope = rootScope.Children.Single();
                nestedScope.TestBeforeEachs.Should().HaveCount(1);
              }
            );
          }
        );

        Describe(
          "with a double nest",
          () =>
          {
            BeforeEach(() =>
            {
              rootBlock.As(() =>
              {
                TestBuilder
                  .Describe("Nested")
                  .As(() =>
                  {
                    TestBuilder.BeforeEach(() => { });
                    TestBuilder.It("Nested It", emptyItBody);
                    TestBuilder
                      .Describe("Double Nested")
                      .As(() =>
                      {
                        TestBuilder.BeforeEach(() => { });
                        TestBuilder.It("Double Nested It", emptyItBody);
                      });
                  });
              });
            });

            It(
              "should add a test to the double nested scope",
              () =>
              {
                var rootScope = TestBuilder.ConsumeRootScope();
                var nestedScope = rootScope.Children.Single();
                nestedScope.TestBlocks.Should().HaveCount(1);
                nestedScope.TestBlocks.Single().Metadata.Description.Should().Be("Nested It");
                nestedScope.Children.Should().HaveCount(1);
                var doubleNestedScope = nestedScope.Children.Single();
                doubleNestedScope.TestBlocks.Should().HaveCount(1);
                doubleNestedScope
                  .TestBlocks.Single()
                  .Metadata.Description.Should()
                  .Be("Double Nested It");
              }
            );

            It(
              "should add a beforeEach to the double nested scope",
              () =>
              {
                var rootScope = TestBuilder.ConsumeRootScope();
                var nestedScope = rootScope.Children.Single();
                var doubleNestedScope = nestedScope.Children.Single();
                doubleNestedScope.TestBeforeEachs.Should().HaveCount(1);
              }
            );
          }
        );
      }
    );
  }
}
