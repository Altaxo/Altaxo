// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Commands.TabStrip
{
	public class CloseFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			OpenFileTab tab  = (OpenFileTab)Owner;
			if (tab != null && tab.TabPages != null && tab.SelectedIndex >= 0 && tab.SelectedIndex < tab.TabPages.Count) {
				OpenFileTab.MyTabPage selected = (OpenFileTab.MyTabPage)tab.TabPages[tab.SelectedIndex];
				
				IWorkbenchWindow window = tab.ClickedWindow;
				
				if (window != null) {
					window.CloseWindow(false);
					if (window != selected.Window) {
						selected.SelectPage();
					}
				}
			}
		}
	}

	public class SaveFileTab : AbstractMenuCommand
	{
		public override void Run()
		{
			OpenFileTab tab = (OpenFileTab)Owner;
			if (tab != null && tab.TabPages != null && tab.SelectedIndex >= 0 && tab.SelectedIndex < tab.TabPages.Count) {
				OpenFileTab.MyTabPage selected = (OpenFileTab.MyTabPage)tab.TabPages[tab.SelectedIndex];
				
				IWorkbenchWindow window = tab.ClickedWindow;
				
				if (window != null) {
					if (window.ViewContent.IsViewOnly) {
						return;
					}
					if (window.ViewContent.IsUntitled) {
						SaveFileAsTab.SaveFileAs(window);
					} else {
						IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
						projectService.MarkFileDirty(window.ViewContent.ContentName);
						
						FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
						fileUtilityService.ObservedSave(new FileOperationDelegate(window.ViewContent.Save), window.ViewContent.ContentName);
					}
				}
			}
		}
	}
	
	public class SaveFileAsTab : AbstractMenuCommand
	{
		public static void SaveFileAs(IWorkbenchWindow window)
		{
			using (SaveFileDialog fdiag = new SaveFileDialog()) {
				fdiag.OverwritePrompt = true;
				fdiag.AddExtension    = true;
				
			 	fdiag.Filter          = String.Join("|", (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(null)).ToArray(typeof(string)));
				
				string[] fileFilters  = (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/FileFilter").BuildChildItems(null)).ToArray(typeof(string));
				fdiag.Filter          = String.Join("|", fileFilters);
				for (int i = 0; i < fileFilters.Length; ++i) {
					if (fileFilters[i].IndexOf(Path.GetExtension(window.ViewContent.ContentName == null ? window.ViewContent.UntitledName : window.ViewContent.ContentName)) >= 0) {
						fdiag.FilterIndex = i + 1;
						break;
					}
				}
				
				if (fdiag.ShowDialog() == DialogResult.OK) {
					string fileName = fdiag.FileName;
					// currently useless, because the fdiag.FileName can't
					// handle wildcard extensions :(
					if (Path.GetExtension(fileName).StartsWith("?") || Path.GetExtension(fileName) == "*") {
						fileName = Path.ChangeExtension(fileName, "");
					}
					
					window.ViewContent.Save(fileName);
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowMessage(fileName, "File saved");
				}
			}
		}
		
		public override void Run()
		{
			OpenFileTab tab = (OpenFileTab)Owner;
			if (tab != null && tab.TabPages != null && tab.SelectedIndex >= 0 && tab.SelectedIndex < tab.TabPages.Count) {
				OpenFileTab.MyTabPage selected = (OpenFileTab.MyTabPage)tab.TabPages[tab.SelectedIndex];
				
				IWorkbenchWindow window = tab.ClickedWindow;
				
				if (window != null) {
					if (window.ViewContent.IsViewOnly) {
						return;
					}
					SaveFileAs(window);
				}
			}
		}
	}
	
	
	public class CopyPathName : AbstractMenuCommand
	{
		public override void Run()
		{
			OpenFileTab tab = (OpenFileTab)Owner;
			
			IWorkbenchWindow window = tab.ClickedWindow;
			
			if (window != null && window.ViewContent.ContentName != null) {
				Clipboard.SetDataObject(new DataObject(DataFormats.Text, window.ViewContent.ContentName));
			}
		}
	}
}
