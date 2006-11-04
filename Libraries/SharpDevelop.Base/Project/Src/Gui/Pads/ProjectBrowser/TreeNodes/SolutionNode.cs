﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1576 $</version>
// </file>

using System;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public class SolutionNode : AbstractProjectBrowserTreeNode, ISolutionFolderNode
	{
		Solution solution;
		public ISolutionFolder Folder {
			get {
				return solution;
			}
		}
		
		public override Solution Solution {
			get {
				return solution;
			}
		}
		public ISolutionFolderContainer Container {
			get {
				return solution;
			}
		}
		
		public SolutionNode(Solution solution)
		{
			sortOrder = -1;
			this.solution = solution;
			UpdateText();;
			autoClearNodes = false;
			canLabelEdit = true;
			
			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/SolutionNode";
			
			SetIcon("ProjectBrowser.Solution");
			Tag = solution;
		}
		
		public override void BeforeLabelEdit()
		{
			Text = solution.Name;
		}
		
		public override void AfterLabelEdit(string newName)
		{
			try {
				if (solution.Name == newName)
					return;
				if (!FileService.CheckFileName(newName))
					return;
				string newFileName = Path.Combine(solution.Directory, newName + ".sln");
				if (!FileService.RenameFile(solution.FileName, newFileName, false)) {
					return;
				}
				solution.FileName = newFileName;
				solution.Name = newName;
			} finally {
				UpdateText();
			}
		}
		
		void UpdateText()
		{
			Text = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.SolutionNodeText") + " " + solution.Name;
		}
		
		public void AddItem(string fileName)
		{
			string folderName = ResourceService.GetString("ICSharpCode.SharpDevelop.Commands.ProjectBrowser.SolutionItemsNodeText");
			SolutionFolderNode node = null;
			foreach (TreeNode n in Nodes) {
				node = n as SolutionFolderNode;
				if (node != null && node.Folder.Name == folderName) {
					break;
				}
				node = null;
			}
			if (node == null) {
				SolutionFolder newSolutionFolder = solution.CreateFolder(folderName);
				solution.AddFolder(newSolutionFolder);
				solution.Save();
				
				node = new SolutionFolderNode(solution, newSolutionFolder);
				node.AddTo(this);
			}
			node.AddItem(fileName);
		}
		
		#region Drag & Drop
		public override DragDropEffects GetDragDropEffect(IDataObject dataObject, DragDropEffects proposedEffect)
		{
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				
				if (folderNode.Folder.Parent != solution) {
					return DragDropEffects.All;
				}
			}
			
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				
				if (projectNode.Parent != this) {
					return DragDropEffects.Move;
				}
			}
			return DragDropEffects.None;
			
		}
		
		public override void DoDragDrop(IDataObject dataObject, DragDropEffects effect)
		{
			AbstractProjectBrowserTreeNode parentNode = null;
			
			if (dataObject.GetDataPresent(typeof(SolutionFolderNode))) {
				SolutionFolderNode folderNode = (SolutionFolderNode)dataObject.GetData(typeof(SolutionFolderNode));
				parentNode = folderNode.Parent as AbstractProjectBrowserTreeNode;
				
				folderNode.Remove();
				folderNode.AddTo(this);
				
				this.solution.AddFolder(folderNode.Folder);
			}
			if (dataObject.GetDataPresent(typeof(ProjectNode))) {
				ProjectNode projectNode = (ProjectNode)dataObject.GetData(typeof(ProjectNode));
				parentNode = projectNode.Parent as AbstractProjectBrowserTreeNode;
				
				projectNode.Remove();
				projectNode.AddTo(this);
				projectNode.EnsureVisible();
				this.solution.AddFolder(projectNode.Project);
			}
			
			if (parentNode != null) {
				parentNode.Refresh();
			}
			
			
			solution.Save();
		}
		#endregion
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		#region Cut&Paste
		public override bool EnablePaste {
			get {
				return SolutionFolderNode.DoEnablePaste(this);
			}
		}
		
		public override void Paste()
		{
			SolutionFolderNode.DoPaste(this);
		}
		#endregion
	}
}
