//
// SharpDevelop
//
// Copyright (C) 2004 Matthew Ward
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// Matthew Ward (mrward@users.sourceforge.net)

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Text;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;


namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	/// <summary>
	/// The Output Window options panel.
	/// </summary>
	public class OutputWindowOptionsPanel : AbstractOptionPanel
	{
		public static readonly string OutputWindowsProperty = "SharpDevelop.UI.OutputWindowOptions";
		FontSelectionPanel fontSelectionPanel;
		
		public OutputWindowOptionsPanel()
		{
		}
		
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory,
			                          @"resources\panels\OutputWindowOptionsPanel.xfrm"));
			
			IProperties properties = (IProperties)PropertyService.GetProperty(OutputWindowsProperty, new DefaultProperties());
			fontSelectionPanel = new FontSelectionPanel();
			fontSelectionPanel.Dock = DockStyle.Fill;
			ControlDictionary["FontGroupBox"].Controls.Add(fontSelectionPanel);
			((CheckBox)ControlDictionary["wordWrapCheckBox"]).Checked = properties.GetProperty("WordWrap", true);
			
			fontSelectionPanel.CurrentFontString = properties.GetProperty("DefaultFont", new Font("Courier New", 10).ToString()).ToString();
		}
		
		public override bool StorePanelContents()
		{
			IProperties properties = (IProperties)PropertyService.GetProperty(OutputWindowsProperty, new DefaultProperties());
			properties.SetProperty("WordWrap", ((CheckBox)ControlDictionary["wordWrapCheckBox"]).Checked);
			properties.SetProperty("DefaultFont", fontSelectionPanel.CurrentFontString);
			
			PropertyService.SetProperty(OutputWindowsProperty, properties);
			return true;
		}
		
		
		
	}
}
