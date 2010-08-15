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
	public interface IClassNodeBuilder
	{
		bool CanBuildClassTree(IClass c);
		TreeNode AddClassNode(ExtTreeView classBrowser, IProject project, IClass c);
	}
}
