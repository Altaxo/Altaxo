//
// SharpDevelop
//
// Copyright (C) 2005 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

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
	/// This class represents a web reference node.
	/// </summary>
	public class WebReferencesNode : NamedFolderNode 
	{
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/WebReferencesNode";
						
		string folderName = String.Empty;
		
		public WebReferencesNode(string folderName) : base("ProjectComponent.WebReferencesString", 2)
		{
			this.folderName = folderName;
			contextmenuAddinTreePath = defaultContextMenuPath;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenWebReferencesFolder");
			ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedWebReferencesFolder");
		}
		
		/// <summary>
		/// Removes the Web References folder from a project
		/// NOTE : This method assumes that its parent is 
		/// from the type 'ProjectBrowserNode'.
		/// </summary>
		public override bool RemoveNode()
		{
			return RemoveWebReferencesFolder(false);
		}		
		
		/// <summary>
		/// Called by a child WebReferenceNode when it has been removed.  
		/// This allows the main Web References Node to delete itself if there
		/// are no children left.
		/// </summary>
		public void RemovingChild(WebReferenceNode child)
		{
			if (LastChild(child)) {
				// Make sure web references folder is empty otherwise
				// do not delete the node - user may have added their
				// own files in this folder.
				if (IsFolderEmpty) {
					RemoveWebReferencesFolder(true);	
					Parent.Nodes.Remove(this);
				}
			}
		}
		
		/// <summary>
		/// Removes the web references folder and prompts the user if the silent
		/// flag is set to false.
		/// </summary>
		bool RemoveWebReferencesFolder(bool silent)
		{
			if (folderName != null && folderName.Length == 0) {
				return false;
			}
			
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			if (silent) {
				// Delete all web references.
				fileService.RemoveFile(folderName);				
			} else {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				
				int ret = new SharpMessageBox(resourceService.GetString("ProjectComponent.RemoveAllWebReferences.Title"),
				                              String.Format(resourceService.GetString("ProjectComponent.ProjectComponent.RemoveAllWebReferences.RemoveAllWebReferencesQuestion"), Project.Name),
				                              resourceService.GetString("Global.DeleteButtonText"),
				                              resourceService.GetString("Global.CancelButtonText")).ShowMessageBox();
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				switch (ret) {
					case -1:
					case 1: 
						// Cancelled.
						return false;
					case 0: 
						// Delete all web references.
						fileService.RemoveFile(folderName);
						break;
				}
			}
			
			return true;
		}
		
		/// <summary>
		/// Is this the last child node?
		/// </summary>
		bool LastChild(WebReferenceNode childNode)
		{
			bool lastChild = false;
			
			if (Nodes.Count == 1) {
				WebReferenceNode node = Nodes[0] as WebReferenceNode;
				if (node != null) {
					if (Object.ReferenceEquals(node, childNode)) {
						lastChild = true;
					}
				}
			}
			    
			return lastChild;
		}
		
		/// <summary>
		/// Checks that the web reference folder is empty.
		/// </summary>
		bool IsFolderEmpty {
			get {
				return (Directory.GetFiles(folderName).Length == 0) && (Directory.GetDirectories(folderName).Length == 0);
			}
		}
	}
}
