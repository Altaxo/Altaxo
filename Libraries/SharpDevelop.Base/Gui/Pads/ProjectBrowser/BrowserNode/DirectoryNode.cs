// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default directory in the project browser.
	/// </summary>
	public class DirectoryNode : FolderNode 
	{
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/DefaultDirectoryNode";
				
		string folderName;
		
		/// <summary>
		/// This property gets the name of a directory for a 
		/// 'directory' folder. 
		/// </summary>
		public string FolderName {
			get {
				return folderName;
			}
			set {
				folderName = value;
				canLabelEdited = true;
			}
		}
		
		public DirectoryNode(string folderName) : base(Path.GetFileName(folderName))
		{
			this.folderName = folderName;
			canLabelEdited  = true;
			contextmenuAddinTreePath = defaultContextMenuPath;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap");
			ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap");
		}
		
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(FileNode)) && DragDropUtil.CanDrag((FileNode)dataObject.GetData(typeof(FileNode)), this)) {				
				return proposedEffect;
			}
			if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				return proposedEffect;
			}
			return DragDropEffects.None;
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			if (dataObject.GetDataPresent(typeof(FileNode))) {
				FileNode fileNode = DragDropUtil.DoDrag((FileNode)dataObject.GetData(typeof(FileNode)), this, effect);
				DragDropUtil.DoDrop(fileNode, folderName, effect);
			} else if (dataObject.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])dataObject.GetData(DataFormats.FileDrop);
				foreach (string file in files) {
					try {
						ProjectBrowserView.MoveCopyFile(file, this, effect == DragDropEffects.Move, false);
					} catch (Exception ex) {
						Console.WriteLine(ex.ToString());
					}
				}
			} else {
				throw new System.NotImplementedException();
			}
		}
		
		
		public override void AfterLabelEdit(string newName)
		{
			if (newName != null && newName.Trim().Length > 0) {
				
				string oldFoldername = folderName;
				string newFoldername = Path.GetDirectoryName(oldFoldername) + Path.DirectorySeparatorChar + newName;
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				
				if (oldFoldername != newFoldername) {
					try {
						
						IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
						FileUtilityService fileUtilityService = (FileUtilityService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(FileUtilityService));
						if (fileUtilityService.IsValidFileName(newFoldername)) {
							fileService.RenameFile(oldFoldername, newFoldername);
							Text       = newName;
							folderName = newFoldername;
						}
					} catch (System.IO.IOException) {   // assume duplicate file
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError("${res:Gui.ProjectBrowser.FileInUseError}");
					} catch (System.ArgumentException) { // new file name with wildcard (*, ?) characters in it
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError("${res:Gui.ProjectBrowser.IllegalCharactersInFileNameError}");
					}
				}
			}
		}
		
		/// <summary>
		/// Removes a folder from a project.
		/// Note : The FolderName property must be set for this method to work.
		/// </summary>
		public override bool RemoveNode()
		{
			if (FolderName != null && FolderName.Length == 0) {
				return false;
			}
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			int ret = new SharpMessageBox(resourceService.GetString("ProjectComponent.RemoveFolder.Title"),
			                              stringParserService.Parse(resourceService.GetString("ProjectComponent.RemoveFolder.Question"), new string[,] { {"FOLDER", Text}, {"PROJECT", Project.Name}}), 
			                              resourceService.GetString("Global.RemoveButtonText"),
			                              resourceService.GetString("Global.DeleteButtonText"),
			                              resourceService.GetString("Global.CancelButtonText")).ShowMessageBox();
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			switch (ret) {
				case -1:
				case 2:
					return false;
				case 0:
					projectService.RemoveFileFromProject(FolderName);
					break;
				case 1:
					fileService.RemoveFile(FolderName);
					break;
			}
			return true;
		}
	}
}
