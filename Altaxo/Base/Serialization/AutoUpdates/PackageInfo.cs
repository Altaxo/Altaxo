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
	/// <summary>
	/// Information about a downloaded package.
	/// </summary>
	public class PackageInfo
	{
		/// <summary>Gets the version of the package.</summary>
		public Version Version { get; private set; }

		/// <summary>Gets the hash sum of the package file.</summary>
		public string Hash { get; private set; }

		/// <summary>Gets a value indicating whether this package is the unstable or the stable build of the program.</summary>
		public bool IsUnstableVersion { get; private set; }

		/// <summary>Gets the file length of the package.</summary>
		public long FileLength { get; private set; }


		/// <summary>Name (without path) of the version file, both at the remote location and on the local hard disk.</summary>
		public const string VersionFileName = "VersionInfo.txt";

		/// <summary>Gets the package info from the first line of the provided stream.</summary>
		/// <param name="fs">Stream to read from.</param>
		/// <returns>The package info. If the format of the stream is invalid, various exceptions will be thrown.</returns>
		public static PackageInfo FromStream(Stream fs)
		{
			StreamReader sr = new StreamReader(fs, true);
			string firstLine = sr.ReadLine();

			firstLine = firstLine.Trim();
			var entries = firstLine.Split(new char[] { '\t' }, StringSplitOptions.None);

			if (entries.Length != 4)
				throw new InvalidOperationException("First line doesn't contain 4 words, separated by tabulators");

			PackageInfo result = new PackageInfo();

			bool isUnstableVersion;
			if (!IsValidStableIdentifier(entries[0].Trim(), out isUnstableVersion))
				throw new InvalidOperationException("First item in line is neither 'stable' nor 'unstable'");
			result.IsUnstableVersion = isUnstableVersion;
			result.Version = new Version(entries[1].Trim());
			result.FileLength = long.Parse(entries[2].Trim());
			result.Hash = entries[3].Trim();

			return result;

		}

		/// <summary>Determines whether the provided string designates either the stable or the unstable build.</summary>
		/// <param name="s">The string..</param>
		/// <param name="isUnstable">On output, this returns true if the string designates the unstable build.</param>
		/// <returns><c>true</c> if the provided string designates either the stable or the unstable build; otherwise, <c>false</c>.</returns>
		public static bool IsValidStableIdentifier(string s, out bool isUnstable)
		{
			if (0 == string.Compare(s, "unstable", true))
			{
				isUnstable = true;
				return true;
			}
			else if (0 == string.Compare(s, "stable", true))
			{
				isUnstable = false;
				return true;
			}
			else
			{
				isUnstable = false;
				return false;
			}
		}


		/// <summary>Gets the hash as hexadecimal string.</summary>
		/// <param name="hash">The hash.</param>
		/// <returns>Hexadecimal string representing the hash.</returns>
		public static string GetHashAsString(byte[] hash)
		{
			var stb = new StringBuilder(hash.Length * 2);

			foreach (var b in hash)
				stb.Append(b.ToString("X2"));
			return stb.ToString();
		}


		/// <summary>Gets the name of the package file.</summary>
		/// <param name="version">The version of the package.</param>
		/// <returns>The package file name.</returns>
		public static string GetPackageFileName(Version version)
		{
			return "AltaxoBinaries-" + version.ToString(4) + ".zip";
		}


		/// <summary>Gets the stable identifier (either 'Unstable' or 'Stable').</summary>
		/// <param name="loadUnstable">Determines either to return 'Unstable' or 'Stable'</param>
		/// <returns>'Unstable' if <paramref name="loadUnstable"/> is <c>true</c>, otherwise 'Stable'.</returns>
		public static string GetStableIdentifier(bool loadUnstable)
		{
			return loadUnstable ? "Unstable" : "Stable";
		}

		/// <summary>Gets the directory where to store the downloaded update package.</summary>
		/// <param name="loadUnstableVersion"><c>true</c> when to get the folder for the unstable version; false otherwise.</param>
		/// <returns>The directory where to store the downloaded package.</returns>
		public static string GetDownloadDirectory(bool loadUnstableVersion)
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Altaxo\\AutoUpdates\\" + GetStableIdentifier(loadUnstableVersion));
		}



				/// <summary>If there already a package in the download directory, this function gets the present downloaded package. If anything is invalid (wrong format of the version file,
		/// wrong hash sum), the return value is <c>Null</c>.</summary>
		/// <param name="fs">Stream to read the version info from. This must be the opened stream of the VersionInfo.txt file in the download directory.</param>
		/// <param name="storagePath">Path to the directory that stores the downloaded package.</param>
		/// <returns>The info for the already present package in the download directory. If anything is invalid, the return value is null.</returns>
		public static PackageInfo GetPresentDownloadedPackage(Stream fs, string storagePath)
		{
			FileStream packageStream;
			var result = GetPresentDownloadedPackage(fs, storagePath, out packageStream);
			if (null != packageStream)
				packageStream.Close();

			return result;
		}

		/// <summary>If there already a package in the download directory, this function gets the present downloaded package. If anything is invalid (wrong format of the version file,
		/// wrong hash sum), the return value is <c>Null</c>.</summary>
		/// <param name="fs">Stream to read the version info from. This must be the opened stream of the VersionInfo.txt file in the download directory.</param>
		/// <param name="storagePath">Path to the directory that stores the downloaded package.</param>
		/// <param name="packageFile">On successfull return, the opened <see cref="FileStream"/> of the package file is provided here. You are responsible for closing the stream!</param>
		/// <returns>The info for the already present package in the download directory. If anything is invalid, the return value is null.</returns>
		public static PackageInfo GetPresentDownloadedPackage(Stream fs, string storagePath, out FileStream packageFile)
		{
			PackageInfo packageInfo = null;
			packageFile = null;
			try
			{
				packageInfo = PackageInfo.FromStream(fs);

				// test, if the file exists and has the right Hash
				var fileInfo = new FileInfo(Path.Combine(storagePath, GetPackageFileName(packageInfo.Version)));
				if (!fileInfo.Exists)
					return null;

				// test for the appropriate length
				if (fileInfo.Length != packageInfo.FileLength)
					return null;

				packageFile = new FileStream(Path.Combine(storagePath, GetPackageFileName(packageInfo.Version)), FileMode.Open, FileAccess.Read, FileShare.Read);
				var hashProvider = new System.Security.Cryptography.SHA1Managed();
				var hash = hashProvider.ComputeHash(packageFile);

				if (GetHashAsString(hash) != packageInfo.Hash)
					throw new InvalidOperationException("Hash of downloaded package is not valid");

				return packageInfo;
			}
			catch (Exception)
			{
			}

			return null;

		}

	}
}
