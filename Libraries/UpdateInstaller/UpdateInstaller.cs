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

using ICSharpCode.SharpZipLib.Zip;

namespace Altaxo.Serialization.AutoUpdates
{
	public class UpdateInstaller
	{
		const string PackListRelativePath = "doc\\PackList.txt";
		string _eventName;
		string _packageName;
		string _altaxoExecutableFullName;
		string _pathToInstallation;


		/// <summary>Initializes a new instance of the <see cref="Downloader"/> class.</summary>
		/// <param name="loadUnstable">If set to <c>true</c>, the <see cref="Downloader"/> take a look for the latest unstable version. If set to <c>false</c>, it 
		/// looks for the latest stable version.</param>
		/// <param name="currentProgramVersion">The version of the currently installed Altaxo program.</param>
		public UpdateInstaller(string eventName, string packageFullFileName, string altaxoExecutableFullFileName)
		{
			_eventName = eventName;
			_packageName = packageFullFileName;
			_altaxoExecutableFullName = altaxoExecutableFullFileName;


			_pathToInstallation = Path.GetDirectoryName(_altaxoExecutableFullName);
			if (!Path.IsPathRooted(_pathToInstallation))
				throw new ArgumentException("Path to Altaxo executable is not an absolute path!");

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



		public void Run()
		{
			string pathToInstallation = Path.GetDirectoryName(_altaxoExecutableFullName);

			using (FileStream fs = new FileStream(_packageName, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				fs.Seek(0, SeekOrigin.Begin);
				// signal Altaxo, that we have the stream now
				var waitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset, _eventName);
				waitHandle.Set();
				Console.WriteLine("Event has been set");

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
						Console.WriteLine("Waiting for shutdown of Altaxo: {0} {1}", ex.GetType().ToString(), ex.Message);
						System.Threading.Thread.Sleep(250);
					}
					finally
					{
						if(null!=axoExe)
							axoExe.Close();
					}
				} while (axoExe == null);



				Console.WriteLine("Altaxo now has ended and is ready to be updated");

				// remove the old files
				Console.Write("Remove old installation files...");
				RemoveOldInstallationFiles();
				Console.WriteLine("ok!");

				// now delete all orphaned directories in the installation directory
				Console.Write("Deleting orphaned directories...");
				DeleteDirIfOrphaned(new DirectoryInfo(_pathToInstallation));
				Console.WriteLine("ok!");

				// and extract the new files
				Console.WriteLine("Extracting new files...");
				ExtractPackageFiles(fs);

				fs.Close();
			}
		}



		private void ExtractPackageFiles(FileStream fs)
		{
			var zipFile = new ZipFile(fs);
			byte[] buffer = new byte[4096];

			foreach (ZipEntry entry in zipFile)
			{
				var destinationFileName = Path.Combine(_pathToInstallation, entry.Name);
				var destinationPath = Path.GetDirectoryName(destinationFileName);
				Console.WriteLine("{0} => {1}", entry.Name, destinationFileName);

				if (!Directory.Exists(destinationPath))
					Directory.CreateDirectory(destinationPath);

				using (var entryStream = zipFile.GetInputStream(entry))
				{
					using (var destStream = new FileStream(destinationFileName, FileMode.Create, FileAccess.Write, FileShare.None))
					{
						int bytesReaded;
						while (0 != (bytesReaded = entryStream.Read(buffer, 0, buffer.Length)))
						{
							destStream.Write(buffer, 0, bytesReaded);
						}
					}
				}
				File.SetLastWriteTime(destinationFileName, entry.DateTime);
			}
		}

		public bool PackListFileExists()
		{
			string fullName = Path.Combine(_pathToInstallation, PackListRelativePath);
			return File.Exists(fullName);
		}

		public bool PackListFileIsWriteable()
		{
			string fullName = Path.Combine(_pathToInstallation, PackListRelativePath);
			try
			{
				using (FileStream fs = new FileStream(fullName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
				{
					fs.Close();
					return true;
				}
			}
			catch (Exception ex)
			{
			}
			return false;
		}


		private void RemoveOldInstallationFiles()
		{
			byte[] buff = null;
			using (FileStream fso = new FileStream(Path.Combine(_pathToInstallation, PackListRelativePath), FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				buff = new byte[fso.Length];
				fso.Read(buff, 0, buff.Length);
				fso.Close();
			}
			// thus we ensured that PackList.txt is closed, so that it can be deleted now

			StreamReader str = new StreamReader(new MemoryStream(buff));
			string line;
			while (null != (line = str.ReadLine()))
			{
				line = line.Trim();
				if (line.Length == 0)
					continue;
				// each line consist of the length of the file and the relative file name, separated by a tab
				int idx = line.IndexOf('\t');
				if (idx < 1)
					throw new InvalidOperationException("Unrecognized format of line in PackList.txt");
				long fileLength = long.Parse(line.Substring(0, idx));
				string fileName = line.Substring(idx + 1);

				var info = new FileInfo(Path.Combine(_pathToInstallation, fileName));

				if (info.Exists && info.Length == fileLength)
					info.Delete();
			}
		}


		static bool DeleteDirIfOrphaned(DirectoryInfo dir)
		{
			bool isOrphaned = true;

			// delete orphaned subdirectories
			foreach (DirectoryInfo subdir in dir.GetDirectories())
			{
				if (!DeleteDirIfOrphaned(subdir))
					isOrphaned = false;
			}

			if (dir.GetFiles().Length != 0)
				isOrphaned = false; // not orphaned
			if (dir.GetDirectories().Length != 0)
				isOrphaned = false; // not orphaned


			// if the directory is orphaned, the it can be deleted
			if (isOrphaned)
			{
				dir.Delete();
			}

			return isOrphaned;
		}
	}
}
