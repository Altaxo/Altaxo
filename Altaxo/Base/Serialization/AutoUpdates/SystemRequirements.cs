using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.AutoUpdates
{
  public static class SystemRequirements
  {
    public const string PropertyKeyNetFrameworkVersion = "RequiredNetFrameworkVersion";

    public static bool MatchesRequirements(PackageInfo packageInfo)
    {
      var properties = packageInfo.Properties;

      string netFrameworkVersion;

      if (properties.ContainsKey(PropertyKeyNetFrameworkVersion))
        netFrameworkVersion = properties[PropertyKeyNetFrameworkVersion];
      else
        netFrameworkVersion = "4.0";

      if (!NetFrameworkVersionDetermination.IsVersionInstalled(netFrameworkVersion))
        return false;

      return true;
    }
  }
}
