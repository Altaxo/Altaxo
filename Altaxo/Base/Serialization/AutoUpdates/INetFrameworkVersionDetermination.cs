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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Serialization.AutoUpdates
{
  public interface INetFrameworkVersionDetermination
  {
    /// <summary>
    /// Determines whether a specified version of the net framework is installed.
    /// </summary>
    /// <param name="version">The version. Examples are 4.0, 4.6, 4.7.1 etc.</param>
    /// <returns>
    ///   <c>true</c> if the specified version of the .NET framework is installed; otherwise, <c>false</c>.
    /// </returns>
    bool IsVersionInstalled(string version);
  }
}
