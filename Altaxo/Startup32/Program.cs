using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo
{
	public static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			string applicationFileName = System.Reflection.Assembly.GetEntryAssembly().Location;
			var path = System.IO.Path.GetDirectoryName(applicationFileName);
			var otherAppFileName = System.IO.Path.Combine(path, "AltaxoStartup.exe");
			AppDomain.CurrentDomain.ExecuteAssembly(otherAppFileName, args);
		}
	}
}