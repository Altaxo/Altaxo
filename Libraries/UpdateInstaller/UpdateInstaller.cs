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
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Altaxo.Serialization.AutoUpdates
{
	public class UpdateInstaller
	{

		string _storagePath;
		string _downloadURL;
		string _stableIdentifier;
		Version _currentVersion;
		string _altaxoExecutableFullName;


		/// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
		/// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it 
		/// looks for the latest stable version.</param>
		/// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
		public UpdateInstaller(bool loadUnstable, Version currentProgramVersion, string executablePath)
		{
			_currentVersion = currentProgramVersion;
			_stableIdentifier = loadUnstable ? "Unstable" : "Stable";

			_storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"Altaxo\\AutoUpdates\\" + _stableIdentifier);

			if(loadUnstable)
				_downloadURL = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Unstable/";
			else
				_downloadURL = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Stable/";
		}



		public void Run()
		{
			if (!Directory.Exists(_storagePath))
				Directory.CreateDirectory(_storagePath);

			var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);
			using (FileStream fs = new FileStream(versionFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				fs.Seek(0, SeekOrigin.Begin);
				var alreadyDownloadedVersion = PackageInfo.GetPresentDownloadedPackage(fs,_storagePath);

				if (null != alreadyDownloadedVersion && Comparer<Version>.Default.Compare(alreadyDownloadedVersion.Version, _currentVersion) > 0)
				{
					// warte auf das Close von Altaxo

					FileStream axoExe = null;
					do
					{
						try
						{
							axoExe = new FileStream(_altaxoExecutableFullName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
						}
						catch (Exception ex)
						{
							System.Threading.Thread.Sleep(500);
						}
					} while (axoExe == null);

				}
			}
		}
	}
}
