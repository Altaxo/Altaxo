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
using System.Collections;
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
	public class WebReferenceNode : AbstractBrowserNode
	{		
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/WebReferenceNode";
		ProjectFile file = null;

		/// <summary>
		/// Context menu path for any child nodes.
		/// </summary>
		public static readonly string ChildNodeContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/WebReferenceChildNode";
		
		public WebReferenceNode(ProjectFile file)
		{
			this.file = file;
			Text = Path.GetFileName(file.Name);
			contextmenuAddinTreePath = defaultContextMenuPath;
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			IconImage = resourceService.GetBitmap("Icons.16x16.WebReference");
		}
		
		/// <summary>
		/// Gets the WSDL uri for the web reference.
		/// </summary>
		public string Uri {
			get {
				return file.Data;
			}
		}
		
		/// <summary>
		/// Gets the name of the WSDL file for the web reference.
		/// </summary>
		public string WsdlFileName {
			get {
				ProjectFile file = GetWsdlFile();
				if (file != null) {
					return file.Name;		
				}
				return String.Empty;
			}
		}
		
		/// <summary>
		/// Gets the name of the web proxy file for the web reference.
		/// </summary>
		public string ProxyFileName {
			get {
				ProjectFile file = GetProxyFile();
				if (file != null) {
					return file.Name;		
				}
				return String.Empty;
			}
		}	
		
		/// <summary>
		/// Gets the name of the proxy namespace for the web reference.
		/// </summary>
		public string ProxyNamespace {
			get {
				ProjectFile file = GetProxyFile();
				if (file != null) {
					return file.Data;		
				}
				return String.Empty;
			}
		}		
		
		/// <summary>
		/// Removes the Web Reference from a project
		/// NOTE : This method assumes that its parent is 
		/// from the type 'ProjectBrowserNode'.
		/// </summary>
		public override bool RemoveNode()
		{
			if (file == null) {
				return false;
			}
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			int ret = new SharpMessageBox(resourceService.GetString("ProjectComponent.RemoveWebReference.Title"),
			                              String.Format(resourceService.GetString("ProjectComponent.RemoveWebReference.RemoveWebReferenceQuestion"), Text, Project.Name),
			                              resourceService.GetString("Global.DeleteButtonText"),
			                              resourceService.GetString("Global.CancelButtonText")).ShowMessageBox();
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			switch (ret) {
				case -1:
				case 1: 
					// Cancelled.
					return false;
				case 0: 
					// Delete web reference.
					fileService.RemoveFile(file.Name);
					// Inform parent node.
					WebReferencesNode parent = Parent as WebReferencesNode;
					if (parent != null) {
						parent.RemovingChild(this);
					}
					break;
			}
			return true;
		}			
		
		ProjectFile GetWsdlFile()
		{
			ProjectFile wsdlFile = null;
			
			foreach (AbstractBrowserNode node in Nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null) {
					ProjectFile projectFile = fileNode.UserData as ProjectFile;
					if (projectFile != null) {
						if (projectFile.Data == "WSDL") {
							wsdlFile = projectFile;
							break;
						}
					}
				}
			}
			
			return wsdlFile;
		}
		
		/// <summary>
		/// If the file is Subtype.Code and the BuildAction is Compile then
		/// it is assumed to be the proxy source file.
		/// </summary>
		ProjectFile GetProxyFile()
		{
			ProjectFile proxyFile = null;
			
			foreach (AbstractBrowserNode node in Nodes) {
				FileNode fileNode = node as FileNode;
				if (fileNode != null) {
					ProjectFile projectFile = fileNode.UserData as ProjectFile;
					if (projectFile != null) {
						if (projectFile.Subtype == Subtype.Code && projectFile.BuildAction == BuildAction.Compile) {
							proxyFile = projectFile;
							break;
						}
					}
				}
			}
			
			return proxyFile;
		}
	}
}
