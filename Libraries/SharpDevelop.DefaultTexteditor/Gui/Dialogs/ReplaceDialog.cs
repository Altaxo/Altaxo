// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.DefaultEditor.Gui.Editor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.XmlForms;
using ICSharpCode.SharpDevelop.Gui.XmlForms;
using ICSharpCode.TextEditor;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class ReplaceDialog : BaseSharpDevelopForm
	{
		bool replaceMode;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public ReplaceDialog(bool replaceMode)
		{
			this.replaceMode = replaceMode;
			if (replaceMode) {
				this.SetupFromXml(Path.Combine(propertyService.DataDirectory, @"resources\dialogs\ReplaceDialog.xfrm"));
				ControlDictionary["replaceHelpButton"].Enabled = false;
			} else {
				this.SetupFromXml(Path.Combine(propertyService.DataDirectory, @"resources\dialogs\FindDialog.xfrm"));
			}
			
			ControlDictionary["findHelpButton"].Enabled = false;
			AcceptButton = (Button)ControlDictionary["findButton"];
			CancelButton = (Button)ControlDictionary["closeButton"];
			
			((CheckBox)ControlDictionary["ignoreCaseCheckBox"]).Checked          = !SearchReplaceManager.SearchOptions.IgnoreCase;
			((CheckBox)ControlDictionary["searchWholeWordOnlyCheckBox"]).Checked = SearchReplaceManager.SearchOptions.SearchWholeWordOnly;
			
			((CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"]).Checked  = SearchReplaceManager.SearchOptions.SearchStrategyType != SearchStrategyType.Normal;
			((CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"]).CheckedChanged += new EventHandler(SpecialSearchStrategyCheckBoxChangedEvent);
			
			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).Items.Add("Wildcards");
			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).Items.Add(resourceService.GetString("Dialog.NewProject.SearchReplace.SearchStrategy.RegexSearch"));
			int index = 0;
			switch (SearchReplaceManager.SearchOptions.SearchStrategyType) {
				case SearchStrategyType.Normal:
				case SearchStrategyType.Wildcard:
					break;
				case SearchStrategyType.RegEx:
					index = 1;
					break;
			}
 			((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).SelectedIndex = index;
			
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add(resourceService.GetString("Global.Location.currentfile"));
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add(resourceService.GetString("Global.Location.allopenfiles"));
			((ComboBox)ControlDictionary["searchLocationComboBox"]).Items.Add(resourceService.GetString("Global.Location.wholeproject"));
			
			index = 0;
			switch (SearchReplaceManager.SearchOptions.DocumentIteratorType) {
				case DocumentIteratorType.AllOpenFiles:
					index = 1;
					break;
				case DocumentIteratorType.WholeCombine:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
					break;
			}
			((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndex = index;
			
			ControlDictionary["searchPatternComboBox"].Text  = SearchReplaceManager.SearchOptions.SearchPattern;
			
			// insert event handlers
			ControlDictionary["findButton"].Click  += new EventHandler(FindNextEvent);
			ControlDictionary["closeButton"].Click += new EventHandler(CloseDialogEvent);
			
			if (replaceMode) {
				this.Text = resourceService.GetString("Dialog.NewProject.SearchReplace.ReplaceDialogName");
				ControlDictionary["replaceButton"].Click    += new EventHandler(ReplaceEvent);
				ControlDictionary["replaceAllButton"].Click += new EventHandler(ReplaceAllEvent);
				ControlDictionary["replacePatternComboBox"].Text = SearchReplaceManager.SearchOptions.ReplacePattern;
			} else {
				this.Text = resourceService.GetString("Dialog.NewProject.SearchReplace.FindDialogName");
				ControlDictionary["markAllButton"].Click    += new EventHandler(MarkAllEvent);
			}
			/*
				ControlDictionary["replacePatternComboBox"].Visible = false;
				ControlDictionary["replaceAllButton"].Visible       = false;
				ControlDictionary["replacePatternLabel"].Visible    = false;
				ControlDictionary["replacePatternButton"].Visible   = false;
				ControlDictionary["replaceButton"].Text             = resourceService.GetString("Dialog.NewProject.SearchReplace.ToggleReplaceModeButton");
				ClientSize = new Size(ClientSize.Width, ClientSize.Height - 32);
			*/
			SpecialSearchStrategyCheckBoxChangedEvent(null, null);
			SearchReplaceManager.ReplaceDialog     = this;
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			SearchReplaceManager.ReplaceDialog     = null;
		}
		
		public void SetSearchPattern(string pattern)
		{
			ControlDictionary["searchPatternComboBox"].Text  = pattern;
		}
		
		void SetupSearchReplaceManager()
		{
			SearchReplaceManager.SearchOptions.SearchPattern  = ControlDictionary["searchPatternComboBox"].Text;
			if (replaceMode) {
				SearchReplaceManager.SearchOptions.ReplacePattern = ControlDictionary["replacePatternComboBox"].Text;
			}
			
			SearchReplaceManager.SearchOptions.IgnoreCase          = !((CheckBox)ControlDictionary["ignoreCaseCheckBox"]).Checked;
			SearchReplaceManager.SearchOptions.SearchWholeWordOnly = ((CheckBox)ControlDictionary["searchWholeWordOnlyCheckBox"]).Checked;
			
			if (((CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"]).Checked) {
				switch (((ComboBox)ControlDictionary["specialSearchStrategyComboBox"]).SelectedIndex) {
					case 0:
						SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.Wildcard;
						break;
					case 1:
						SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.RegEx;
						break;
				}
			} else {
				SearchReplaceManager.SearchOptions.SearchStrategyType = SearchStrategyType.Normal;
			}
			
			switch (((ComboBox)ControlDictionary["searchLocationComboBox"]).SelectedIndex) {
				case 0:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.CurrentDocument;
					break;
				case 1:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.AllOpenFiles;
					break;
				case 2:
					SearchReplaceManager.SearchOptions.DocumentIteratorType = DocumentIteratorType.WholeCombine;
					break;
			}
		}
		
		void FindNextEvent(object sender, EventArgs e)
		{
			if (ControlDictionary["searchPatternComboBox"].Text.Length == 0) {
				return;
			}
			
			try {
				Cursor = Cursors.WaitCursor;
				SetupSearchReplaceManager();
				SearchReplaceManager.FindNext();
				this.Focus();
			}
			finally {
				Cursor = Cursors.Default;
			}
		}
		
		void ReplaceEvent(object sender, EventArgs e)
		{
			if (ControlDictionary["searchPatternComboBox"].Text.Length == 0) {
				return;
			}
			
			try {
				Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.Replace();
			}
			finally {
				Cursor = Cursors.Default;
			}
		}
		
		void ReplaceAllEvent(object sender, EventArgs e)
		{
			if (ControlDictionary["searchPatternComboBox"].Text.Length == 0) {
				return;
			}
			
			try {
				Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.ReplaceAll();
			} finally {
				Cursor = Cursors.Default;
			}
		}
		
		void MarkAllEvent(object sender, EventArgs e)
		{
			if (ControlDictionary["searchPatternComboBox"].Text.Length == 0) {
				return;
			}
			
			try {
				Cursor = Cursors.WaitCursor;
				
				SetupSearchReplaceManager();
				SearchReplaceManager.MarkAll();			
			} finally {
				Cursor = Cursors.Default;
			}
		}
		
		void CloseDialogEvent(object sender, EventArgs e)
		{
			Close();
		}
		
		void SpecialSearchStrategyCheckBoxChangedEvent(object sender, EventArgs e)
		{
			CheckBox cb = (CheckBox)ControlDictionary["useSpecialSearchStrategyCheckBox"];
			if (cb != null) {
				ControlDictionary["specialSearchStrategyComboBox"].Enabled = cb.Checked;
			}
		}
	}
}
