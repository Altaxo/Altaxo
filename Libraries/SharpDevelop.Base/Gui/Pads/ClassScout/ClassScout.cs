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
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Utility;
using System.Resources;
using System.Xml;
using System.Threading;
using System.Text;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using Reflector.UserInterface;

using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class ClassScoutTag
	{
		int    line;
		string filename;
		object tag;
		
		public int Line {
			get {
				return line;
			}
		}

		public string FileName {
			get {
				return filename;
			}
			set {
				filename = value;
			}
		}
		public object Tag {
			get {
				return tag;
			}
		}
		
		public ClassScoutTag(int line, string filename)
		{
			this.line     = line;
			this.filename = filename;
		}
		
		public ClassScoutTag(int line, string filename, object tag)
		{
			this.line = line;
			this.filename = filename;
			this.tag = tag;
		}
	}

	/// <summary>
	/// This class is the project scout tree view
	/// </summary>
	public class ClassScout : AbstractPadContent
	{
		TreeView treeView = new TreeView();
		Panel contentPanel = new Panel();
		int imageIndexOffset = -1;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		ParseInformationEventHandler addParseInformationHandler = null;
		ParseInformationEventHandler removeParseInformationHandler = null;
		Combine parseCombine;
		
		delegate void MyD();
		delegate void MyParseEventD(TreeNodeCollection nodes, ParseInformationEventArgs e);
		
		IClassScoutNodeBuilder classBrowserNodeBuilder = new DefaultDotNetClassScoutNodeBuilder();

		public override Control Control {
			get {
				return treeView;
			}
		}
		
		public AbstractClassScoutNode SelectedNode {
			get {
				return treeView.SelectedNode as AbstractClassScoutNode;
			}
			set {
				treeView.SelectedNode = value;
			}
		}
		
		public ImageList ImageList {
			get {
				return treeView.ImageList;
			}
		}

		public ClassScout() : base("${res:MainWindow.Windows.ClassScoutLabel}", "Icons.16x16.Class")
		{
			addParseInformationHandler = new ParseInformationEventHandler(OnParseInformationAdded);
			removeParseInformationHandler = new ParseInformationEventHandler(OnParseInformationRemoved);
			
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));

			treeView.ImageList  = classBrowserIconService.ImageList;

			imageIndexOffset                      = ImageList.Images.Count;
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			foreach (Image img in iconService.ImageList.Images) {
				treeView.ImageList.Images.Add(img);
			}

			treeView.LabelEdit     = false;
			treeView.HotTracking   = false;
			treeView.AllowDrop     = true;
			treeView.HideSelection = false;
			treeView.Dock          = DockStyle.Fill;

			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));

			projectService.CombineOpened += new CombineEventHandler(OnCombineOpen);
			projectService.CombineClosed += new CombineEventHandler(OnCombineClosed);

			AmbienceService ambienceService = (AmbienceService)ServiceManager.Services.GetService(typeof(AmbienceService));
			ambienceService.AmbienceChanged += new EventHandler(AmbienceChangedEvent);
			
			treeView.DoubleClick += new EventHandler(TreeViewDoubleClick);
			treeView.MouseDown += new MouseEventHandler(TreeViewMouseDown);
			treeView.MouseUp += new MouseEventHandler(TreeViewMouseUp);
			treeView.BeforeExpand += new TreeViewCancelEventHandler(TreeViewBeforeExpand);
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.FileRenamed += new FileEventHandler(RenameFile);
			fileService.FileRemoved += new FileEventHandler(RemoveFile);
		}
		
		void RemoveFile(object sender, FileEventArgs e)
		{
			string fileName = Path.GetFullPath(e.FileName).ToUpper();
			Stack stack = new Stack();
			foreach (TreeNode node in treeView.Nodes) {
				stack.Push(node);
			}
			
			while (stack.Count > 0) {
				TreeNode node = (TreeNode)stack.Pop();
				ClassScoutTag tag = node.Tag as ClassScoutTag;
				if (tag != null) {
					if (Path.GetFullPath(tag.FileName).ToUpper() == fileName) {
						node.Parent.Nodes.Remove(node);
					}
				} else {
					foreach (TreeNode child in node.Nodes) {
						stack.Push(child);
					}
				}
			}
		}
		void RenameFile(object sender, FileEventArgs e)
		{
			string fileName = Path.GetFullPath(e.SourceFile).ToUpper();
			Stack stack = new Stack();
			foreach (TreeNode node in treeView.Nodes) {
				stack.Push(node);
			}
			
			while (stack.Count > 0) {
				TreeNode node = (TreeNode)stack.Pop();
				ClassScoutTag tag = node.Tag as ClassScoutTag;
				if (tag != null) {
					if (Path.GetFullPath(tag.FileName).ToUpper() == fileName) {
						tag.FileName = e.TargetFile;
					}
				}
				foreach (TreeNode child in node.Nodes) {
					stack.Push(child);
				}
			}
		}
		
		void TreeViewBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			AbstractClassScoutNode node = e.Node as AbstractClassScoutNode;
			if (node != null) {
				node.BeforeExpand();
			}
		}
		
		void AmbienceChangedEvent(object sender, EventArgs e)
		{
			if (parseCombine != null) {
				DoPopulate();
			}
		}

		void OnCombineOpen(object sender, CombineEventArgs e)
		{
			treeView.Nodes.Clear();
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			treeView.Nodes.Add(new TreeNode(stringParserService.Parse("${res:ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode}")));
			StartCombineparse(e.Combine);
		}

		void OnCombineClosed(object sender, CombineEventArgs e)
		{
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			parserService.ParseInformationAdded -= addParseInformationHandler;
			parserService.ParseInformationRemoved -= removeParseInformationHandler;
			treeView.Nodes.Clear();
		}

		void OnParseInformationAdded(object sender, ParseInformationEventArgs e)
		{
			if (Thread.CurrentThread.IsBackground) {
				treeView.Invoke(new MyParseEventD(AddParseInformation2), new object[] { treeView.Nodes, e });
			} else {
				AddParseInformation2(treeView.Nodes, e);
			}
		}
		
		void OnParseInformationRemoved(object sender, ParseInformationEventArgs e)
		{
			if (Thread.CurrentThread.IsBackground) {
				treeView.Invoke(new MyParseEventD(RemoveParseInformation2), new object[] { treeView.Nodes, e });
			} else {
				RemoveParseInformation2(treeView.Nodes, e);
			}
		}
		
		void TreeViewDoubleClick(object sender, EventArgs e)
		{
			TreeNode node = SelectedNode;
			if (node != null) {
				ClassScoutTag tag = node.Tag as ClassScoutTag;
				if (tag != null) {
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					fileService.OpenFile(tag.FileName);
					
					IViewContent content = fileService.GetOpenFile(tag.FileName).ViewContent;
					if (content is IPositionable) {
						if (tag.Line > 0) {
							((IPositionable)content).JumpTo(tag.Line - 1, 0);
							content.Control.Focus();
						}
					}
				}
			}
		}
		
		void TreeViewMouseDown(object sender, MouseEventArgs e)
		{
			AbstractClassScoutNode node = treeView.GetNodeAt(e.X, e.Y) as AbstractClassScoutNode;

			if (node != null) {
				SelectedNode = node;
			}
		}
		
		void TreeViewMouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right && SelectedNode != null) {
				AbstractClassScoutNode selectedBrowserNode = (AbstractClassScoutNode)SelectedNode;
				if (selectedBrowserNode.ContextmenuAddinTreePath != null && selectedBrowserNode.ContextmenuAddinTreePath.Length > 0) {
					MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
					menuService.ShowContextMenu(this, selectedBrowserNode.ContextmenuAddinTreePath, treeView, e.X, e.Y);
				}
			}
		}
		
		void StartCombineparse(Combine combine)
		{
			parseCombine = combine;
			
			System.Threading.Thread t = new Thread(new ThreadStart(StartPopulating));
			t.IsBackground = true;
			t.Priority = ThreadPriority.Lowest;
			t.Start();
		}
		
		void StartPopulating()
		{
			ParseCombine(parseCombine);
			treeView.Invoke(new MyD(DoPopulate));
			IParserService parserService  = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
		}
		
		public void ParseCombine(Combine combine)
		{
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					ParseProject(((ProjectCombineEntry)entry).Project);
				} else {
					ParseCombine(((CombineCombineEntry)entry).Combine);
				}
			}
		}
		
		void ParseProject(IProject p)
		{
			if (p.ProjectType == "C#" || p.ProjectType == "VBNET") {
	 			foreach (ProjectFile finfo in p.ProjectFiles) {
					if (finfo.BuildAction == BuildAction.Compile) {
						IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
						parserService.ParseFile(finfo.Name);
					}
	 			}
			}
		}
		
		void DoPopulate()
		{
			treeView.BeginUpdate();
			treeView.Nodes.Clear();
			try {
				Populate(parseCombine, treeView.Nodes);
			} catch (Exception e) {
				MessageBox.Show(e.ToString(), "Parse Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			treeView.EndUpdate();
			IParserService parserService = (IParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IParserService));
			parserService.ParseInformationAdded   += addParseInformationHandler;
			parserService.ParseInformationRemoved += removeParseInformationHandler;
		}
		
		public void Populate(Combine combine, TreeNodeCollection nodes)
		{
			ClassBrowserIconsService classBrowserIconService = (ClassBrowserIconsService)ServiceManager.Services.GetService(typeof(ClassBrowserIconsService));
			TreeNode combineNode = new TreeNode(combine.Name);
			combineNode.SelectedImageIndex = combineNode.ImageIndex = classBrowserIconService.CombineIndex;
			foreach (CombineEntry entry in combine.Entries) {
				if (entry is ProjectCombineEntry) {
					Populate(((ProjectCombineEntry)entry).Project, combineNode.Nodes);
				} else {
					Populate(((CombineCombineEntry)entry).Combine, combineNode.Nodes);
				}
			}
			SortUtility.QuickSort(combineNode.Nodes, TreeNodeComparer.Default);
			nodes.Add(combineNode);
		}
		
		void Populate(IProject p, TreeNodeCollection nodes)
		{
			// only C# is currently supported.
			bool builderFound = false;
			if (classBrowserNodeBuilder.CanBuildClassTree(p)) {
				TreeNode prjNode = classBrowserNodeBuilder.BuildClassTreeNode(p, imageIndexOffset);
				nodes.Add(prjNode);
				prjNode.Tag = p;
				builderFound = true;
			}
			
			// no builder found -> create 'dummy' node
			if (!builderFound) {
				TreeNode prjNode = new TreeNode(p.Name);
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
				prjNode.SelectedImageIndex = prjNode.ImageIndex = imageIndexOffset + iconService.GetImageIndexForProjectType(p.ProjectType);
				prjNode.Nodes.Add(new TreeNode("No class builder found"));
				prjNode.Tag = p;
				nodes.Add(prjNode);
			}
		}
		
		void AddParseInformation2(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
//			treeView.BeginUpdate();
			AddParseInformation(nodes, e);
//			treeView.EndUpdate();
		}
		
		void AddParseInformation(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
			foreach (TreeNode node in nodes) {
				if (node.Tag is IProject) {
					IProject p = (IProject)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						classBrowserNodeBuilder.AddToClassTree(node, e);
					}
				} else {
					AddParseInformation(node.Nodes, e);
				}
			}
		}
		
		void RemoveParseInformation2(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
//			treeView.BeginUpdate();
			RemoveParseInformation(nodes, e);
//			treeView.EndUpdate();
		}
		
		void RemoveParseInformation(TreeNodeCollection nodes, ParseInformationEventArgs e)
		{
			for (int i = 0; i < nodes.Count; ++i) {
				TreeNode node = nodes[i];
				if (node.Tag is IProject) {
					IProject p = (IProject)node.Tag;
					if (p.IsFileInProject(e.FileName)) {
						classBrowserNodeBuilder.RemoveFromClassTree(node, e);
					}
				} else {
					RemoveParseInformation(node.Nodes, e);
				}
			}
		}
	}
}
