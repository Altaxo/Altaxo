// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.OptionPanels.CompletionDatabaseWizard
{
	public class CreateDatabasePanel : AbstractWizardPanel, IProgressMonitor
	{
		IProperties properties;
		Thread      generateThread = null;
		int         totalWork = 0;
		static FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public override bool ReceiveDialogMessage(DialogMessage message)
		{
	    	return true;
		}
		
		void SetValues(object sender, EventArgs e)
		{
			properties = (IProperties)CustomizationObject;
		}
		
		void StartCreation(object sender, EventArgs e)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (generateThread != null) {
				try {
					generateThread.Abort();
				} catch (Exception) {}
				SetProgressBarValue(0);
				ControlDictionary["createButton"].Text = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.StartCreationButton");
				EnableCancel = EnablePrevious = true;
				ControlDictionary["fastCreationCheckBox"].Enabled = true;
				generateThread = null;
			} else {
				EnableCancel = EnablePrevious = false;
				ControlDictionary["fastCreationCheckBox"].Enabled = false;
				generateThread = new Thread(new ThreadStart(CreateDatabase));
				generateThread.Priority = ThreadPriority.Lowest;
				generateThread.Start();
				
				ControlDictionary["createButton"].Text = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.CancelCreationButton");
			}
		}
		
		void CreateDatabase()
		{
			try {
				DefaultParserService parserService  = (DefaultParserService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DefaultParserService));
				string path  = properties.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty);
				if (((CheckBox)ControlDictionary["fastCreationCheckBox"]).Checked) {
					parserService.GenerateCodeCompletionDatabaseFast(path, this);
				} else {
					parserService.GenerateEfficientCodeCompletionDatabase(path, this);
				}
			} catch (ThreadAbortException) {
				// do nothing, thread stopped
			} catch (Exception e) {
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				MessageBox.Show(resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.CreateDbErrorMessage") + "\n" + e.ToString(), 
				                resourceService.GetString("Global.ErrorText"), 
				                MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
			}
		}
		
		static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		public CreateDatabasePanel() : base(propertyService.DataDirectory + @"\resources\panels\CompletionDatabaseWizard\ViewProgressPanel.xfrm")
		{
			NextWizardPanelID = "CreationSuccessful";
			
			EnableFinish      = false;
			EnableNext        = false;
			CustomizationObjectChanged += new EventHandler(SetValues);
			
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			((TextBox)ControlDictionary["textBox"]).Lines = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.PanelDescription").Split('\n');
			
			ControlDictionary["createButton"].Click += new EventHandler(StartCreation);
		}
		
		void SetButtonFinish(int val)
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			ControlDictionary["createButton"].Text = resourceService.GetString("Dialog.Wizards.CodeCompletionDatabaseWizard.CreateDatabasePanel.FinishedCreationButton");
			ControlDictionary["createButton"].Enabled = false;
			ControlDictionary["progressBar"].Enabled = false;
			EnableCancel = EnablePrevious = false;
			EnableFinish = true;
			FinishPanel();
		}
		
		void SetProgressBarLimit(int val)
		{
			((ProgressBar)ControlDictionary["progressBar"]).Maximum = val;
		}
		void SetProgressBarValue(int val)
		{
			((ProgressBar)ControlDictionary["progressBar"]).Value = val;
		}
		
		delegate void SetValue(int val);
		
		public void BeginTask(string name, int totalWork)
		{
			((ProgressBar)ControlDictionary["progressBar"]).Invoke(new SetValue(SetProgressBarLimit), new object[] { totalWork });
		}
		
		
		public void Worked(int work)
		{
			((ProgressBar)ControlDictionary["progressBar"]).Invoke(new SetValue(SetProgressBarValue), new object[] { work });
		}
		
		public void Done()
		{
			SetProgressBarValue(totalWork);
			ControlDictionary["createButton"].Invoke(new SetValue(SetButtonFinish), new object[] { totalWork});
		}
		
		public bool Canceled {
			get {
				return false;
			}
			set {
			}
		}
		
		public string TaskName {
			get {
				return String.Empty;
			}
			set {
			}
		}
	}
}
