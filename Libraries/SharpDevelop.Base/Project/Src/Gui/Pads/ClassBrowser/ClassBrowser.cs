﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2104 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	[Flags]
	public enum ClassBrowserFilter
	{
		None = 0,
		ShowProjectReferences = 1,
		ShowBaseAndDerivedTypes = 32,
		
		ShowPublic = 2,
		ShowProtected = 4,
		ShowPrivate = 8,
		ShowOther = 16,
		
		All = ShowProjectReferences | ShowPublic | ShowProtected | ShowPrivate | ShowOther | ShowBaseAndDerivedTypes
	}
	
	public class ClassBrowserPad : AbstractPadContent
	{
		static ClassBrowserPad instance;
		
		
		public static ClassBrowserPad Instance {
			get {
				return instance;
			}
		}
		ClassBrowserFilter filter               = ClassBrowserFilter.All;
		Panel              contentPanel         = new Panel();
		ExtTreeView        classBrowserTreeView = new ExtTreeView();
		
		public ClassBrowserFilter Filter {
			get {
				return filter;
			}
			set {
				filter = value;
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					if (node is ExtTreeNode) {
						((ExtTreeNode)node).UpdateVisibility();
					}
				}
			}
		}
		
		public override Control Control {
			get {
				return contentPanel;
			}
		}
		ToolStrip toolStrip;
		ToolStrip searchStrip;
		
		void UpdateToolbars()
		{
			ToolbarService.UpdateToolbar(toolStrip);
			ToolbarService.UpdateToolbar(searchStrip);
		}
		
		public ClassBrowserPad()
		{
			instance = this;
			classBrowserTreeView.Dock         = DockStyle.Fill;
			classBrowserTreeView.ImageList    = ClassBrowserIconService.ImageList;
			classBrowserTreeView.AfterSelect += new TreeViewEventHandler(ClassBrowserTreeViewAfterSelect);
			
			contentPanel.Controls.Add(classBrowserTreeView);
			
			searchStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Searchbar");
			searchStrip.Stretch   = true;
			searchStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			contentPanel.Controls.Add(searchStrip);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ClassBrowser/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			contentPanel.Controls.Add(toolStrip);
			
			ProjectService.SolutionLoaded += ProjectServiceSolutionChanged;
			ProjectService.ProjectAdded += ProjectServiceSolutionChanged; // rebuild view when project is added to solution
			ProjectService.SolutionFolderRemoved += ProjectServiceSolutionChanged; // rebuild view when project is removed from solution
			ProjectService.SolutionClosed += ProjectServiceSolutionClosed;
			
			ParserService.ParseInformationUpdated += new ParseInformationEventHandler(ParserServiceParseInformationUpdated);
			
			AmbienceService.AmbienceChanged += new EventHandler(AmbienceServiceAmbienceChanged);
			if (ProjectService.OpenSolution != null) {
				ProjectServiceSolutionChanged(null, null);
			}
			UpdateToolbars();
		}
		
		List<ICompilationUnit[]> pending = new List<ICompilationUnit[]> ();
		
		// running on main thread, invoked by the parser thread when a compilation unit changed
		void UpdateThread()
		{
			lock (pending) {
				foreach (ICompilationUnit[] units in pending) {
					ICompilationUnit nonNullUnit = units[1] ?? units[0];
					foreach (TreeNode node in classBrowserTreeView.Nodes) {
						AbstractProjectNode prjNode = node as AbstractProjectNode;
						if (prjNode != null && prjNode.Project.IsFileInProject(nonNullUnit.FileName)) {
							prjNode.UpdateParseInformation(units[0], units[1]);
						}
					}
				}
				pending.Clear();
			}
		}
		
		public void ParserServiceParseInformationUpdated(object sender, ParseInformationEventArgs e)
		{
			lock (pending) {
				pending.Add(new ICompilationUnit[] { e.ParseInformation.MostRecentCompilationUnit as ICompilationUnit, e.CompilationUnit});
			}
			WorkbenchSingleton.SafeThreadAsyncCall(UpdateThread);
		}
		
		#region Navigation
		Stack<TreeNode> previousNodes = new Stack<TreeNode>();
		Stack<TreeNode> nextNodes     = new Stack<TreeNode>();
		bool navigateBack    = false;
		bool navigateForward = false;
		
		public bool CanNavigateBackward {
			get {
				if (previousNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					return false;
				}
				return previousNodes.Count > 0;
			}
		}
		
		public bool CanNavigateForward {
			get {
				if (nextNodes.Count == 1 && this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					return false;
				}
				return nextNodes.Count > 0;
			}
		}
		
		public void NavigateBackward()
		{
			if (previousNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == previousNodes.Peek()) {
					nextNodes.Push(previousNodes.Pop());
				}
				if (previousNodes.Count > 0) {
					navigateBack = true;
					this.classBrowserTreeView.SelectedNode = previousNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		public void NavigateForward()
		{
			if (nextNodes.Count > 0) {
				if (this.classBrowserTreeView.SelectedNode == nextNodes.Peek()) {
					previousNodes.Push(nextNodes.Pop());
				}
				if (nextNodes.Count > 0) {
					navigateForward = true;
					this.classBrowserTreeView.SelectedNode = nextNodes.Pop();
				}
			}
			UpdateToolbars();
		}
		
		void ClassBrowserTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			if (navigateBack) {
				nextNodes.Push(e.Node);
				navigateBack = false;
			} else {
				if (!navigateForward) {
					nextNodes.Clear();
				}
				previousNodes.Push(e.Node);
				navigateForward = false;
			}
			UpdateToolbars();
		}
		#endregion
		
		bool inSearchMode = false;
		List<TreeNode> oldNodes = new List<TreeNode>();
		string searchTerm = "";
		
		public bool IsInSearchMode {
			get {
				return inSearchMode;
			}
		}
		public string SearchTerm {
			get {
				return searchTerm;
			}
			set {
				searchTerm = value.ToUpper().Trim();
			}
		}
		
		public void StartSearch()
		{
			if (searchTerm.Length == 0) {
				CancelSearch();
				return;
			}
			if (!inSearchMode) {
				foreach (TreeNode node in classBrowserTreeView.Nodes) {
					oldNodes.Add(node);
				}
				inSearchMode = true;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
			classBrowserTreeView.BeginUpdate();
			classBrowserTreeView.Nodes.Clear();
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						foreach (IClass c in projectContent.Classes) {
							if (c.Name.ToUpper().StartsWith(searchTerm)) {
								ClassNodeBuilders.AddClassNode(classBrowserTreeView, project, c);
							}
						}
					}
				}
			}
			if (classBrowserTreeView.Nodes.Count == 0) {
				ExtTreeNode notFoundMsg = new ExtTreeNode();
				notFoundMsg.Text = ResourceService.GetString("MainWindow.Windows.ClassBrowser.NoResultsFound");
				notFoundMsg.AddTo(classBrowserTreeView);
			}
			classBrowserTreeView.Sort();
			classBrowserTreeView.EndUpdate();
		}
		
		public void CancelSearch()
		{
			if (inSearchMode) {
				classBrowserTreeView.Nodes.Clear();
				foreach (TreeNode node in oldNodes) {
					classBrowserTreeView.Nodes.Add(node);
				}
				oldNodes.Clear();
				inSearchMode = false;
				previousNodes.Clear();
				nextNodes.Clear();
				UpdateToolbars();
			}
		}
		
		void ProjectServiceSolutionChanged(object sender, EventArgs e)
		{
			classBrowserTreeView.Nodes.Clear();
			foreach (IProject project in ProjectService.OpenSolution.Projects) {
				if (project is MissingProject || project is UnknownProject) {
					continue;
				}
				ProjectNodeBuilders.AddProjectNode(classBrowserTreeView, project);
			}
		}
		
		void ProjectServiceSolutionClosed(object sender, EventArgs e)
		{
			classBrowserTreeView.Nodes.Clear();
			previousNodes.Clear();
			nextNodes.Clear();
			UpdateToolbars();
		}
		
		void AmbienceServiceAmbienceChanged(object sender, EventArgs e)
		{
		}
		
	}
}
