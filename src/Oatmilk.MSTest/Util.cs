using Microsoft.VisualStudio.TestTools.UnitTesting;
using Oatmilk.Internal;

namespace Oatmilk.MSTest;

internal static class Util
{
  internal static TestResult GetTestResult(
    OatmilkRunSummary runSummary,
    TestScope testScope,
    TestBlock testBlock
  )
  {
    var result = new TestResult
    {
      DisplayName = testBlock.GetDescription(testScope),
      Outcome = runSummary switch
      {
        { Skipped: 1 } => UnitTestOutcome.Inconclusive,
        { Failed: 1 } => UnitTestOutcome.Failed,
        _ => UnitTestOutcome.Passed,
      },
      Duration = runSummary.Time,
      TestFailureException = runSummary.Exception,
    };

    return result;
  }
}
