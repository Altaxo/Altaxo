// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

namespace ICSharpCode.SharpDevelop.DefaultEditor.Gui.OptionPanels
{
	/// <summary>
	/// General texteditor options panel.
	/// </summary>
	public class GeneralTextEditorPanel : AbstractOptionPanel
	{
		Font selectedFont = new Font("Courier New", 10);
		
		int encoding = Encoding.UTF8.CodePage;
		
		public override void LoadPanelContents()
		{
			SetupFromXml(Path.Combine(PropertyService.DataDirectory, 
			                          @"resources\panels\GeneralTextEditorPanel.xfrm"));
			
			((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("DoubleBuffer", true);
			((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked  = ((IProperties)CustomizationObject).GetProperty("EnableCodeCompletion", true);
			((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked         = ((IProperties)CustomizationObject).GetProperty("EnableFolding", true);
			
			ControlDictionary["fontNameDisplayTextBox"].Text = ((IProperties)CustomizationObject).GetProperty("DefaultFont", selectedFont).ToString();
			
			((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked = ((IProperties)CustomizationObject).GetProperty("UseAntiAliasFont", false);
			
			foreach (String name in CharacterEncodings.Names) {
				((ComboBox)ControlDictionary["textEncodingComboBox"]).Items.Add(name);
			}
			int i = 0;
			try {
				i = CharacterEncodings.GetEncodingIndex((Int32)((IProperties)CustomizationObject).GetProperty("Encoding", encoding));
			} catch {
				i = CharacterEncodings.GetEncodingIndex(encoding);
			}
			((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex = i;
			encoding = CharacterEncodings.GetEncodingByIndex(i).CodePage;
			
			selectedFont = ParseFont(ControlDictionary["fontNameDisplayTextBox"].Text);

			ControlDictionary["browseButton"].Click += new EventHandler(SelectFontEvent);
		}
		
		public override bool StorePanelContents()
		{
			((IProperties)CustomizationObject).SetProperty("DoubleBuffer",         ((CheckBox)ControlDictionary["enableDoublebufferingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("UseAntiAliasFont",     ((CheckBox)ControlDictionary["enableAAFontRenderingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("EnableCodeCompletion", ((CheckBox)ControlDictionary["enableCodeCompletionCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("EnableFolding",        ((CheckBox)ControlDictionary["enableFoldingCheckBox"]).Checked);
			((IProperties)CustomizationObject).SetProperty("DefaultFont",          selectedFont);
			((IProperties)CustomizationObject).SetProperty("Encoding",             CharacterEncodings.GetCodePageByIndex(((ComboBox)ControlDictionary["textEncodingComboBox"]).SelectedIndex));
			return true;
		}
		
		static Font ParseFont(string font)
		{
			string[] descr = font.Split(new char[]{',', '='});
			return new Font(descr[1], Single.Parse(descr[3]));
		}
		
		void SelectFontEvent(object sender, EventArgs e)
		{
			using (FontDialog fdialog = new FontDialog()) {
				fdialog.Font = selectedFont;
				if (fdialog.ShowDialog() == DialogResult.OK) {
					Font newFont  = new Font(fdialog.Font.FontFamily, (float)Math.Round(fdialog.Font.Size));
					ControlDictionary["fontNameDisplayTextBox"].Text = newFont.ToString();
					selectedFont  = newFont;
					((IProperties)CustomizationObject).SetProperty("DefaultFont",          selectedFont);
					
				}
			}
		}
	}
}
