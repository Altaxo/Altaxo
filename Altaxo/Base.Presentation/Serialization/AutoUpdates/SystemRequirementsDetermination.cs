#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2020 Dr. Dirk Lellinger
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

namespace Altaxo.Serialization.AutoUpdates
{
#if AutoUpdateDownloader
  /// <summary>
  /// Checker for the system requirements.
  /// </summary>
  public class SystemRequirementsDetermination
#else
  /// <summary>
  /// Implementation of the <see cref="ISystemRequirementsDetermination"/> interface.
  /// </summary>
  /// <seealso cref="Altaxo.Serialization.AutoUpdates.ISystemRequirementsDetermination" />
  public class SystemRequirementsDetermination : ISystemRequirementsDetermination
#endif
  {
    /// <summary>
    /// Determines whether a specified version of the .NET framework is installed.
    /// </summary>
    /// <param name="versionString">The version. Examples are 4.0, 4.6, 4.7.1 etc.</param>
    /// <returns>
    ///   <c>true</c> if the specified version of the .NET framework is installed; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNetFrameworkVersionInstalled(string versionString)
    {
      return NetFrameworkVersionDetermination.IsVersionInstalled(versionString);
    }

    /// <inheritdoc/>
    public bool IsNetCoreVersionInstalled(string versionString)
    {
      return NetCoreVersionDetermination.IsVersionInstalled(versionString);
    }
  }
}
