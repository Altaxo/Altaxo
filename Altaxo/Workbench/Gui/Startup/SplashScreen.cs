// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Altaxo.Gui.Startup
{
	internal sealed class SplashScreenForm : Form
	{
		private static SplashScreenForm splashScreen;
		private static List<string> requestedFileList = new List<string>();
		private static List<string> parameterList = new List<string>();
		private Bitmap bitmap;

		public static SplashScreenForm SplashScreen
		{
			get
			{
				return splashScreen;
			}
			set
			{
				splashScreen = value;
			}
		}

		public SplashScreenForm()
		{
			System.Reflection.Assembly startass = System.Reflection.Assembly.GetExecutingAssembly();
			Version version = startass.GetName().Version;
			string versionText = string.Format("Altaxo {0}.{1} build {2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
#if DEBUG
			versionText += " (debug)";
#endif

			FormBorderStyle = FormBorderStyle.None;
			StartPosition = FormStartPosition.CenterScreen;
			ShowInTaskbar = false;
			// Stream must be kept open for the lifetime of the bitmap
			bitmap = new Bitmap(typeof(SplashScreenForm).Assembly.GetManifestResourceStream("Altaxo.Resources.SplashScreen.jpg"));
			this.ClientSize = bitmap.Size;

			Font font = null;
			foreach (string fontFamilyName in new[] { "Microsoft Sans Serif", "Liberation Sans", "Verdana", "Arial", "Helvetica" })
			{
				try
				{
					font = new Font(fontFamilyName, 4); break;
				}
				catch (Exception)
				{
				}
			}

			{
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					if (null != font)
					{
						g.DrawString(versionText, font, Brushes.Black, 230 - 3 * versionText.Length, 14);
						font.Dispose();
					}
				}
			}
			BackgroundImage = bitmap;
		}

		public static void ShowSplashScreen()
		{
			splashScreen = new SplashScreenForm();
			splashScreen.Show();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (bitmap != null)
				{
					bitmap.Dispose();
					bitmap = null;
				}
			}
			base.Dispose(disposing);
		}

		public static string[] GetParameterList()
		{
			return parameterList.ToArray();
		}

		public static string[] GetRequestedFileList()
		{
			return requestedFileList.ToArray();
		}

		public static void SetCommandLineArgs(string[] args)
		{
			requestedFileList.Clear();
			parameterList.Clear();

			foreach (string arg in args)
			{
				if (arg.Length == 0) continue;
				if (arg[0] == '-' || arg[0] == '/')
				{
					int markerLength = 1;

					if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-')
					{
						markerLength = 2;
					}

					string param = arg.Substring(markerLength);
					// The SharpDevelop AddIn project template uses /addindir:"c:\temp\"
					// but that actually means the last quote is escaped.
					// This HACK makes this work anyways by replacing the trailing quote
					// with a backslash:
					if (param.EndsWith("\"", StringComparison.Ordinal))
						param = param.Substring(0, param.Length - 1) + "\\";
					parameterList.Add(param);
				}
				else
				{
					requestedFileList.Add(arg);
				}
			}
		}
	}
}
