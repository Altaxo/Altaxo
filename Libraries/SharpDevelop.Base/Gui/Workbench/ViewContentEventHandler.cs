/*
 * Created by SharpDevelop.
 * User: Omnibrain
 * Date: 30.08.2004
 * Time: 13:47
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Services;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui
{
	public delegate void ViewContentEventHandler(object sender, ViewContentEventArgs e);
		
	public class ViewContentEventArgs : System.EventArgs
	{
		IViewContent content;
		
		public IViewContent Content {
			get {
				return content;
			}
			set {
				content = value;
			}
		}
		
		public ViewContentEventArgs(IViewContent content)
		{
			this.content = content;
		}
	}
}
