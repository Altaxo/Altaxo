// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels
{
	public class IDEOptionPanel : AbstractOptionPanel
	{
		readonly static string uiLanguageProperty = "CoreProperties.UILanguage";
		ListView listView = new ListView();
		Label newCulture = new Label();
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		
		string SelectedCulture {
			get {
				if (listView.SelectedItems.Count > 0) {
					return listView.SelectedItems[0].SubItems[1].Text;
				}
				return null;
			}
		}
		
		string SelectedCountry {
			get {
				if (listView.SelectedItems.Count > 0) {
					return listView.SelectedItems[0].Text;
				}
				return null;
			}
		}
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (SelectedCulture != null) {
					propertyService.SetProperty(uiLanguageProperty, SelectedCulture);
				}
			}
			return true;
		}
		
		void ChangeCulture(object sender, EventArgs e)
		{
			newCulture.Text = resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.UILanguageSetToLabel") + " " + SelectedCountry;
		}
		
		string GetCulture(string languageCode)
		{
			LanguageService languageService = (LanguageService)ServiceManager.Services.GetService(typeof(LanguageService));
			foreach (Language language in languageService.Languages) {
				if (languageCode.StartsWith(language.Code)) {
					return language.Name;
				}
			}
			return "English";
		}
		
		public IDEOptionPanel()// : base(panelName)
		{
			LanguageService languageService = (LanguageService)ServiceManager.Services.GetService(typeof(LanguageService));
			listView.Location = new Point(8, 8);
			listView.Size     = new Size(136, 200);
			listView.LargeImageList = languageService.LanguageImageList;
			listView.ItemActivate += new EventHandler(ChangeCulture);
			listView.Sorting = SortOrder.Ascending;
			listView.Activation = ItemActivation.OneClick;
			listView.Anchor = (System.Windows.Forms.AnchorStyles.Top | 
			                  (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
			
			foreach (Language language in languageService.Languages) {
				listView.Items.Add(new ListViewItem(new string[] {language.Name, language.Code}, language.ImageIndex));
			}			
			
			this.Controls.Add(listView);
			
			Label culture = new Label();
			culture.Location  = new Point(8, 220);
			culture.Size      = new Size(350, 16);
			culture.Anchor = (System.Windows.Forms.AnchorStyles.Top | 
			                  (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
						
			Dock = DockStyle.Fill;
			
			culture.Text = resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.CurrentUILanguageLabel") + " " + GetCulture(propertyService.GetProperty(uiLanguageProperty, "en"));
			this.Controls.Add(culture);
			
			Label descr = new Label();
			descr.Location  = new Point(8, 280);
			descr.Size      = new Size(390, 50);
			descr.Text      = resourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.DescriptionText");
			descr.Anchor = (System.Windows.Forms.AnchorStyles.Top | 
			                  (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
			
			this.Controls.Add(descr);
			
			newCulture.Location  = new Point(8, 240);
			newCulture.Size      = new Size(360, 50);
			newCulture.Anchor = (System.Windows.Forms.AnchorStyles.Top | 
			                  (System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right));
			this.Controls.Add(newCulture);
		}
	}
}
