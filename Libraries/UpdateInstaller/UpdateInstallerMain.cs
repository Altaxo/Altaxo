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

namespace Altaxo.Serialization.AutoUpdates
{
	class UpdateInstallerMain
	{
		static void Main(string[] args)
		{
			// first arg is either 'stable' or 
			// second argument is the version string of the Altaxo executable
			// 3rd argument is the full path of the Altaxo executable file
			// 4th argument is either 0 or 1: if 1, then afterwards the updater has run, Altaxo is restarted again
			// 5th and more arguments: the original arguments of the Altaxo executable

			if (args.Length != 3)
				throw new ArgumentOutOfRangeException("Programm called with less than or more than 3 arguments");

			bool loadUnstableVersion;
			if (!PackageInfo.IsValidStableIdentifier(args[0], out loadUnstableVersion))
				throw new ArgumentException("first argument is not a valid stable identifier (is neither 'stable' nor 'unstable')");

			var currentProgramVersion = new Version(args[1]);

			var installer = new UpdateInstaller(loadUnstableVersion, currentProgramVersion,args[2]);
			installer.Run();
		}
	}
}
