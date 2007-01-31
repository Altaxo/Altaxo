﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2028 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace SearchAndReplace
{
	/// <summary>
	/// Specifies the format for viewing search results in the <see cref="SearchResultPanel"/>.
	/// </summary>
	/// <remarks>
	/// The members of this enumeration are used to auto-generate the 
	/// 'Select Search Mode' drop-down menu in the <see cref="SearchResultPanel"/>.
	/// </remarks>
	public enum SearchResultPanelViewMode
	{
		Flat,
		PerFile,
		//Structural // TODO: SearchAndReplace - develop Structural view
	}
	
	public class SearchResultPanel : AbstractPadContent, IOwnerState
	{
		static SearchResultPanel instance;
		
		public static SearchResultPanel Instance {
			get {
				return instance;
			}
		}
		
		Panel       myPanel        = new Panel();
		ExtTreeView resultTreeView = new ExtTreeView();
		ToolStrip   toolStrip;
		
		string             curPattern = null;
		List<SearchResult> curResults = null;
		
		public override Control Control {
			get {
				return myPanel;
			}
		}
		
		public SearchResultPanelViewMode ViewMode {
			get {
				return PropertyService.Get("SearchAndReplace.SearchResultPanelViewMode", SearchResultPanelViewMode.Flat);
			}
			set {
				PropertyService.Set("SearchAndReplace.SearchResultPanelViewMode", value);
				ShowSearchResults(curPattern, curResults);
			}
		}
		
		public void ExpandAll()
		{
			Stack<TreeNode> nodes = new Stack<TreeNode>();
			foreach (TreeNode node in resultTreeView.Nodes) {
				nodes.Push(node);
			}
			while (nodes.Count > 0) {
				TreeNode node = nodes.Pop();
				node.Expand();
				foreach (TreeNode childNode in node.Nodes) {
					nodes.Push(childNode);
				}
			}
		}
		public void Clear()
		{
			resultTreeView.Nodes.Clear();
		}
		
		public void CollapseAll()
		{
			Stack<TreeNode> nodes = new Stack<TreeNode>();
			foreach (TreeNode node in resultTreeView.Nodes) {
				nodes.Push(node);
			}
			while (nodes.Count > 0) {
				TreeNode node = nodes.Pop();
				node.Collapse();
				foreach (TreeNode childNode in node.Nodes) {
					nodes.Push(childNode);
				}
			}
		}
		
		void ShowSearchResultsPerFile()
		{
			Dictionary<string, SearchFolderNode> folderNodes = new Dictionary<string, SearchFolderNode>();
			foreach (SearchResult result in curResults) {
				if (!folderNodes.ContainsKey(result.FileName)) {
					folderNodes[result.FileName] = new SearchFolderNode(result.FileName);
				}
				folderNodes[result.FileName].Results.Add(result);
			}
			
			SearchRootNode searchRootNode = new SearchRootNode(curPattern, curResults, folderNodes.Count);
			foreach (SearchFolderNode folderNode in folderNodes.Values) {
				folderNode.SetText();
				searchRootNode.Nodes.Add(folderNode);
			}
			
			resultTreeView.Nodes.Add(searchRootNode);
			searchRootNode.Expand();
		}
		
		void ShowSearchResultsFlat()
		{
			Dictionary<string, SearchFolderNode> folderNodes = new Dictionary<string, SearchFolderNode>();
			foreach (SearchResult result in curResults) {
				if (!folderNodes.ContainsKey(result.FileName)) {
					folderNodes[result.FileName] = new SearchFolderNode(result.FileName);
				}
				folderNodes[result.FileName].Results.Add(result);
			}
			
			SearchRootNode searchRootNode = new SearchRootNode(curPattern, curResults, folderNodes.Count);
			foreach (SearchFolderNode folderNode in folderNodes.Values) {
				folderNode.PerformInitialization();
				foreach (SearchResultNode node in folderNode.Nodes) {
					node.ShowFileName = true;
					searchRootNode.Nodes.Add(node);
				}
			}
			
			resultTreeView.Nodes.Add(searchRootNode);
			searchRootNode.Expand();
		}
		
		public void ShowSearchResults(string pattern, List<SearchResult> results)
		{
			RemoveSpecialPanel();
			
			this.curPattern = pattern;
			this.curResults = results;
			if (results == null) {
				return;
			}
			resultTreeView.BeginUpdate();
			resultTreeView.Nodes.Clear();
			
			switch (ViewMode) {
				case SearchResultPanelViewMode.PerFile:
					ShowSearchResultsPerFile();
					break;
				case SearchResultPanelViewMode.Flat:
					ShowSearchResultsFlat();
					break;
			}
			
			
			resultTreeView.EndUpdate();
		}
		
		public SearchResultPanel()
		{
			instance = this;
			
			resultTreeView.Dock = DockStyle.Fill;
			resultTreeView.Font = ExtTreeNode.RegularBigFont;
			resultTreeView.IsSorted = false;
			toolStrip = ToolbarService.CreateToolStrip(this, "/SharpDevelop/Pads/SearchResultPanel/Toolbar");
			toolStrip.Stretch   = true;
			toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			
			myPanel.Controls.AddRange(new Control[] { resultTreeView, toolStrip} );
		}
		
		// Special panel mode:
		enum SearchResultPanelOwnerState
		{
			DefaultMode = 1,
			SpecialPanelMode = 2,
			// warning: OwnerState does a FLAG comparison!
		}
		
		Control specialPanel;
		
		Enum IOwnerState.InternalState {
			get {
				return (specialPanel != null) ? SearchResultPanelOwnerState.SpecialPanelMode : SearchResultPanelOwnerState.DefaultMode;
			}
		}
		
		public void ShowSpecialPanel(Control ctl)
		{
			ctl.Dock = DockStyle.Fill;
			if (specialPanel == ctl)
				return;
			if (specialPanel != null)
				myPanel.Controls.Remove(specialPanel);
			else
				myPanel.Controls.Remove(resultTreeView);
			specialPanel = ctl;
			myPanel.Controls.Add(ctl);
			myPanel.Controls.SetChildIndex(ctl, 0);
			ToolbarService.UpdateToolbar(toolStrip);
		}
		
		public void RemoveSpecialPanel()
		{
			if (specialPanel != null) {
				specialPanel = null;
				myPanel.Controls.Remove(specialPanel);
				myPanel.Controls.Add(resultTreeView);
				myPanel.Controls.SetChildIndex(resultTreeView, 0);
				ToolbarService.UpdateToolbar(toolStrip);
			}
		}
	}
	public class BackToNormalView : AbstractMenuCommand
	{
		public override void Run()
		{
			SearchResultPanel.Instance.RemoveSpecialPanel();
		}
	}
}
