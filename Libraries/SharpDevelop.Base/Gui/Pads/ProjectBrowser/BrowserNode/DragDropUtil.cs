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
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class defines some Drag&Drop helper methods
	/// </summary>
	public sealed class DragDropUtil
	{
		public static bool CanDrag(FileNode fileNode, TreeNode newParent)
		{
			if (fileNode == null || fileNode.UserData == null) {
				return false;
			}
			foreach (AbstractBrowserNode node in newParent.Nodes) {
				if (node.UserData is ProjectFile) {
					if (Path.GetFileName(((ProjectFile)fileNode.UserData).Name) == Path.GetFileName(((ProjectFile)node.UserData).Name)) {
						return false;
					}
				}
			}
			return true;
		}
		
		public static FileNode DoDrag(FileNode fileNode, TreeNode newParent, DragDropEffects effect)
		{
			if (fileNode == null || fileNode.UserData == null) {
				return null;
			}
			switch (effect) {
				case DragDropEffects.Move:
					fileNode.Parent.Nodes.Remove(fileNode);
					break;
				case DragDropEffects.Copy:
					string addInTreePath = fileNode.ContextmenuAddinTreePath;
					fileNode = new FileNode((ProjectFile)((ProjectFile)fileNode.UserData).Clone());
					fileNode.ContextmenuAddinTreePath = addInTreePath;
					break;
				default:
					return null;
			}
			
			newParent.Nodes.Add(fileNode);
			return fileNode;
		}
		
		public static void DoDrop(FileNode fileNode, string baseDirectory, DragDropEffects effect)
		{
			if (fileNode == null || fileNode.UserData == null) {
				return;
			}
			ProjectFile fInfo = (ProjectFile)fileNode.UserData;
			
			Debug.Assert(fInfo != null);
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string newName = fileUtilityService.GetDirectoryNameWithSeparator(baseDirectory) + Path.GetFileName(fInfo.Name);
			
			switch (effect) {
				case DragDropEffects.Move:
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					fileService.RenameFile(fInfo.Name, newName);
					break;
				case DragDropEffects.Copy:
					try {
						File.Copy(fInfo.Name, newName);
					} catch (Exception) {}
					break;
				default:
					return;
			}
			fInfo.Name = newName;
		}
	}
}
