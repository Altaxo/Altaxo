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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	public class DefaultDotNetNodeBuilder : IProjectNodeBuilder
	{
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));

		public bool CanBuildProjectTree(IProject project)
		{
			return true;
		}

		public static bool IsWebReference(AbstractBrowserNode node)
		{
			if (node != null) {
				if (node is ProjectBrowserNode)
					return false;
				if (node.Text == "Web References")
					return true;
				return IsWebReference((AbstractBrowserNode)node.Parent);
			}

			return false;
		}

		public AbstractBrowserNode BuildProjectTreeNode(IProject project)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			ProjectBrowserNode projectNode = new ProjectBrowserNode(project);

			projectNode.IconImage = iconService.GetImageForProjectType(project.ProjectType);

			FolderNode resourceNode = new NamedFolderNode("ProjectComponent.ResourceFilesString", 0);
			resourceNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ResourceFolderNode";
			resourceNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenResourceFolder");
			resourceNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedResourceFolder");
			projectNode.Nodes.Add(resourceNode);

			FolderNode referenceNode = new NamedFolderNode("ProjectComponent.ReferencesString", 1);
			referenceNode.ContextmenuAddinTreePath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ReferenceFolderNode";
			referenceNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenReferenceFolder");
			referenceNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedReferenceFolder");
			projectNode.Nodes.Add(referenceNode);

			// build a hash of projectFile items
			System.Collections.Hashtable fileHash = new Hashtable();

			// add items to the list by dependency order
			bool bDone = false;
			while(fileHash.Count != project.ProjectFiles.Count && bDone != true) {
				bool bAtLeastOneLoaded = false;
				bDone = true;
				foreach(ProjectFile projectFile in project.ProjectFiles) {
					if(fileHash.ContainsKey(projectFile.Name)) {
						continue;
					}

					bDone = false;

					if(projectFile.DependsOn != null && projectFile.DependsOn != String.Empty) {
						if(!fileHash.ContainsKey(projectFile.DependsOn)) {
							// cannot load yet
							continue;
						}
					}

					// flag to true in the hash
					bAtLeastOneLoaded = true;
					fileHash.Add(projectFile.Name, true);
				}
				if(!bAtLeastOneLoaded) {
					// we have dependencies that cannot be resolved
					// so we will add them without dependson
					foreach(ProjectFile projectFile in project.ProjectFiles) {
						if(!fileHash.ContainsKey(projectFile.Name)) {
							projectFile.DependsOn = String.Empty;
						}
					}
					break;
				}
			}

			// now we can load the files
			foreach(ProjectFile projectFile in project.ProjectFiles) {
				switch(projectFile.Subtype) {
					case Subtype.Code:
						// add a source file
						switch (projectFile.BuildAction) {
							case BuildAction.Exclude:
								// should we add?
								break;
							case BuildAction.EmbedAsResource:
								// add as a resource
								AbstractBrowserNode newResNode = new FileNode(projectFile);
								resourceNode.Nodes.Add(newResNode);
								break;
							default:
								// add everything else
								AddProjectFileNode(project, projectNode, projectFile);
								break;
						}
						break;
					default:
						// add everything else
						AddProjectFileNode(project, projectNode, projectFile);
						break;
				}
			}

			/*
			// create 'empty' directories
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if(project.ProjectFiles[i].Subtype == Subtype.WebReferences ) {
					string directoryName   = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, project.ProjectFiles[i].Name);
					// if directoryname starts with ./ oder .\
					if (directoryName.StartsWith(".")) {
						directoryName =  directoryName.Substring(2);
					}
					string parentDirectory = Path.GetFileName(directoryName);
					AbstractBrowserNode currentPathNode = GetPath(directoryName, projectNode, true);

					DirectoryNode newFolderNode  = new DirectoryNode(project.ProjectFiles[i].Name);
					newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenWebReferenceFolder");
					newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedWebReferenceFolder");
					currentPathNode.Nodes.Add(newFolderNode);

				}
				else if (project.ProjectFiles[i].Subtype == Subtype.Directory) {
					string directoryName   = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, project.ProjectFiles[i].Name);

					// if directoryname starts with ./ oder .\
					if (directoryName.StartsWith(".")) {
						directoryName =  directoryName.Substring(2);
					}

					string parentDirectory = Path.GetFileName(directoryName);

					AbstractBrowserNode currentPathNode = GetPath(directoryName, projectNode, true);

					DirectoryNode newFolderNode  = new DirectoryNode(project.ProjectFiles[i].Name);
					newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap");
					newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap");

					currentPathNode.Nodes.Add(newFolderNode);

				}
			}

			// create file tree
			for (int i = 0; i < project.ProjectFiles.Count; ++i) {
				if (project.ProjectFiles[i].Subtype != Subtype.Directory) {
					ProjectFile fileInformation = project.ProjectFiles[i];

					string relativeFile = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, fileInformation.Name);

					string fileName     = Path.GetFileName(fileInformation.Name);

					switch (fileInformation.BuildAction) {

						case BuildAction.Exclude:
							break;

						case BuildAction.EmbedAsResource:
							AbstractBrowserNode newResNode = new FileNode(fileInformation);
							resourceNode.Nodes.Add(newResNode);
							break;

						default:
							AbstractBrowserNode currentPathNode = GetPath(relativeFile, projectNode, true);

							AbstractBrowserNode newNode = new FileNode(fileInformation);
							newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
							currentPathNode.Nodes.Add(newNode);
							break;
					}
				}
			}
			*/

			InitializeReferences(referenceNode, project);
			SortUtility.QuickSort(referenceNode.Nodes, TreeNodeComparer.ProjectNode);
			SortUtility.QuickSort(resourceNode.Nodes, TreeNodeComparer.ProjectNode);
			SortUtility.QuickSort(projectNode.Nodes, TreeNodeComparer.ProjectNode);
			return projectNode;
		}

		public static void AddProjectFileNode(IProject project, AbstractBrowserNode projectNode, ProjectFile projectFile) {

			if(projectNode.TreeView != null)
				projectNode.TreeView.BeginUpdate();

			// only works for relative paths right now!
			AbstractBrowserNode parentNode = null;
			string relativeFile = fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory, projectFile.Name);
			string fileName     = Path.GetFileName(projectFile.Name);
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));

			parentNode = projectNode;

			if(projectFile.DependsOn != String.Empty && projectFile.DependsOn != null) {
				// make sure the dependant node exists
				AbstractBrowserNode dependNode = GetPath(fileUtilityService.AbsoluteToRelativePath(project.BaseDirectory,projectFile.DependsOn), projectNode, false);
				if(dependNode == null) {
					// dependsOn does not exist, do what?
				}

			}

			switch(projectFile.Subtype) {
				case Subtype.Code:
					// add a source file
					switch (projectFile.BuildAction) {

						case BuildAction.Exclude:
							break;

						case BuildAction.EmbedAsResource:
							// no resources
							break;

						default:
							AbstractBrowserNode currentPathNode1;
							currentPathNode1 = GetPath(relativeFile, parentNode, true);

							AbstractBrowserNode newNode = new FileNode(projectFile);
							newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
							//parentNode.Nodes.Add(newNode);
							
							SortUtility.SortedInsert(newNode, currentPathNode1.Nodes, TreeNodeComparer.ProjectNode);
							break;
					}
					break;
				case Subtype.Directory:
					{
						// add a directory
						string directoryName   = relativeFile;

						// if directoryname starts with ./ oder .\ //
						if (directoryName.StartsWith(".")) {
							directoryName =  directoryName.Substring(2);
						}

						string parentDirectory = Path.GetFileName(directoryName);

						AbstractBrowserNode currentPathNode;
						currentPathNode = GetPath(directoryName, parentNode, false);

						if(currentPathNode == null) {
							currentPathNode = parentNode;
							DirectoryNode newFolderNode  = new DirectoryNode(projectFile.Name);
							if(IsWebReference(currentPathNode)) {
								newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenWebReferenceFolder");
								newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedWebReferenceFolder");
							} else {
								newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenFolderBitmap");
								newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedFolderBitmap");
							}
							SortUtility.SortedInsert(newFolderNode, currentPathNode.Nodes, TreeNodeComparer.ProjectNode);
						}
					}
					break;
				case Subtype.WebReferences:
					{
						// add a web directory
						string directoryName   = relativeFile;
						// if directoryname starts with ./ oder .\ //
						if (directoryName.StartsWith(".")) {
							directoryName =  directoryName.Substring(2);
						}

						DirectoryNode newFolderNode  = new DirectoryNode(projectFile.Name);
						newFolderNode.OpenedImage = resourceService.GetBitmap("Icons.16x16.OpenWebReferenceFolder");
						newFolderNode.ClosedImage = resourceService.GetBitmap("Icons.16x16.ClosedWebReferenceFolder");

						string parentDirectory = Path.GetFileName(directoryName);
						projectNode.Nodes.Insert(2, newFolderNode);
					}
					break;
				case Subtype.WebForm:
					{
						// add the source file with the cool icon
						// set reference to the special context menu path
						AbstractBrowserNode currentPathNode1;
						currentPathNode1 = GetPath(relativeFile, parentNode, true);

						AbstractBrowserNode newNode = new FileNode(projectFile);
						newNode.IconImage = resourceService.GetBitmap("Icons.16x16.WebForm");
						newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
						//parentNode.Nodes.Add(newNode);
						
						SortUtility.SortedInsert(newNode, currentPathNode1.Nodes, TreeNodeComparer.ProjectNode);
						// codeBehind?
					}

					break;

				case Subtype.WinForm:
					{
						// add the source file with the cool icon
						// set reference to the special context menu path
						AbstractBrowserNode currentPathNode1;
						currentPathNode1 = GetPath(relativeFile, parentNode, true);

						AbstractBrowserNode newNode = new FileNode(projectFile);
						newNode.IconImage = resourceService.GetBitmap("Icons.16x16.WinForm");
						newNode.ContextmenuAddinTreePath = FileNode.ProjectFileContextMenuPath;
						//parentNode.Nodes.Add(newNode);
						
						SortUtility.SortedInsert(newNode, currentPathNode1.Nodes, TreeNodeComparer.ProjectNode);
					}

					break;
				case Subtype.XmlForm:
					// not supported yet
					break;
				case Subtype.Dataset:
					// not supported yet
					break;
				case Subtype.WebService:
					// not supported yet
					break;
				default:
					// unknown file type
					break;
			}
			if(projectNode.TreeView != null)
				projectNode.TreeView.EndUpdate();
		}

		public static AbstractBrowserNode GetProjectNode(AbstractBrowserNode childNode) {
			// find and return the project node if it exists
			AbstractBrowserNode parentNode = childNode;
			while(parentNode != null) {
				if(parentNode is ProjectBrowserNode) {
					break;
				}
				parentNode = (AbstractBrowserNode)parentNode.Parent;
			}
			// this could be null!!!
			return parentNode;
		}

		public static AbstractBrowserNode GetNodeFromCollection(TreeNodeCollection collection, string title)
		{
			foreach (AbstractBrowserNode node in collection) {
				if (node.Text == title) {
					return node;
				}
			}
			return null;
		}


		public static AbstractBrowserNode GetPath(string filename, AbstractBrowserNode root, bool create)
		{
			string directory    = Path.GetDirectoryName(filename);
			string[] treepath   = directory.Split(new char[] { Path.DirectorySeparatorChar });
			AbstractBrowserNode curpathnode = root;

			foreach (string path in treepath) {
				if (path.Length == 0 || path[0] == '.') {
					continue;
				}

				AbstractBrowserNode node = GetNodeFromCollection(curpathnode.Nodes, path);

				if (node == null) {
					if (create) {
						DirectoryNode newFolderNode  = new DirectoryNode(fileUtilityService.GetDirectoryNameWithSeparator(ConstructFolderName(curpathnode)) + path);
						SortUtility.SortedInsert(newFolderNode, curpathnode.Nodes, TreeNodeComparer.ProjectNode);
						curpathnode = newFolderNode;
						continue;
					} else {
						return null;
					}
				}
				curpathnode = node;
			}

			return curpathnode;
		}

		static string ConstructFolderName(AbstractBrowserNode folderNode)
		{
			if (folderNode is DirectoryNode) {
				return ((DirectoryNode)folderNode).FolderName;
			}

			if (folderNode is ProjectBrowserNode) {
				return ((ProjectBrowserNode)folderNode).Project.BaseDirectory;
			}

			throw new ApplicationException("Folder name construction failed, got unexpected parent node :" +  folderNode);
		}

		public static AbstractBrowserNode GetNodeByName(string name) {
			return null;
		}

		public static void InitializeReferences(AbstractBrowserNode parentNode, IProject project)
		{
			parentNode.Nodes.Clear();
			foreach (ProjectReference referenceInformation in project.ProjectReferences) {
				string name = null;
				switch (referenceInformation.ReferenceType) {
					case ReferenceType.Typelib:
						int index = referenceInformation.Reference.IndexOf("|");
						if (index > 0) {
							name = referenceInformation.Reference.Substring(0, index);
						} else {
							name = referenceInformation.Reference;
						}
						break;
					case ReferenceType.Project:
						name = referenceInformation.Reference;
						break;
					case ReferenceType.Assembly:
						name = Path.GetFileName(referenceInformation.Reference);
						break;
					case ReferenceType.Gac:
						name = referenceInformation.Reference.Split(',')[0];
						break;
					default:
						throw new NotImplementedException("reference type : " + referenceInformation.ReferenceType);
				}

				AbstractBrowserNode newReferenceNode = new ReferenceNode(referenceInformation);
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				newReferenceNode.IconImage = resourceService.GetBitmap("Icons.16x16.Reference");

				parentNode.Nodes.Add(newReferenceNode);
			}
			SortUtility.QuickSort(parentNode.Nodes, TreeNodeComparer.ProjectNode);
		}
		
	}
}
