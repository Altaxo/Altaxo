// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;

using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	/// <summary>
	/// This class represents the default folder in the project browser.
	/// NOTE: For a 'directory' folder use the DirectoryNode class
	/// </summary>
	public class FolderNode : AbstractBrowserNode 
	{
		Image closedImage = null;
		Image openedImage = null;
		
		public Image ClosedImage {
			get {
				return closedImage;
			}
			set {
				closedImage = value;
				if (!IsExpanded) {
					IconImage = closedImage;
				}
			}
		}
		
		public Image OpenedImage {
			get {
				return openedImage;
			}
			set {
				openedImage = value;
				if (IsExpanded) {
					IconImage = openedImage;
				}
			}
		}
		
		public FolderNode(string nodeName)
		{
			Text           = nodeName;
			canLabelEdited = false;
		}
		
		public override void BeforeExpand()
		{
			if (openedImage != null) {
				IconImage = openedImage;
			}
		}
		
		public override void BeforeCollapse()
		{
			if (closedImage != null) {
				IconImage = closedImage;
			}
		}
	}
}
