// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 5888 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	public class SplashScreenForm : Form
	{
		static SplashScreenForm splashScreen;
		static List<string> requestedFileList = new List<string>();
		static List<string> parameterList = new List<string>();
		Bitmap bitmap;
		
		public static SplashScreenForm SplashScreen {
			get {
				return splashScreen;
			}
			set {
				splashScreen = value;
			}
		}
		
		public SplashScreenForm()
		{
#if ModifiedForAltaxo
      const string search1 = "=";
      string[] vs = null;
      System.Reflection.Assembly startass = System.Reflection.Assembly.GetExecutingAssembly();
      int idx1 = startass.FullName.IndexOf(search1);
      if (idx1 > 0)
        vs = startass.FullName.Substring(idx1 + search1.Length).Split(new char[] { '.', ' ', ',' }, 5);

      string versionText = (vs != null && vs.Length >= 4) ?
        ("Altaxo " + vs[0] + '.' + vs[1] + " build " + vs[3]) : 
        (startass.FullName);
      #if DEBUG
      versionText += " (debug)";
			#endif
#else
			const string versionText = "SharpDevelop"
				+ (RevisionClass.BranchName != null ? "-" + RevisionClass.BranchName : "")
				+ " " + RevisionClass.FullVersion
				#if DEBUG
				+ " (debug)"
				#endif
				;
#endif
			
			FormBorderStyle = FormBorderStyle.None;
			StartPosition   = FormStartPosition.CenterScreen;
			ShowInTaskbar   = false;
			// Stream must be kept open for the lifetime of the bitmap
			bitmap = new Bitmap(typeof(SplashScreenForm).Assembly.GetManifestResourceStream("Resources.SplashScreen.jpg"));
			this.ClientSize = bitmap.Size;
			using (Font font = new Font("Sans Serif", 4)) {
				using (Graphics g = Graphics.FromImage(bitmap)) {
#if ModifiedForAltaxo
          g.DrawString(versionText, font, Brushes.Black, 230 - 3 * versionText.Length, 14);
#else
					g.DrawString(versionText, font, Brushes.Black, 166 - 3 * versionText.Length, 142);
#endif
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
			
			foreach (string arg in args) {
				if (arg.Length == 0) continue;
				if (arg[0] == '-' || arg[0] == '/') {
					int markerLength = 1;
					
					if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-') {
						markerLength = 2;
					}
					
					string param = arg.Substring(markerLength);
					// work around .NET "feature" that causes trouble with /addindir:"c:\temp\"
					// http://www.mobzystems.com/code/bugingetcommandlineargs.aspx
					if (param.EndsWith("\"", StringComparison.Ordinal))
						param = param.Substring(0, param.Length - 1) + "\\";
					parameterList.Add(param);
				} else {
					requestedFileList.Add(arg);
				}
			}
		}
	}
}
