namespace Detest.Core;

/// <summary>
/// Represents the context of a test that has finished running.
/// This is passed into the callback to <see cref="TestBuilder.AfterEach(Func{Task})"/>.
/// </summary>
/// <param name="Passed">True if the test passed.</param>
public record FinishedTestContext(bool Passed);
