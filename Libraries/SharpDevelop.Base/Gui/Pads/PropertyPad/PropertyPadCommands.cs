// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class PropertyPadResetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			PropertyPad.Grid.ResetSelectedProperty();
		}
	}
	
	public class PropertyPadShowDescriptionCommand : AbstractCheckableMenuCommand
	{
		public override bool IsChecked {
			get {
				return PropertyPad.Grid.HelpVisible;
			}
			set {
				PropertyPad.Grid.HelpVisible = value;
			}
		}
		
		public override void Run()
		{
			PropertyPad.Grid.HelpVisible = !PropertyPad.Grid.HelpVisible;
		}
	}
	
}
