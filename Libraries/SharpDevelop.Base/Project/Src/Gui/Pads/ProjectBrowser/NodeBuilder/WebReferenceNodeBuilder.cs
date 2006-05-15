// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 955 $</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Project
{
	public class WebReferenceNodeBuilder
	{
		WebReferenceNodeBuilder()
		{
		}
		
		public static TreeNode AddWebReferencesFolderNode(ProjectNode projectNode, WebReference webReference)
		{
			if (webReference.WebReferencesProjectItem != null) {
				WebReferencesFolderNode webReferencesFolderNode = new WebReferencesFolderNode(webReference.WebReferencesProjectItem);
				webReferencesFolderNode.FileNodeStatus = FileNodeStatus.InProject;
				webReferencesFolderNode.AddTo(projectNode);
				return webReferencesFolderNode;
			}	
			return null;
		}
		
		public static TreeNode AddWebReference(WebReferencesFolderNode webReferencesFolderNode, WebReference webReference)
		{
			WebReferenceNode node = new WebReferenceNode(webReference);
			node.FileNodeStatus = FileNodeStatus.InProject;
			node.AddTo(webReferencesFolderNode);
			return node;
		}
	}
}
