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

using ICSharpCode.Core.AddIns;

using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	/// <summary>
	/// This class reperesents the base class for all nodes in the
	/// class browser.
	/// </summary>
	public class AbstractClassScoutNode : TreeNode
	{
		protected string contextmenuAddinTreePath = String.Empty;
		
		/// <summary>
		/// Gets the add-in tree path for the context menu. 
		/// </summary>
		/// <remarks>
		/// I choosed to give back the add-in tree path instead of a popup menu 
		/// or a menuitem collection, because I don't want to add a magic library 
		/// or Windows.Forms dependency.
		/// </remarks>
		public virtual string ContextmenuAddinTreePath {
			get {
				return contextmenuAddinTreePath;
			}
			set {
				contextmenuAddinTreePath = value;
			}
		}
		
		public AbstractClassScoutNode(string name) : base(name)
		{
		}
	}
}
