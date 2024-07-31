using System.Diagnostics.CodeAnalysis;

namespace Oatmilk.Internal;

internal record OatmilkRunSummary(
  int Total = 0,
  int Passed = 0,
  [property: MemberNotNull(nameof(Exception))] int Failed = 0,
  int Skipped = 0,
  TimeSpan Time = default,
  Exception? Exception = null
);
