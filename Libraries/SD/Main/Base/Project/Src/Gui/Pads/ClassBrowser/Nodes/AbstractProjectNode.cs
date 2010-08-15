﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Robert Zaunere"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public abstract class AbstractProjectNode : ExtTreeNode
	{
		IProject project;

		public IProject Project {
			get {
				return project;
			}
		}

		protected AbstractProjectNode()
		{
			this.project = null;
		}

		public AbstractProjectNode(IProject project)
		{
			this.project = project;
		}

		public abstract void UpdateParseInformation(ICompilationUnit oldUnit, ICompilationUnit unit);
		public abstract TreeNode GetNodeByPath(string directory, bool create);
	}
}
