// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using System.Xml;
using System.Resources;
using Reflector.UserInterface;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class implements a project browser.
	/// </summary>
	public class ProjectBrowserView : TreeView, IPadContent, IMementoCapable
	{
		static readonly string nodeBuilderPath = "/SharpDevelop/Views/ProjectBrowser/NodeBuilders";

		AbstractBrowserNode highlightedNode = null;

		public static Font PlainFont = null;
		Font               boldFont  = null;
		Panel contentPanel = new Panel();
		static ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));

		public Control Control {
			get {
				return contentPanel;
			}
		}

		public string Title {
			get {
				return resourceService.GetString("MainWindow.Windows.ProjectScoutLabel");
			}
		}

		public string Icon {
			get {
				return "Icons.16x16.CombineIcon";
			}
		}

		public void RedrawContent()
		{
			BeginUpdate();
			AbstractBrowserNode.ShowExtensions = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			foreach (AbstractBrowserNode node in Nodes) {
				node.UpdateNaming();
			}
			EndUpdate();
		}
		
		static ProjectBrowserView()
		{
			projectBrowserImageList = new ImageList();
			projectBrowserImageList.ColorDepth = ColorDepth.Depth32Bit;
		}
		public ProjectBrowserView()
		{
			LabelEdit     = true;
			AllowDrop     = true;
			HideSelection = false;
			Dock          = DockStyle.Fill;
			
			ImageList = projectBrowserImageList;
			LabelEdit = false;

			WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += new EventHandler(ActiveWindowChanged);
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CombineOpened += new CombineEventHandler(OpenCombine);
			projectService.CombineClosed += new CombineEventHandler(CloseCombine);

			PlainFont = new Font(Font, FontStyle.Regular);
			boldFont  = new Font(Font, FontStyle.Bold);

			Font = boldFont;
			contentPanel.Controls.Add(this);
		}
		
		public void RefreshTree(Combine combine)
		{
			DisposeProjectNodes();
			Nodes.Clear();
			TreeNode treeNode = BuildCombineTreeNode(combine);
			SortUtility.SortedInsert(treeNode, Nodes, TreeNodeComparer.ProjectNode);
			combine.StartupPropertyChanged += new EventHandler(StartupPropertyChanged);
			StartupPropertyChanged(null, null);
			// .NET bugfix : have to expand the node to ensure the refresh
			// (Refresh won't work) tested 08/16/2002 Mike
			treeNode.Expand();
		}
		
		void OpenCombine(object sender, CombineEventArgs e)
		{
			try {
				RefreshTree(e.Combine);
			} catch (InvalidOperationException) {
				this.Invoke(new CombineEventHandler(OpenCombine), new object[] {sender, e});
			}
		}

		void CloseCombine(object sender, CombineEventArgs e)
		{
			try {
				DisposeProjectNodes();
				Nodes.Clear();
			} catch (InvalidOperationException) {
				this.Invoke(new CombineEventHandler(CloseCombine), new object[] {sender, e});
			}
		}

		void StartupPropertyChanged(object sender, EventArgs e)
		{
			Combine combine = ((AbstractBrowserNode)Nodes[0]).Combine;
			if (highlightedNode != null) {
				highlightedNode.NodeFont = PlainFont;
			}

			if (combine.SingleStartupProject) {
				foreach (AbstractBrowserNode node in Nodes[0].Nodes) {
					if (node is ProjectBrowserNode) {
						if (combine.SingleStartProjectName == node.Project.Name) {
							highlightedNode = node;
							node.NodeFont = null;
						}
					} else if (node is CombineBrowserNode) {
						if (combine.SingleStartProjectName == node.Combine.Name) {
							highlightedNode = node;
							node.NodeFont = null;
						}
					}
				}
			} else {
				highlightedNode   = (AbstractBrowserNode)Nodes[0];
				highlightedNode.NodeFont = boldFont;
			}
		}

		void DisposeProjectNodes()
		{
			if (Nodes.Count == 1) {
				Stack stack = new Stack();
				stack.Push(Nodes[0]);
				while (stack.Count > 0) {
					TreeNode node = (TreeNode)stack.Pop();
					if (node is IDisposable) {
						((IDisposable)node).Dispose();
					}
					foreach (TreeNode childNode in node.Nodes) {
						stack.Push(childNode);
					}
				}
			}
		}


		/// <summary>
		/// Searches AbstractBrowserNodeCollection recursively for a given file name.
		/// Note that the UserData properties for the files have to set to FileInformation
		/// or to ReferenceInformation for this method to work.
		/// </summary>
		AbstractBrowserNode GetNodeFromCollectionTreeByFileName(TreeNodeCollection collection, string fileName)
		{
			foreach (AbstractBrowserNode node in collection) {
				if (node.UserData is ProjectFile && ((ProjectFile)node.UserData).Name == fileName) {
					return node;
				}
				if (node.UserData is ProjectReference && ((ProjectReference)node.UserData).GetReferencedFileName(node.Project) == fileName) {
					return node;
				}
				
				AbstractBrowserNode childnode = GetNodeFromCollectionTreeByFileName(node.Nodes, fileName);
				if (childnode != null) {
					return childnode;
				}
			}
			return null;
		}


		/// <summary>
		/// Selectes the current active workbench window in the Project Browser Tree and ensures
		/// the visibility of this node.
		/// </summary>
		void ActiveWindowChanged(object sender, EventArgs e)
		{
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				string fileName = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.ContentName;

				AbstractBrowserNode node = GetNodeFromCollectionTreeByFileName(Nodes, fileName);
				if (node != null) {
					node.EnsureVisible();
					SelectedNode = node;
				}
			}
		}

		/// <summary>
		/// If you want to edit a node label. Select the node you want to edit and then
		/// call this method, instead of using the LabelEdit Property and the BeginEdit
		/// Method directly.
		/// </summary>
		public void StartLabelEdit()
		{
			AbstractBrowserNode selectedNode = (AbstractBrowserNode)SelectedNode;
			if (selectedNode != null && selectedNode.CanLabelEdited) {
				LabelEdit = true;
				selectedNode.BeforeLabelEdit();
				selectedNode.BeginEdit();
			}
		}

		/// <summary>
		/// Updates the combine tree, this method should be called, if the combine has
		/// changed (added a project/combine)
		/// </summary>
		public void UpdateCombineTree()
		{
			XmlElement storedTree = new TreeViewMemento(this).ToXmlElement(new XmlDocument());
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			CloseCombine(this,new CombineEventArgs(projectService.CurrentOpenCombine));
			OpenCombine(this, new CombineEventArgs(projectService.CurrentOpenCombine));
			((TreeViewMemento)new TreeViewMemento().FromXmlElement(storedTree)).Restore(this);
			ActiveWindowChanged(this, EventArgs.Empty);
		}

		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static AbstractBrowserNode BuildProjectTreeNode(IProject project)
		{
			IProjectNodeBuilder[] nodeBuilders = (IProjectNodeBuilder[])(AddInTreeSingleton.AddInTree.GetTreeNode(nodeBuilderPath).BuildChildItems(null)).ToArray(typeof(IProjectNodeBuilder));
			IProjectNodeBuilder   projectNodeBuilder = null;
			foreach (IProjectNodeBuilder nodeBuilder in nodeBuilders) {
				if (nodeBuilder.CanBuildProjectTree(project)) {
					projectNodeBuilder = nodeBuilder;
					break;
				}
			}
			if (projectNodeBuilder != null) {
				return projectNodeBuilder.BuildProjectTreeNode(project);
			}

			throw new NotImplementedException("can't create node builder for project type " + project.ProjectType);
		}

		/// <summary>
		/// This method builds a ProjectBrowserNode Tree out of a given combine.
		/// </summary>
		public static AbstractBrowserNode BuildCombineTreeNode(Combine combine)
		{
			CombineBrowserNode combineNode = new CombineBrowserNode(combine);
			
			// build subtree
			foreach (CombineEntry entry in combine.Entries) {
				TreeNode node = null;
				if (entry.Entry is IProject) {
					node = BuildProjectTreeNode((IProject)entry.Entry);
				} else {
					node = BuildCombineTreeNode((Combine)entry.Entry);
				}
				combineNode.Nodes.Add(node);
			}
			
			SortUtility.QuickSort(combineNode.Nodes, TreeNodeComparer.ProjectNode);
			
			return combineNode;
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			switch (keyData) {
				case Keys.F2:
					StartLabelEdit();
					break;
				default:
					return base.ProcessDialogKey(keyData);
			}
			return true;
		}

		protected override void OnAfterSelect(TreeViewEventArgs e)
		{ // set current project & current combine
			base.OnAfterSelect(e);
			AbstractBrowserNode node = (AbstractBrowserNode)e.Node;

			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CurrentSelectedProject = node.Project;
			projectService.CurrentSelectedCombine = node.Combine;
			PropertyPad.SetDesignableObject(node.UserData);
			
			MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
			ContextMenu = menuService.CreateContextMenu(this, node.ContextmenuAddinTreePath);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(e.X, e.Y);

			if (node != null) {
				SelectedNode = node;
			}
		}

		// open file with the enter key
		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
			if (e.KeyChar == '\r') {
				OnDoubleClick(e);
			}
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			if (SelectedNode != null && SelectedNode is AbstractBrowserNode) {
				((AbstractBrowserNode)SelectedNode).ActivateItem();
			}
		}

		protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
		{ // show open folder icons
			base.OnBeforeExpand(e);
			if (e.Node != null && e.Node is AbstractBrowserNode) {
				((AbstractBrowserNode)e.Node).BeforeExpand();
			}
		}

		protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
		{
			base.OnBeforeCollapse(e);
			if (e.Node != null && e.Node is AbstractBrowserNode) {
				((AbstractBrowserNode)e.Node).BeforeCollapse();
			}
		}

		protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
		{
			base.OnAfterLabelEdit(e);
			AbstractBrowserNode node = (AbstractBrowserNode)e.Node;
			LabelEdit = false;
			
			// we set the label ourself
			node.AfterLabelEdit(e.Label);
			
			e.CancelEdit = true;
			
			if(node.Parent != null) {
				SortUtility.QuickSort(node.Parent.Nodes, TreeNodeComparer.ProjectNode);
			}
			
			// save changes
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			projectService.SaveCombine();
		}
		
		
		static ImageList projectBrowserImageList  = null;
		static Hashtable projectBrowserImageIndex = new Hashtable();

		public static int GetImageIndexForImage(Image image)
		{
			if (projectBrowserImageIndex[image] == null) {
				projectBrowserImageList.Images.Add(image);
				projectBrowserImageIndex[image] = projectBrowserImageList.Images.Count - 1;
				return projectBrowserImageList.Images.Count - 1;
			}

			return (int)projectBrowserImageIndex[image];
		}

		public IXmlConvertable CreateMemento()
		{
			return new TreeViewMemento(this);
		}

		public void SetMemento(IXmlConvertable memento)
		{
			((TreeViewMemento)memento).Restore(this);
		}

		// Drag & Drop handling
		
		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			base.OnItemDrag(e);
			AbstractBrowserNode node = e.Item as AbstractBrowserNode;
			if (node != null) {
				DataObject dataObject = node.DragDropDataObject;

				if (dataObject != null) {
					DoDragDrop(dataObject, DragDropEffects.All);
				}
			}
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			e.Effect = DragDropEffects.Move | DragDropEffects.Copy | DragDropEffects.None;
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);

			Point clientcoordinate   = PointToClient(new Point(e.X, e.Y));
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(clientcoordinate);

			DragDropEffects effect = DragDropEffects.None;

			if ((e.KeyState & 8) > 0) { // CTRL key pressed.
				effect = DragDropEffects.Copy;
			} else {
				effect = DragDropEffects.Move;
			}
			e.Effect = node.GetDragDropEffect(e.Data, effect);

			if (e.Effect != DragDropEffects.None) {
				((Form)WorkbenchSingleton.Workbench).Activate();
				Select();
				SelectedNode = node;
			}
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);

			Point clientcoordinate   = PointToClient(new Point(e.X, e.Y));
			AbstractBrowserNode node = (AbstractBrowserNode)GetNodeAt(clientcoordinate);

			if (node == null) {
				return;
			}
			node.DoDragDrop(e.Data, e.Effect);
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			if(node.Parent != null) {
				SortUtility.QuickSort(node.Parent.Nodes, TreeNodeComparer.ProjectNode);
			}
			
			projectService.SaveCombine();
		}
		
		static ProjectBrowserNode GetRootProjectNode(AbstractBrowserNode node)
		{
			while (node != null) {
				if(node is ProjectBrowserNode) {
					return (ProjectBrowserNode)node;
				}
				node = (AbstractBrowserNode)node.Parent;
			}
			return null;
		}

		public static void MoveCopyFile(string filename, AbstractBrowserNode node, bool move, bool alreadyInPlace)
		{
			//			FileType type      = FileUtility.GetFileType(filename);
			bool     directory = fileUtilityService.IsDirectory(filename);
			if (
//			    type == FileType.Dll ||
//			    type == FileType.Resource ||
			    directory) { // insert reference
			    return;
			    }

			    Debug.Assert(directory || File.Exists(filename), "ProjectBrowserEventHandler.MoveCopyFile : source file doesn't exist");

			// search "folder" in which the node contains
			while (!(node is DirectoryNode || node is ProjectBrowserNode))  {
				node = (AbstractBrowserNode)node.Parent;
		       	if (node == null) {
		       		return;
		       	}
			}

			string name        = Path.GetFileName(filename);
			string baseDirectory = node is DirectoryNode ? ((DirectoryNode)node).FolderName : node.Project.BaseDirectory;
			string newfilename = alreadyInPlace ? filename : fileUtilityService.GetDirectoryNameWithSeparator(baseDirectory) + name;

			string oldrelativename = fileUtilityService.AbsoluteToRelativePath(baseDirectory, filename);
			string newrelativename = fileUtilityService.AbsoluteToRelativePath(baseDirectory, newfilename);

			AbstractBrowserNode oldparent = DefaultDotNetNodeBuilder.GetPath(oldrelativename, GetRootProjectNode(node), false);          // TODO : change this for more projects
			AbstractBrowserNode newparent = DefaultDotNetNodeBuilder.GetPath(newrelativename, GetRootProjectNode(node), alreadyInPlace);

			AbstractBrowserNode oldnode   = null; // if oldnode is == null the old file doesn't exist in current tree

			if (oldparent != null) {
				foreach (AbstractBrowserNode childnode in oldparent.Nodes) {
					if (childnode.Text == name) {
						oldnode = childnode;
						break;
					}
				}
			}

			if (oldnode != null && oldnode is DirectoryNode) { // TODO can't move folders yet :(
			                                                                                                             return;
			}

			if (oldparent == newparent && oldnode != null) { // move/copy to the same location
				return;
			}

			if (move) {
				if (filename != newfilename) {
					File.Copy(filename, newfilename);
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					fileService.RemoveFile(filename);
				}
				if (oldnode != null) {
					oldparent.Nodes.Remove(oldnode);
				}
			} else {
				if (filename != newfilename) {
					File.Copy(filename, newfilename);
				}
			}

			ProjectFile fInfo;
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			if (newparent.Project.IsCompileable(newfilename)) {
				fInfo = projectService.AddFileToProject(newparent.Project, newfilename, BuildAction.Compile);
			} else {
				fInfo = projectService.AddFileToProject(newparent.Project, newfilename, BuildAction.Nothing);
			}

			AbstractBrowserNode pbn = new FileNode(fInfo);
			SortUtility.SortedInsert(pbn, newparent.Nodes, TreeNodeComparer.ProjectNode);
			pbn.EnsureVisible();
			projectService.SaveCombine();
		}


		// ********* Own events
		protected virtual void OnTitleChanged(EventArgs e)
		{
			if (TitleChanged != null) {
				TitleChanged(this, e);
			}
		}

		protected virtual void OnIconChanged(EventArgs e)
		{
			if (IconChanged != null) {
				IconChanged(this, e);
			}
		}

		public event EventHandler TitleChanged;
		public event EventHandler IconChanged;
	}
}
