using System;

namespace Altaxo.Serialization.AutoUpdates
{
  public class SystemRequirementsHelper
  {
    public static object ServiceLocker { get; } = new object();
  }

  public class SystemRequirements_DotNet9IsNotInstalled : ISystemRequirementsDetermination
  {
    public bool IsNetCoreVersionInstalled(Version version)
    {
      return false;
    }

    public bool IsNetFrameworkVersionInstalled(Version version)
    {
      return true;
    }
  }

  public class SystemRequirements_DotNet9IsInstalled : ISystemRequirementsDetermination
  {
    public bool IsNetCoreVersionInstalled(Version version)
    {
      return true;
    }

    public bool IsNetFrameworkVersionInstalled(Version version)
    {
      return true;
    }
  }
}
