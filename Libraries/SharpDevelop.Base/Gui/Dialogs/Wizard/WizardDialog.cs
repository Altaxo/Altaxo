// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// TreeView options are used, when more options will be edited (for something like
	/// IDE Options + Plugin Options)
	/// </summary>
	public class WizardDialog : System.Windows.Forms.Form
	{
		StatusPanel       statusPanel  = null;
		CurrentPanelPanel curPanel     = null;
		
		Panel             dialogPanel  = new Panel();
		
		/// <remarks>
		/// On this stack the indices of the previous active wizard panels. This
		/// is used to restore the path the user gone. (for the back button)
		/// </remarks>
		Stack             idStack      = new Stack();
		
		ArrayList         wizardPanels = new ArrayList();
		int               activePanelNumber  = 0;
		
		EventHandler enableNextChangedHandler;
		EventHandler enableCancelChangedHandler;
		EventHandler nextWizardPanelIDChangedHandler;
		EventHandler finishPanelHandler;
		
		public ArrayList WizardPanels {
			get {
				return wizardPanels;
			}
		}
		
		public int ActivePanelNumber {
			get {
				return activePanelNumber;
			}
		}
		
		public IWizardPanel CurrentWizardPane {
			get {
				return (IWizardPanel)((IDialogPanelDescriptor)wizardPanels[activePanelNumber]).DialogPanel;
			}
		}
		
		int GetPanelNumber(string id)
		{
			for (int i = 0; i < wizardPanels.Count; ++i) {
				IDialogPanelDescriptor descriptor = (IDialogPanelDescriptor)wizardPanels[i];
				if (descriptor.ID == id) {
					return i;
				}
			}
			return -1;
		}
		
		public int GetSuccessorNumber(int curNr)
		{
			IWizardPanel panel = (IWizardPanel)((IDialogPanelDescriptor)wizardPanels[curNr]).DialogPanel;
			
			if (panel.IsLastPanel) {
				return wizardPanels.Count + 1;
			}
			
			int nextID = GetPanelNumber(panel.NextWizardPanelID);
			if (nextID < 0) {
				return curNr + 1;
			}
			return nextID;
		}
		
		/// <value> returns true, if all dialog panels could be finished</value?>
		bool CanFinish {
			get {
				int currentNr = 0;
				while (currentNr < wizardPanels.Count) {
					IDialogPanelDescriptor descriptor = (IDialogPanelDescriptor)wizardPanels[currentNr];
					if (!descriptor.DialogPanel.EnableFinish) {
						return false;
					}
					currentNr = GetSuccessorNumber(currentNr);
				}
				return true;
			}
		}
		
		Label label1        = new Label();
		
		Button backButton   = new Button();
		Button nextButton   = new Button();
		Button finishButton = new Button();
		Button cancelButton = new Button();
		Button helpButton   = new Button();
		
		void CheckFinishedState(object sender, EventArgs e)
		{
			finishButton.Enabled = CanFinish;
		}
		
		void AddNodes(object customizer, ArrayList dialogPanelDescriptors)
		{
			foreach (IDialogPanelDescriptor descriptor in dialogPanelDescriptors) {
				
				if (descriptor.DialogPanel != null) { // may be null, if it is only a "path"
				descriptor.DialogPanel.EnableFinishChanged += new EventHandler(CheckFinishedState);
					descriptor.DialogPanel.CustomizationObject    = customizer;
					wizardPanels.Add(descriptor);
				}
				
				if (descriptor.DialogPanelDescriptors != null) {
					AddNodes(customizer, descriptor.DialogPanelDescriptors);
				}
			}
		}
		
		void EnableCancelChanged(object sender, EventArgs e)
		{
			cancelButton.Enabled = CurrentWizardPane.EnableCancel;
		}
		
		void EnableNextChanged(object sender, EventArgs e)
		{
			nextButton.Enabled = CurrentWizardPane.EnableNext && GetSuccessorNumber(activePanelNumber) < wizardPanels.Count;
			backButton.Enabled = CurrentWizardPane.EnablePrevious && idStack.Count > 0;
		}
		
		void NextWizardPanelIDChanged(object sender, EventArgs e)
		{
			EnableNextChanged(null, null);
			finishButton.Enabled = CanFinish;
			statusPanel.Refresh();
		}
		
		void ActivatePanel(int number)
		{
			// take out old event handlers
			if (CurrentWizardPane != null) {
				CurrentWizardPane.EnableNextChanged        -= enableNextChangedHandler;
				CurrentWizardPane.EnableCancelChanged      -= enableCancelChangedHandler;
				CurrentWizardPane.EnablePreviousChanged    -= enableNextChangedHandler;
				CurrentWizardPane.NextWizardPanelIDChanged -= nextWizardPanelIDChangedHandler;
				CurrentWizardPane.IsLastPanelChanged       -= nextWizardPanelIDChangedHandler;
				CurrentWizardPane.FinishPanelRequested     -= finishPanelHandler;
				
			}
			
			// set new active panel
			activePanelNumber = number;
			
			// insert new event handlers
			if (CurrentWizardPane != null) {
				CurrentWizardPane.EnableNextChanged        += enableNextChangedHandler;
				CurrentWizardPane.EnableCancelChanged      += enableCancelChangedHandler;
				CurrentWizardPane.EnablePreviousChanged    += enableNextChangedHandler;
				CurrentWizardPane.NextWizardPanelIDChanged += nextWizardPanelIDChangedHandler;
				CurrentWizardPane.IsLastPanelChanged       += nextWizardPanelIDChangedHandler;
				CurrentWizardPane.FinishPanelRequested     += finishPanelHandler;
			}
			
			// initialize panel status
			EnableNextChanged(null, null);
			NextWizardPanelIDChanged(null, null);
			EnableCancelChanged(null, null);
			
			// take out panel control & show new one
			statusPanel.Refresh();
			curPanel.Refresh();
			dialogPanel.Controls.Clear();
			
			Control panelControl = CurrentWizardPane.Control;
			panelControl.Dock    = DockStyle.Fill;
			dialogPanel.Controls.Add(panelControl);
			
		}
		
		public WizardDialog(string title, object customizer, string treePath)
		{
			IAddInTreeNode node = AddInTreeSingleton.AddInTree.GetTreeNode(treePath);
			this.Text = title;
			
			if (node != null) {
				AddNodes(customizer, node.BuildChildItems(this));
			}
			InitializeComponents();
			
			enableNextChangedHandler        = new EventHandler(EnableNextChanged);
			nextWizardPanelIDChangedHandler = new EventHandler(NextWizardPanelIDChanged);
			enableCancelChangedHandler      = new EventHandler(EnableCancelChanged);
			finishPanelHandler              = new EventHandler(FinishPanelEvent);
			ActivatePanel(0);
		}
		
		void FinishPanelEvent(object sender, EventArgs e)
		{
			AbstractWizardPanel panel = (AbstractWizardPanel)CurrentWizardPane;
			bool isLast = panel.IsLastPanel;
			panel.IsLastPanel = false;
			ShowNextPanelEvent(sender, e);
			panel.IsLastPanel = isLast;
		}
		
		void ShowNextPanelEvent(object sender, EventArgs e)
		{
			int nextID = GetSuccessorNumber(this.ActivePanelNumber);
			Debug.Assert(nextID < wizardPanels.Count && nextID >= 0);
			if (!CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Next)) {
				return;
			}
			idStack.Push(activePanelNumber);
			ActivatePanel(nextID);
			CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Activated);
		}
		
		void ShowPrevPanelEvent(object sender, EventArgs e)
		{
			Debug.Assert(idStack.Count > 0);
			if (!CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Prev)) {
				return;
			}
			ActivatePanel((int)idStack.Pop());
		}
		
		void FinishEvent(object sender, EventArgs e)
		{
			foreach (IDialogPanelDescriptor descriptor in wizardPanels) {
				if (!descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Finish)) {
					return;
				}
			}
			DialogResult = DialogResult.OK;
		}
		
		void CancelEvent(object sender, EventArgs e)
		{
			foreach (IDialogPanelDescriptor descriptor in wizardPanels) {
				if (!descriptor.DialogPanel.ReceiveDialogMessage(DialogMessage.Cancel)) {
					return;
				}
			}
			DialogResult = DialogResult.Cancel;
		}
		
		void HelpEvent(object sender, EventArgs e)
		{
			CurrentWizardPane.ReceiveDialogMessage(DialogMessage.Help);
		}
		
		
		void InitializeComponents()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			this.SuspendLayout();
			
			ShowInTaskbar = false;
			StartPosition   = FormStartPosition.CenterScreen;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MinimizeBox = MaximizeBox = false;
			Icon   = null;
			ClientSize = new Size(640, 440);
			
			int buttonSize = 92;
			int buttonYLoc = 464 -  2 * 24  - 4;
			int buttonXStart = Width - ((buttonSize + 4) * 4) - 4;
			
			label1.Size        = new Size(Width - 4, 1);
			label1.BorderStyle = BorderStyle.Fixed3D;
			label1.Location    = new Point(2, 404 - 2);
			label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left;
			Controls.Add(label1);
			
			backButton.Text = resourceService.GetString("Global.BackButtonText");
			backButton.Location = new Point(buttonXStart, buttonYLoc);
			backButton.ClientSize     = new Size(buttonSize, 26);
			backButton.Click   += new EventHandler(ShowPrevPanelEvent);
			backButton.FlatStyle = FlatStyle.System;
			backButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(backButton);
			
			nextButton.Text = resourceService.GetString("Global.NextButtonText");
			nextButton.Location = new Point(buttonXStart + buttonSize + 4, buttonYLoc);
			nextButton.ClientSize     = new Size(buttonSize, 26);
			nextButton.Click   += new EventHandler(ShowNextPanelEvent);
			nextButton.FlatStyle = FlatStyle.System;
			nextButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(nextButton);
			
			finishButton.Text = resourceService.GetString("Dialog.WizardDialog.FinishButton");
			finishButton.Location = new Point(buttonXStart + 2 * (buttonSize + 4), buttonYLoc);
			finishButton.ClientSize     = new Size(buttonSize, 26);
			finishButton.Click   += new EventHandler(FinishEvent);
			finishButton.FlatStyle = FlatStyle.System;
			finishButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(finishButton);
			
			cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			cancelButton.Location = new Point(buttonXStart + 3 * (buttonSize + 4), buttonYLoc);
			cancelButton.ClientSize     = new Size(buttonSize, 26);
			cancelButton.Click   += new EventHandler(CancelEvent);
			cancelButton.FlatStyle = FlatStyle.System;
			cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			Controls.Add(cancelButton);
			
//			helpButton.Text = resourceService.GetString("Global.HelpButtonText");
//			helpButton.Location = new Point(buttonXStart + 4 * (buttonSize + 4), buttonYLoc);
//			helpButton.ClientSize     = new Size(buttonSize, 26);
//			helpButton.Click   += new EventHandler(HelpEvent);
//			helpButton.FlatStyle = FlatStyle.System;
//			helpButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
//			Controls.Add(helpButton);
			
			statusPanel = new StatusPanel(this);
			statusPanel.Location = new Point(2, 2);
			statusPanel.Anchor = AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left;
			Controls.Add(statusPanel);
			
			curPanel = new CurrentPanelPanel(this);
			curPanel.Location = new Point(200, 2);
			curPanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left;
			Controls.Add(curPanel);
			
			dialogPanel.Location    = new Point(200, 27);
			//        	dialogPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			dialogPanel.Size        = new Size(Width - 8 - statusPanel.Bounds.Right,
			                                   label1.Location.Y - dialogPanel.Location.Y);
			dialogPanel.Anchor = AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom;
			Controls.Add(dialogPanel);
			this.ResumeLayout(true);
		}
	}
}
