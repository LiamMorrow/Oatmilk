namespace Oatmilk.Internal;

internal static class OatmilkConfiguration
{
  public static TimeSpan DefaultTimeout { get; set; } =
    Environment.GetEnvironmentVariable("OATMILK_DEFAULT_TIMEOUT_SECONDS") is string timeoutString
      ? TimeSpan.FromSeconds(int.Parse(timeoutString))
      : TimeSpan.FromSeconds(5);
}
