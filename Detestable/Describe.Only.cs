using System.Runtime.CompilerServices;

namespace Detestable;

public static partial class Describe
{
  public static void Only(
    string description,
    Func<Task> body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description, lineNumber, filePath).When(body);

  public static void Only(
    string description,
    Action body,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) => Only(description, lineNumber, filePath).When(body);

  public static TestBuilder.ItBlock Only(
    string description,
    [CallerLineNumber] int lineNumber = 0,
    [CallerFilePath] string filePath = ""
  ) =>
    new(
      Description: description,
      IsOnly: true,
      IsSkipped: false,
      LineNumber: lineNumber,
      FilePath: filePath
    );
}
