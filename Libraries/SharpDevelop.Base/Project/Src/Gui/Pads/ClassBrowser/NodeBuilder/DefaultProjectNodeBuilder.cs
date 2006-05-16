using System;
using System.IO;
using ICSharpCode.Core;
using System.Windows.Forms;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.ClassBrowser
{
	public class DefaultProjectNodeBuilder : IProjectNodeBuilder
	{
		public bool CanBuildProjectTree(IProject project)
		{
			return true;
		}

		public TreeNode AddProjectNode(ExtTreeView classBrowser, IProject project)
		{
			ProjectNode prjNode = new ProjectNode(project);
			prjNode.AddTo(classBrowser);
			return prjNode;
		}
	}
}
