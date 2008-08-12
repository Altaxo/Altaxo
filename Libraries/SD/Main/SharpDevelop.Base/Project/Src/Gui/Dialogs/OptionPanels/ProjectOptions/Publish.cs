// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class Publish : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.Publish.xfrm");
			InitializeHelper();
		}
	}
}
