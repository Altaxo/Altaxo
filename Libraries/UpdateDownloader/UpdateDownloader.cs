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
using System.Reflection;

namespace Altaxo.Serialization.AutoUpdates
{
	public class Downloader
	{
		string _storagePath;
		string _downloadURL;
		Version _currentVersion;

		/// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
		/// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it 
		/// looks for the latest stable version.</param>
		/// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
		public Downloader(bool loadUnstable, Version currentProgramVersion)
		{
			_currentVersion = currentProgramVersion;

			_storagePath = PackageInfo.GetDownloadDirectory(loadUnstable);

			if (loadUnstable)
				_downloadURL = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Unstable/";
			else
				_downloadURL = "http://downloads.sourceforge.net/project/altaxo/Altaxo/Altaxo-Latest-Stable/";
		}



		/// <summary>Runs the <see cref="Downloader"/>.</summary>
		/// <remarks>
		/// The download is done in steps:
		/// <para>Firstly, the appropriate version file in the application data directory is locked, 
		/// so that no other program can use it, until this program ends.</para>
		/// <para>Then, the version file is downloaded from the remote location.</para>
		/// <para>If there is already a valid version file in the download directory, 
		/// and the version obtained from the remote version file is equal to the version obtained from the version file in the download directory,
		/// then the package was already downloaded before. Then we only check that the package file is also present and that it has the appropriate hash sum.</para>
		/// <para>Else, if the version obtained from the remote version file is higher than the program's current version, 
		/// we download the package file from the remote location.</para>
		/// </remarks>
		public void Run()
		{
			if (!Directory.Exists(_storagePath))
				Directory.CreateDirectory(_storagePath);

			var versionFileFullName = Path.Combine(_storagePath, PackageInfo.VersionFileName);
			using (FileStream fs = new FileStream(versionFileFullName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
			{
				fs.Seek(0, SeekOrigin.Begin);
				var alreadyDownloadedVersion = PackageInfo.GetPresentDownloadedPackage(fs, _storagePath);
				fs.Seek(0, SeekOrigin.Begin);

				using (var webClient = new System.Net.WebClient())
				{
					Console.Write("Starting to download version file ...");
					var versionData = webClient.DownloadData(_downloadURL + PackageInfo.VersionFileName);
					Console.WriteLine(" ok! ({0} bytes downloaded)", versionData.Length);
					// we leave the file open, thus no other process can access it
					var parsedVersion = PackageInfo.FromStream(new MemoryStream(versionData));

					fs.Write(versionData, 0, versionData.Length);
					fs.Flush(); // write the new version to disc in order to change the write date

					if (null == alreadyDownloadedVersion || Comparer<Version>.Default.Compare(parsedVersion.Version, _currentVersion) > 0) // if the remote version is higher than the currently installed Altaxo version
					{
						Console.Write("Cleaning download directory ...");
						CleanDirectory(versionFileFullName); // Clean old downloaded files from the directory
						Console.WriteLine(" ok!");

						var packageUrl = _downloadURL + PackageInfo.GetPackageFileName(parsedVersion.Version);
						var packageFileName = Path.Combine(_storagePath, PackageInfo.GetPackageFileName(parsedVersion.Version));
						Console.Write("Start download of package file ...");
						webClient.DownloadFile(packageUrl, packageFileName); // download the package
						Console.WriteLine(" ok!");

						// make at least the test for the right length
						var fileInfo = new FileInfo(packageFileName);
						if (fileInfo.Length != parsedVersion.FileLength)
						{
							Console.WriteLine("Downloaded file length ({0}) differs from length in VersionInfo.txt {1}, thus the downloaded file will be deleted!", fileInfo.Length, parsedVersion.FileLength);
							fileInfo.Delete();
						}
						else
						{
							Console.WriteLine("Test file length of downloaded package file ... ok!");
						}
					}
				}
			}
		}


		/// <summary>Cleans the download directory from all package files.</summary>
		/// <param name="withExceptionOfThisFile">A file name, which should not be removed.</param>
		void CleanDirectory(string withExceptionOfThisFile)
		{
			try
			{
				var files = Directory.GetFiles(_storagePath, "Altaxo*.zip");
				foreach (var fileName in files)
				{
					if (0 == string.Compare(fileName, withExceptionOfThisFile, true))
						continue;
					File.Delete(fileName);
				}
			}
			catch (Exception)
			{
			}
		}

	}
}
