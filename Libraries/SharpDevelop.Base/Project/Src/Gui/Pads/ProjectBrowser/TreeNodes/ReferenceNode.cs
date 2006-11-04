﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Diagnostics;
using System.IO;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceNode : AbstractProjectBrowserTreeNode
	{
		ReferenceProjectItem referenceProjectItem;
		
		public ReferenceProjectItem ReferenceProjectItem {
			get {
				return ReferenceProjectItem;
			}
		}
		
		public ReferenceNode(ReferenceProjectItem referenceProjectItem)
		{
			this.referenceProjectItem = referenceProjectItem;
			Tag = referenceProjectItem;

			ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/ReferenceNode";
			SetIcon("Icons.16x16.Reference");
			if (referenceProjectItem.ItemType == ItemType.ProjectReference) {
				Text = Path.GetFileNameWithoutExtension(referenceProjectItem.Include);
			} else {
				Text = referenceProjectItem.Name;
			}
		}
		
		#region Cut & Paste
		public override bool EnableDelete {
			get {
				return true;
			}
		}
	
		public override void Delete()
		{
			IProject project = Project;
			ProjectService.RemoveProjectItem(referenceProjectItem.Project, referenceProjectItem);
			Debug.Assert(Parent != null);
			Debug.Assert(Parent is ReferenceFolder);
			((ReferenceFolder)Parent).ShowReferences();
			project.Save();
		}
		#endregion
		
		public override object AcceptVisitor(ProjectBrowserTreeNodeVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
