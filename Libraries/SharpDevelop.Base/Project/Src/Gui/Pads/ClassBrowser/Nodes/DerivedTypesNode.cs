﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Refactoring;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class DerivedTypesNode : ExtFolderNode
	{
		IProject project;
		IClass   c;
		
		public override bool Visible {
			get {
				ClassBrowserFilter filter = ClassBrowserPad.Instance.Filter;
				return (filter & ClassBrowserFilter.ShowBaseAndDerivedTypes) != 0;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public DerivedTypesNode(IProject project, IClass c)
		{
			sortOrder = 1;
			
			this.project = project;
			this.c       = c;
			Text         = ResourceService.GetString("MainWindow.Windows.ClassBrowser.DerivedTypes");
			
			OpenedIcon = "ProjectBrowser.Folder.Open";
			ClosedIcon = "ProjectBrowser.Folder.Closed";
			
			Nodes.Add(new TreeNode(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode")));
		}
		
		protected override void Initialize()
		{
			base.Initialize();
			Nodes.Clear();
			
			List<IProjectContent> contentList = new List<IProjectContent>(1);
			contentList.Add(null);
			if (ProjectService.OpenSolution != null) {
				foreach (IProject project in ProjectService.OpenSolution.Projects) {
					IProjectContent projectContent = ParserService.GetProjectContent(project);
					if (projectContent != null) {
						contentList[0] = projectContent;
						foreach (IClass derivedClass in RefactoringService.FindDerivedClasses(c, contentList, true)) {
							new ClassNode(project, derivedClass).AddTo(this);
						}
					}
				}
			}
			
			if (Nodes.Count == 0) {
				SetIcon(ClosedIcon);
				OpenedIcon = ClosedIcon = null;
			}
		}
	}
}
