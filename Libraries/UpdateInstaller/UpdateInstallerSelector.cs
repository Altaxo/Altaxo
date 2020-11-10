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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Altaxo.Serialization.AutoUpdates
{
  /// <summary>
  /// This installer schedules the installation process to one of two possible methods.
  /// The prefered method is to create another directory as sibling of the installation directory, install the files, and then swap the two directories.
  /// The other method is to create a temporary directory in the appdata directory, save all installation files there, and install the new files in the old directory.
  /// </summary>
  /// <seealso cref="Altaxo.Serialization.AutoUpdates.InstallerMethodBase" />
  /// <seealso cref="Altaxo.Serialization.AutoUpdates.IUpdateInstaller" />
  internal class UpdateInstallerSelector : InstallerMethodBase, IUpdateInstaller
  {
    /// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
    /// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it
    /// looks for the latest stable version.</param>
    /// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
    public UpdateInstallerSelector(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
      : base(eventName, packageFullFileName, altaxoExecutableFullFileName)
    {
      string subDirAltaxoShouldResideIn = "" + Path.DirectorySeparatorChar + "bin";
      if (_pathToInstallation.ToLowerInvariant().EndsWith(subDirAltaxoShouldResideIn))
      {
        _pathToInstallation = _pathToInstallation.Substring(0, _pathToInstallation.Length - subDirAltaxoShouldResideIn.Length);
      }
      else
      {
        throw new ArgumentException("Altaxo executable doesn't reside in 'bin' directory, thus this is not a normal installation and is therefore not updated");
      }
    }


    public void Run(Func<double, string, MessageKind, bool> ReportProgress)
    {
      IUpdateInstaller? subInstaller = null;

      if (IsParentDirectoryOfInstallationDirectoryWriteable())
      {
        // then we use the direct method
        subInstaller = new InstallerMethod_UseOuterDirectory(_eventName, _packageName, _altaxoExecutableFullName);
      }
      else
      {
        subInstaller = new InstallerMethod_BackupInnerDirectory(_eventName, _packageName, _altaxoExecutableFullName);
      }

      subInstaller.Run(ReportProgress);
    }
  }
}
