namespace Detest;

public abstract class DetestTestBase
{
  protected abstract string Description { get; }

  [Detest]
  public void TestScope() => TestBuilder.Describe(Description, Described);

  protected abstract void Described();
}
