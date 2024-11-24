namespace Altaxo.Serialization.AutoUpdates
{
  public class SystemRequirementsHelper
  {
    public static object ServiceLocker { get; } = new object();
  }

  public class SystemRequirements_DotNet9IsNotInstalled : ISystemRequirementsDetermination
  {
    public bool IsNetCoreVersionInstalled(string versionString)
    {
      return false;
    }

    public bool IsNetFrameworkVersionInstalled(string versionString)
    {
      return true;
    }
  }

  public class SystemRequirements_DotNet9IsInstalled : ISystemRequirementsDetermination
  {
    public bool IsNetCoreVersionInstalled(string versionString)
    {
      return true;
    }

    public bool IsNetFrameworkVersionInstalled(string versionString)
    {
      return true;
    }
  }
}
