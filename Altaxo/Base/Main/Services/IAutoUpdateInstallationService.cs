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
using System.Threading.Tasks;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Interface to a service that auto update the application.
  /// </summary>
  public interface IAutoUpdateInstallationService
  {
    /// <summary>Starts the installer program, when all presumtions are fullfilled.</summary>
    /// <param name="isApplicationCurrentlyStarting">If set to <c>true</c>, the application will be restarted after the installation is done.</param>
    /// <param name="commandLineArgs">Original command line arguments. Can be <c>null</c> when calling this function on shutdown.</param>
    /// <returns>True if the installer program was started. Then the application have to be shut down immediately. Returns <c>false</c> if the installer program was not started.</returns>
    bool Run(bool isApplicationCurrentlyStarting, string[] commandLineArgs);
  }
}
