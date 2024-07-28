namespace Oatmilk;

/// <summary>
/// Provides a mechanism for writing output from tests.
/// This output can be accessed in the AfterEach hook.
/// </summary>
public interface ITestOutputSink
{
  /// <summary>
  /// Writes a message to the output.
  /// </summary>
  /// <param name="message">The text to output</param>
  void WriteLine(string message);

  /// <summary>
  /// Writes a formatted message to the output.
  /// </summary>
  /// <param name="format">A composite format string</param>
  /// <param name="args">An object array that contains zero or more objects to format</param>
  void WriteLine(string format, params object[] args);
}

internal class TestOutputSink : ITestOutputSink
{
  private readonly List<string> _messages = [];

  public void WriteLine(string message) => _messages.Add(message);

  public void WriteLine(string format, params object[] args) =>
    _messages.Add(string.Format(format, args));

  public TestOutput GetOutput() => new([.. _messages]);
}
