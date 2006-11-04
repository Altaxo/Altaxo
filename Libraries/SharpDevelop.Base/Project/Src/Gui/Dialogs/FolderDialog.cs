﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System.Windows.Forms;
using System.Windows.Forms.Design;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This class helps to display the directory structure in the folder
	/// As the FolderBrowser is inaccessible we have to inherit from the
	/// FileNameBroswer and then call the method
	/// </summary>
	public class FolderDialog : FolderNameEditor
	{
		string path;
		
		public string Path {
			get {
				return path;
			}
		}
		
		// Alain VIZZINI reminded me to try out the .NET folder browser, because
		// the my documents bug seemed to have gone away ...
		public DialogResult DisplayDialog(string description)
		{
			using (FolderBrowser folderBrowser = new FolderBrowser()) {
				folderBrowser.Description = StringParser.Parse(description);
				DialogResult result = folderBrowser.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm);
				path = folderBrowser.DirectoryPath;
				LoggingService.Info("FolderDialog: user has choosen path " + path);
				if (path == null || path.Length == 0)
					return DialogResult.Cancel;
				return result;
			}
		}
	}
}
