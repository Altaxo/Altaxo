#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
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

  /// <summary>
  /// This record is used to store the version data together with the package information and the base URL for downloading the packages.
  /// </summary>
  /// <param name="versionData">The version data as a byte array.</param>
  /// <param name="package">Package information of one package.</param>
  /// <param name="baseUrl">The base URL for downloading the packages.</param>
  public record PackageWithVersionDataAndBaseUrl(byte[] versionData, PackageInfo package, string baseUrl);

  /// <summary>
  /// This record is used to store the version data together with the package information and the base URL for downloading the packages.
  /// </summary>
  /// <param name="versionData">The version data as a byte array.</param>
  /// <param name="packages">An array of package information.</param>
  /// <param name="baseUrl">The base URL for downloading the packages.</param>
  public record PackagesWithVersionDataAndBaseUrl(byte[] versionData, PackageInfo[] packages, string baseUrl);
}
