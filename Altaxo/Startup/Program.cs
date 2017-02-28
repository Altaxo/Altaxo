// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace Altaxo
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public static class Program
	{
		/// <summary>
		/// Starts Altaxo
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			Altaxo.Gui.Startup.AltaxoStartupMain.Main(args);
		}
	}
}