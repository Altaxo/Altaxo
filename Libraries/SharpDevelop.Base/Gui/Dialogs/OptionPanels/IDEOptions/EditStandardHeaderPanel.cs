// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class EditStandardHeaderPanel : AbstractOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\EditStandardHeaderPanel.xfrm"));
			ControlDictionary["headerTextBox"].Font = new Font("Courier New", 10);
			foreach (StandardHeader header in StandardHeader.StandardHeaders) {
				((ComboBox)ControlDictionary["headerChooser"]).Items.Add(header);
			}
			((ComboBox)ControlDictionary["headerChooser"]).SelectedIndexChanged += new EventHandler(SelectedIndexChanged);
			((ComboBox)ControlDictionary["headerChooser"]).SelectedIndex = 0;
			((RichTextBox)ControlDictionary["headerTextBox"]).TextChanged += new EventHandler(TextChangedEvent);
		}
		void TextChangedEvent(object sender , EventArgs e)
		{
			((StandardHeader)((ComboBox)ControlDictionary["headerChooser"]).SelectedItem).Header = ControlDictionary["headerTextBox"].Text;
		}
		void SelectedIndexChanged(object sender , EventArgs e)
		{
			((RichTextBox)ControlDictionary["headerTextBox"]).TextChanged -= new EventHandler(TextChangedEvent);
			int idx =((ComboBox)ControlDictionary["headerChooser"]).SelectedIndex;
			if (idx >= 0) {
				ControlDictionary["headerTextBox"].Text = ((StandardHeader)((ComboBox)ControlDictionary["headerChooser"]).SelectedItem).Header;
				ControlDictionary["headerTextBox"].Enabled = true;
			} else {
				ControlDictionary["headerTextBox"].Text = "";
				ControlDictionary["headerTextBox"].Enabled = false;
			}
			((RichTextBox)ControlDictionary["headerTextBox"]).TextChanged += new EventHandler(TextChangedEvent);
		}
		
		public override bool StorePanelContents()
		{
			StandardHeader.StoreHeaders();
			return true;
		}
	}
}
