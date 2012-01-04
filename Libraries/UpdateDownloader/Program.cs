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
	class Program
	{
		/// <summary>
		/// Main entry point of the updater program.
		/// </summary>
		/// <param name="args">The arguments must contain:
		/// <para>args[0] is either 'stable' or 'unstable'</para>
		/// <para>args[1] is the version string of the currently installed Altaxo version (e.g.: 1.2.3.4).</para>
		/// </param>
		static void Main(string[] args)
		{
			try
			{

				if (args.Length != 2)
				{
					PrintUsage();
					throw new ArgumentOutOfRangeException("Programm called with less than or more than 2 arguments");
				}

				bool loadUnstableVersion;
				if (!PackageInfo.IsValidStableIdentifier(args[0], out loadUnstableVersion))
					throw new ArgumentException("First argument is not a valid stable identifier (is neither 'stable' nor 'unstable')");

				var currentProgramVersion = new Version(args[1]);

				var downLoader = new Downloader(loadUnstableVersion, currentProgramVersion);
				downLoader.Run();
			}
			catch (Exception ex) // catch all Exceptions silently
			{
				System.Environment.ExitCode = 1;
				Console.WriteLine(ex.ToString());

				Console.WriteLine();
				for (int i = 60; i >= 0; --i)
				{
					System.Threading.Thread.Sleep(1000);
					Console.WriteLine("Program ends in {0} s\r", i);
				}

			}
		}

		static void PrintUsage()
		{
			Console.WriteLine(
				"This program has to be called with 2 arguments:\r\n\r\n" +
				"args[0]: either 'stable' or 'unstable', depending on whether to download only the stable versions or both stable and unstable versions\r\n" +
				"args[1]: the version string of the currently installed Altaxo version\r\n\r\n"
				);
		}

	}
}
