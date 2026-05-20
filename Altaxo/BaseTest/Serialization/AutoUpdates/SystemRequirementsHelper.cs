using System;

namespace Altaxo.Serialization.AutoUpdates
{
  public class SystemRequirements_DotNetIsNotInstalled_NetFrameworkIsInstalled : ISystemRequirementsDetermination
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

  public class SystemRequirements_DotNetIsInstalled_NetFrameworkIsInstalled : ISystemRequirementsDetermination
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
