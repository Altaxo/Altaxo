#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2024 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Runtime.InteropServices;

namespace Altaxo.Serialization.AutoUpdates
{

  public class SystemRequirements : SystemRequirementsBase
  {
    /// <summary>
    /// Determines if this system is matching the requirements of the package.
    /// </summary>
    /// <param name="packageInfo">The information about the package.</param>
    /// <returns>True if the system is matching the requirements of the package; otherwise, false.</returns>
    public static bool MatchesRequirements(PackageInfo packageInfo)
    {
      var properties = packageInfo.Properties;
      var systemRequirementsService = Current.GetRequiredService<ISystemRequirementsDetermination>();


      if (properties.TryGetValue(PropertyKeyNetFrameworkVersion, out var netFrameworkVersion))
      {
        if (!systemRequirementsService.IsNetFrameworkVersionInstalled(netFrameworkVersion))
          return false;
      }

      if (properties.TryGetValue(PropertyKeyDotNetVersion, out var dotnetVersion))
      {
        if (!systemRequirementsService.IsNetCoreVersionInstalled(dotnetVersion))
          return false;
      }

      if (properties.TryGetValue(PropertyKeyArchitecture, out var requiredArchitecture))
      {
        if (!MeetsArchitectureRequirements(requiredArchitecture))
          return false;
      }

      if (properties.TryGetValue(PropertyKeyOperatingSystem, out var operatingSystem))
      {
        if (!MeetsOSRequirements(operatingSystem))
          return false;
      }

      return true;
    }
  }
}
