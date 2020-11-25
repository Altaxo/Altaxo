#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altaxo.Main.Services;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Represents the system requirements needed for a given package.
  /// </summary>
  public static class SystemRequirements
  {
    /// <summary>
    /// The property key: .NET framework version
    /// </summary>
    public const string PropertyKeyNetFrameworkVersion = "RequiredNetFrameworkVersion";

    /// <summary>
    /// Determines if this system is matching the requirements of the package.
    /// </summary>
    /// <param name="packageInfo">The information about the package.</param>
    /// <returns>True if the system is matching the requirements of the package; otherwise, false.</returns>
    public static bool MatchesRequirements(PackageInfo packageInfo)
    {
      var properties = packageInfo.Properties;

      string netFrameworkVersion;

      if (properties.ContainsKey(PropertyKeyNetFrameworkVersion))
        netFrameworkVersion = properties[PropertyKeyNetFrameworkVersion];
      else
        netFrameworkVersion = "4.0";


      if (!NetFrameworkVersionDetermination.IsVersionInstalledHere(netFrameworkVersion))
        return false;

      return true;
    }
  }
}
