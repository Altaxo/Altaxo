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
	public class DefaultClassNodeBuilder : IClassNodeBuilder
	{
		public bool CanBuildClassTree(IClass c)
		{
			return true;
		}

		public TreeNode AddClassNode(ExtTreeView classBrowser, IProject project, IClass c)
		{
			ClassNode cNode = new ClassNode(project, c);
			cNode.AddTo(classBrowser);
			return cNode;
		}
	}
}
