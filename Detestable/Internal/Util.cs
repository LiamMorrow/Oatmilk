namespace Detestable.Internal;

internal static class Util
{
  internal static string SafeFormat<T>(string format, T item)
  {
    try
    {
      return string.Format(format, item);
    }
    catch (FormatException)
    {
      return format;
    }
  }
}
