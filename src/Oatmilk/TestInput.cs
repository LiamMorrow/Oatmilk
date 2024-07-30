namespace Oatmilk;

/// <summary>
/// Optionally passed into the body of each test and setup method.
/// Contains the output sink for the test and the cancellation token.
/// </summary>
/// <param name="OutputSink">A sink to write messages into. Can be adapted into a logger to pass into your integration tests.</param>
/// <param name="Token">A token which cancels when the test times out.</param>
public record struct TestInput(ITestOutputSink OutputSink, CancellationToken Token);
