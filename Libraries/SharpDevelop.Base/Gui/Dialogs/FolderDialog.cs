// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System.Windows.Forms.Design;
using System.Windows.Forms;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
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
		
		public FolderDialog()
		{
		}
		
		public DialogResult DisplayDialog()
		{
			return DisplayDialog("Select the directory in which the project will be created.");
		}
		
		// Alain VIZZINI reminded me to try out the .NET folder browser, because
		// the my documents bug seemed to have gone away ...
		public DialogResult DisplayDialog(string description)
		{
			FolderBrowser folderBrowser = new FolderBrowser();
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			folderBrowser.Description = stringParserService.Parse(description);
			DialogResult result = folderBrowser.ShowDialog();
			path = folderBrowser.DirectoryPath;
			return result;
		}
	}
}
