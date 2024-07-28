namespace Oatmilk;

/// <summary>
/// Represents the context of a test that has finished running.
/// This is passed into the callback to <see cref="TestBuilder.AfterEach(Func{Task})"/>.
/// </summary>
/// <param name="Passed">True if the test passed.</param>
/// <param name="TestOutput">The output of the test.</param>
public record FinishedTestContext(bool Passed, TestOutput TestOutput);

/// <summary>
/// Represents the output of a test.
/// </summary>
/// <param name="Messages">Each message which was output by the test</param>
public record TestOutput(string[] Messages)
{
  /// <summary>
  /// The output of the test, concatenated with newlines.
  /// </summary>
  public string Output => string.Join("\n", Messages);

  /// <summary>
  /// Returns the output of the test.
  /// </summary>
  /// <returns>A string of all the output lines</returns>
  public override string ToString() => Output;
}
