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
	class UpdateInstallerMain
	{
		static void Main(string[] args)
		{
			System.Diagnostics.Debugger.Launch();

			try
			{
			// args[0]: the name of the event that must be signalled when the installer is ready to install
			// args[1]: name of the package file to use
			// args[2]: either 0 or 1, if 1 then Altaxo should be restarted after installation. Furthermore, Bit1=1 indicates that this process was started with elevated privileges.
			// args[3]: argument full name of the Altaxo executable
			// args[4]: and more arguments: the original arguments of the Altaxo executable

			if (args.Length <4 )
				throw new ArgumentOutOfRangeException("Programm called with less than 4 arguments");

			for (int i = 0; i < args.Length; ++i)
				Console.WriteLine("args[{0}]: {1}", i, args[i]);

			int options = int.Parse(args[2]);
			bool wasStartedWithElevatedPrivileges = 0 != (options & 2);
			bool restartAltaxo = (0 != (options & 1)) && !wasStartedWithElevatedPrivileges;


			//var currentProgramVersion = new Version(args[1]);

				var installer = new UpdateInstaller(args[0], args[1], args[3]);
				if (!installer.PackListFileExists())
					throw new InvalidOperationException("PackList.txt of old installation not found!");

				if (!installer.PackListFileIsWriteable())
				{
					if (0!=(options & 2))
						throw new InvalidOperationException("There is no write access to the PackList.txt file, thus probably there is also no write access to the installation directory");

					var templateProc = System.Diagnostics.Process.GetCurrentProcess().StartInfo;
					var proc = new System.Diagnostics.ProcessStartInfo(templateProc.FileName,templateProc.Arguments);
					args[2] = (options|2).ToString();
					var stb = new StringBuilder();
					foreach (var s in args)
						stb.AppendFormat("\"{0}\"\t", s);

					proc.Arguments = stb.ToString();
					proc.Verb = "runas";
					var runProcWithElevated = System.Diagnostics.Process.Start(proc);
						
					if(restartAltaxo) 
						runProcWithElevated.WaitForExit();
				}
				else
				{
					installer.Run();
				}

				if (restartAltaxo)
				{
					StringBuilder stb = new StringBuilder();
					for(int i=4;i<args.Length;++i)
						stb.AppendFormat("\"{0}\"\t",args[i]);
					System.Diagnostics.Process.Start(args[3], stb.ToString());
				}
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
