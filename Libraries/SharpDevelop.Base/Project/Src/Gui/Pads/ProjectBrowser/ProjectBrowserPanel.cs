﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1584 $</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Description of ProjectBrowserPanel.
	/// </summary>
	public class ProjectBrowserPanel : System.Windows.Forms.UserControl
	{
		ToolStrip             toolStrip;
		ProjectBrowserControl projectBrowserControl;
		ToolStripItem[]       standardItems;
		
		public AbstractProjectBrowserTreeNode SelectedNode {
			get {
				return projectBrowserControl.SelectedNode;
			}
		}
		
		public AbstractProjectBrowserTreeNode RootNode {
			get {
				return projectBrowserControl.RootNode;
			}
		}
		
		public ProjectBrowserControl ProjectBrowserControl {
			get {
				return projectBrowserControl;
			}
		}
		
		public ProjectBrowserPanel()
		{
			projectBrowserControl      = new ProjectBrowserControl();
			projectBrowserControl.Dock = DockStyle.Fill;
			Controls.Add(projectBrowserControl);
			
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/ProjectBrowser/ToolBar/Standard");
			toolStrip.ShowItemToolTips  = true;
			toolStrip.Dock = DockStyle.Top;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			toolStrip.Stretch   = true;
			standardItems = new ToolStripItem[toolStrip.Items.Count];
			toolStrip.Items.CopyTo(standardItems, 0);
			Controls.Add(toolStrip);
			projectBrowserControl.TreeView.BeforeSelect += TreeViewBeforeSelect;
		}
		
		void TreeViewBeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			UpdateToolStrip(e.Node as AbstractProjectBrowserTreeNode);
		}
		
		void UpdateToolStrip(AbstractProjectBrowserTreeNode node)
		{
			toolStrip.Items.Clear();
			toolStrip.Items.AddRange(standardItems);
			ToolbarService.UpdateToolbar(toolStrip);
			if (node != null && node.ToolbarAddinTreePath != null) {
				toolStrip.Items.Add(new ToolStripSeparator());
				toolStrip.Items.AddRange((ToolStripItem[])AddInTree.BuildItems(node.ToolbarAddinTreePath, node, false).ToArray(typeof(ToolStripItem)));
			}
		}
		
		public void ViewSolution(Solution solution)
		{
			UpdateToolStrip(null);
			projectBrowserControl.ViewSolution(solution);
		}
		
		/// <summary>
		/// Writes the current view state into the memento.
		/// </summary>
		public void StoreViewState(Properties memento)
		{
			projectBrowserControl.StoreViewState(memento);
			memento.Set("ProjectBrowserState", ExtTreeView.GetViewStateString(projectBrowserControl.TreeView));
		}
		
		/// <summary>
		/// Reads the view state from the memento.
		/// </summary>
		public void ReadViewState(Properties memento)
		{
			projectBrowserControl.ReadViewState(memento);
			ExtTreeView.ApplyViewStateString(memento.Get("ProjectBrowserState", ""), projectBrowserControl.TreeView);
		}
		
		public void Clear()
		{
			projectBrowserControl.Clear();
			UpdateToolStrip(null);
		}
		
		public void SelectFile(string fileName)
		{
			projectBrowserControl.SelectFile(fileName);
		}
	}
}
