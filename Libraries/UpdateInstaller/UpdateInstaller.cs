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
			FileStream zipStream = null;

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
						Console.WriteLine("{0} {1}", ex.GetType().ToString(), ex.Message);
						System.Threading.Thread.Sleep(250);
					}
				} while (axoExe == null);



				Console.WriteLine("Altaxo now has ended and is ready to be updated");
				var zipFile = new ZipFile(fs);
				byte[] buffer = new byte[4096];

				foreach (ZipEntry entry in zipFile)
				{
					var destinationFileName = Path.Combine(_pathToInstallation,entry.Name);
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
		}
	}
}
