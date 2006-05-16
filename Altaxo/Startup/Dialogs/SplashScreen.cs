﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1139 $</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Resources;

namespace ICSharpCode.SharpDevelop
{
	public class SplashScreenForm : Form
	{
#if ModifiedForAltaxo
		public const string VersionText = "Altaxo 0.53 build " + RevisionClass.Revision;
#else
		public const string VersionText = "Serralongue build " + RevisionClass.Revision;
#endif

    static SplashScreenForm splashScreen;
		static ArrayList requestedFileList = new ArrayList();
		static ArrayList parameterList = new ArrayList();
		Bitmap bitmap;
		
		public static SplashScreenForm SplashScreen {
			get {
				if (splashScreen == null) {
					splashScreen = new SplashScreenForm();
				}
				return splashScreen;
			}
		}
		
		public SplashScreenForm()
		{
			#if !DEBUG
			TopMost         = true;
			#endif
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			#if DEBUG
			string versionText = VersionText + " (debug)";
			#else
			string versionText = VersionText;
			#endif
			using (Stream stream = Assembly.GetEntryAssembly().GetManifestResourceStream("Resources.SplashScreen.jpg")) {
				bitmap = new Bitmap(stream);
			}
			this.ClientSize = bitmap.Size;
			using (Font font = new Font("Sans Serif", 4)) {
				using (Graphics g = Graphics.FromImage(bitmap)) {
#if ModifiedForAltaxo
          g.DrawString(versionText, font, Brushes.Black, 124, 14);
#else
					g.DrawString(versionText, font, Brushes.Black, 100, 142);
#endif
				}
			}
			BackgroundImage = bitmap;
		}
		
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (bitmap != null) {
					bitmap.Dispose();
					bitmap = null;
				}
			}
			base.Dispose(disposing);
		}
		
		public static string[] GetParameterList()
		{
			return GetStringArray(parameterList);
		}
		
		public static string[] GetRequestedFileList()
		{
			return GetStringArray(requestedFileList);
		}
		
		static string[] GetStringArray(ArrayList list)
		{
			return (string[])list.ToArray(typeof(string));
		}
		
		public static void SetCommandLineArgs(string[] args)
		{
			requestedFileList.Clear();
			parameterList.Clear();
			
			foreach (string arg in args) {
				if (arg[0] == '-' || arg[0] == '/') {
					int markerLength = 1;
					
					if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-') {
						markerLength = 2;
					}
					
					parameterList.Add(arg.Substring(markerLength));
				} else {
					requestedFileList.Add(arg);
				}
			}
		}
	}
}
