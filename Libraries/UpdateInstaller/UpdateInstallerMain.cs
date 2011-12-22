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
			// args[0]: the name of the event that must be signalled when the installer is ready to install
			// args[1]: name of the package file to use
			// args[2]: either 0 or 1, if 1 then Altaxo should be restarted after installation
			// args[3]: argument full name of the Altaxo executable
			// args[4]: and more arguments: the original arguments of the Altaxo executable

			if (args.Length <4 )
				throw new ArgumentOutOfRangeException("Programm called with less than 4 arguments");

			for (int i = 0; i < args.Length; ++i)
				Console.WriteLine("args[{0}]: {1}", i, args[i]);



			//var currentProgramVersion = new Version(args[1]);
			try
			{
				var installer = new UpdateInstaller(args[0], args[1], args[3]);
				installer.Run();
			}
			catch (Exception ex)
			{
				Console.WriteLine("{0} {1}",ex.GetType().ToString(), ex.Message);
			}

			Console.Write("Press any key:");
			Console.ReadKey();
		}
	}
}
