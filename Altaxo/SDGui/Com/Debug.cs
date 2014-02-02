#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2014 Dr. Dirk Lellinger
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

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altaxo.Com
{
#if COMLOGGING

	/// <summary>
	/// Determines the amount of debug information that is logged.
	/// </summary>
	public enum VerboseLevel
	{
		/// <summary>No information is logged.</summary>
		None = 0,

		/// <summary>Only fatal errors are logged.</summary>
		FatalErrors = 1,

		/// <summary>Errors and fatal errors are logged.</summary>
		Errors = 2,

		/// <summary>Warnings, errors and fatal errors are logged.</summary>
		Warnings = 3,

		/// <summary>Infos, warnings, errors and fatal errors are logged.</summary>
		Info = 4,

		/// <summary>Verbose infos, normal infos, warnings, errors and fatal errors are logged.</summary>
		VerboseInfo = 5
	}

	/// <summary>
	/// Collects errors, warnings, infos and debug information.
	/// </summary>
	public static class Debug
	{
		private static System.IO.StreamWriter _output;

		private static string _fullFileName;

		private static object _syncContext = new object();

		static Debug()
		{
			var startTime = System.Diagnostics.Process.GetCurrentProcess().StartTime;

			string path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

			var dateTimeString = startTime.ToString("yyyy-MM-dd HH-mm-ss.fff");

			string fileName = "ErrorLog_Process_" + dateTimeString + ".txt";

			_fullFileName = System.IO.Path.Combine(path, fileName);

			_output = new System.IO.StreamWriter(_fullFileName);
		}

		/// <summary>
		/// Determines the amount of debug information that is logged.
		/// </summary>
		public static VerboseLevel VerboseLevel { get; private set; }

		public static void Report(string level, string format, params object[] args)
		{
			StringBuilder stb = new StringBuilder(128);

			stb.Append(level);
			stb.Append('\t');
			stb.Append(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff"));
			stb.Append('\t');
			stb.AppendFormat("{0,15}", System.Threading.Thread.CurrentThread.Name ?? System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
			stb.Append('\t');
			stb.AppendFormat(format, args);

			try
			{
				lock (_syncContext)
				{
					_output.WriteLine(stb);
					_output.Flush();
					_output.BaseStream.Flush();
				}
			}
			catch (ObjectDisposedException)
			{
			}
		}

		/// <summary>
		/// Reports a fatal error.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportFatalError(string format, params object[] args)
		{
			Report("Fatal", format, args);
		}

		/// <summary>
		/// Reports an error.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportError(string format, params object[] args)
		{
			Report("Error", format, args);
		}

		/// <summary>
		/// Reports a warning.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportWarning(string format, params object[] args)
		{
			Report("Warning", format, args);
		}

		/// <summary>
		/// Reports an informational message.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportInfo(string format, params object[] args)
		{
			Report("Info", format, args);
		}

		/// <summary>
		/// Reports a debug string.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportDebug(string format, params object[] args)
		{
			Report("Debug", format, args);
		}

		/// <summary>
		/// Reports a statistical information.
		/// </summary>
		/// <param name="format">Format string.</param>
		/// <param name="args">Additional arguments.</param>
		public static void ReportStatistics(string format, params object[] args)
		{
			Report("Statistic", format, args);
		}
	}

#endif
}