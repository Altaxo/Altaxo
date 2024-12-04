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

#nullable enable
using System;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// Interface to a class that is able to determine whether a given version of .NET framework and .NET core is installed.
  /// </summary>
  public interface ISystemRequirementsDetermination
  {
    /// <summary>
    /// Determines whether a specified version of the net framework is installed.
    /// </summary>
    /// <param name="version">The version. Examples are 4.0, 4.6, 4.7.1 etc.</param>
    /// <returns>
    ///   <c>true</c> if the specified version of the .NET framework is installed; otherwise, <c>false</c>.
    /// </returns>
    bool IsNetFrameworkVersionInstalled(Version version);

    /// <summary>
    /// Determines whether the specified DotNet core version is installed on the current system.
    /// </summary>
    /// <param name="version">The version string.</param>
    /// <returns>
    ///   <c>true</c> if the specified .NET core version is installed; otherwise, <c>false</c>.
    /// </returns>
    public bool IsNetCoreVersionInstalled(Version version);
  }
}
