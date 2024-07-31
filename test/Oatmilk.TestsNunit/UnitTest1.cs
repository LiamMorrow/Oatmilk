using NUnit.Framework;
using static Oatmilk.TestBuilder;

namespace Oatmilk.TestsNunit;

public class Tests : OatmilkTestBase
{
  [Describe("A test suite of many of the It variants")]
  public void Test1()
  {
    It("should pass", () => Assert.That(false));
  }
}
