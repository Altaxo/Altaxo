﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1337 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public static class ToolbarService
	{
		public static ToolStripItem[] CreateToolStripItems(object owner, AddInTreeNode treeNode)
		{
			List<ToolStripItem> collection = new List<ToolStripItem>();
			foreach (object item in treeNode.BuildChildItems(owner)) {
				if (item is ToolStripItem) {
					collection.Add((ToolStripItem)item);
				} else {
					ISubmenuBuilder submenuBuilder = (ISubmenuBuilder)item;
					collection.AddRange(submenuBuilder.BuildSubmenu(null, owner));
				}
			}
			
			return collection.ToArray();
		}
		
		public static ToolStrip CreateToolStrip(object owner, AddInTreeNode treeNode)
		{
			ToolStrip toolStrip = new ToolStrip();
			toolStrip.Items.AddRange(CreateToolStripItems(owner, treeNode));
			UpdateToolbar(toolStrip); // setting Visible is only possible after the items have been added
			new LanguageChangeWatcher(toolStrip);
			return toolStrip;
		}
		
		class LanguageChangeWatcher {
			ToolStrip toolStrip;
			public LanguageChangeWatcher(ToolStrip toolStrip) {
				this.toolStrip = toolStrip;
				toolStrip.Disposed += Disposed;
				ResourceService.LanguageChanged += OnLanguageChanged;
			}
			void OnLanguageChanged(object sender, EventArgs e) {
				ToolbarService.UpdateToolbarText(toolStrip);
			}
			void Disposed(object sender, EventArgs e) {
				ResourceService.LanguageChanged -= OnLanguageChanged;
			}
		}
		
		public static ToolStrip CreateToolStrip(object owner, string addInTreePath)
		{
			return CreateToolStrip(owner, AddInTree.GetTreeNode(addInTreePath));
		}
		
		public static ToolStrip[] CreateToolbars(object owner, string addInTreePath)
		{
			AddInTreeNode treeNode;
			try {
				treeNode = AddInTree.GetTreeNode(addInTreePath);
			} catch (TreePathNotFoundException) {
				return null;
				
			}
			List<ToolStrip> toolBars = new List<ToolStrip>();
			foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values) {
				toolBars.Add(CreateToolStrip(owner, childNode));
			}
			return toolBars.ToArray();
		}
		
		public static void UpdateToolbar(ToolStrip toolStrip)
		{
			toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateStatus();
				}
			}
			toolStrip.ResumeLayout();
			//toolStrip.Refresh();
		}
		
		public static void UpdateToolbarText(ToolStrip toolStrip)
		{
			toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
				if (item is IStatusUpdate) {
					((IStatusUpdate)item).UpdateText();
				}
			}
			toolStrip.ResumeLayout();
		}
	}
}
