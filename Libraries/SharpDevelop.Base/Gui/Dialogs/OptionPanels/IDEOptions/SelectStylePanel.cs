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

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class SelectStylePanel : AbstractOptionPanel
	{
		CheckBox showExtensionsCheckBox = new CheckBox();
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\SelectStylePanel.xfrm"));
			
			((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked  = PropertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", true);
			
			IAddInTreeNode treeNode = AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Ambiences");
			foreach (IAddInTreeNode childNode in treeNode.ChildNodes.Values) {
				((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Items.Add(childNode.Codon.ID);
			}			
			
			((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text = PropertyService.GetProperty("SharpDevelop.UI.CurrentAmbience", "CSharp");
		}
		
		public override bool StorePanelContents()
		{
			PropertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.ProjectBrowser.ShowExtensions", ((CheckBox)ControlDictionary["showExtensionsCheckBox"]).Checked);
			PropertyService.SetProperty("SharpDevelop.UI.CurrentAmbience", ((ComboBox)ControlDictionary["selectAmbienceComboBox"]).Text);
			return true;
		}
	}
}
