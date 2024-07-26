namespace Detestable.Internal;

internal record DetestableRunSummary(
  int Total = 0,
  int Passed = 0,
  int Failed = 0,
  int Skipped = 0,
  TimeSpan Time = default
)
{
  public DetestableRunSummary Aggregate(DetestableRunSummary runSummary)
  {
    return this with
    {
      Total = Total + runSummary.Total,
      Passed = Passed + runSummary.Passed,
      Failed = Failed + runSummary.Failed,
      Skipped = Skipped + runSummary.Skipped,
      Time = Time + runSummary.Time
    };
  }
}
