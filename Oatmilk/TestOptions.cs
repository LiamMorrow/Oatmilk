namespace Oatmilk;

/// <summary>
/// Options for running a test or suite of tests.  The most specific option from each level will be used.
/// e.g. An It.Options will override a Describe.Options which will override the default TestOptions.
/// </summary>
/// <param name="Timeout">The timeout for the test or each test in the suite of tests. If unset, uses <see cref="TestBuilder.DefaultTimeout"/> </param>
public record struct TestOptions(TimeSpan? Timeout);
