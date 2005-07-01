// created on 10/11/2002 at 2:06 PM

using Microsoft.Win32;
using System;
using System.Data;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Xml;
using System.Xml.Xsl;
using System.Web.Services.Description;
using System.Web.Services.Discovery;
using System.Windows.Forms;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.BrowserDisplayBinding;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	public class AddWebReferenceDialog : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ListView webServicePropertiesListView;
		private System.Windows.Forms.Panel bottomPanel;
		private System.Windows.Forms.Splitter splitter;
		private System.Windows.Forms.Label urlLabel;
		private System.Windows.Forms.Button forwardButton;
		private System.Windows.Forms.ColumnHeader valueColumnHeader;
		private System.Windows.Forms.ComboBox urlComboBox;
		private System.Windows.Forms.TextBox referenceNameTextBox;
		private System.Windows.Forms.Button abortButton;
		private System.Windows.Forms.ImageList tabControlImageList;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Label namespaceLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.TabPage wsdlTabPage;
		private System.Windows.Forms.TreeView webServicesTreeView;
		private System.Windows.Forms.Button goButton;
		private System.Windows.Forms.Panel urlPanel;
		private System.Windows.Forms.Label referenceNameLabel;
		private System.Windows.Forms.ColumnHeader propertyColumnHeader;
		private System.Windows.Forms.ImageList webServicesTreeViewImageList;
		private System.Windows.Forms.Button refreshButton;
		private System.Windows.Forms.TabPage webServicesTabPage;
		private System.Windows.Forms.TextBox namespaceTextBox;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.ToolTip tips;
		private AxWebBrowser webBrowser;

		IProject project = null;
		ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
		
		DiscoveryClientProtocol discoveryClientProtocol = null;
		
		delegate DiscoveryDocument DiscoverAnyAsync(string url);
		delegate void DisplayWebServiceHandler(ServiceDescription desc);
		
		ArrayList referenceInformations = new ArrayList();
		public ArrayList ReferenceInformations 
		{
			get {																		
				return referenceInformations;
			}
		}

		public AddWebReferenceDialog(IProject p)
		{			
			InitDialog();
			
			WebServicePropertiesListViewResize(null, null);
			
			InitWebBrowser();	
			InitImages();
			InitAutoCompletion();
						
			this.project = p;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
				
		private void InitDialog()
		{
			this.components = new System.ComponentModel.Container();
			this.tips = new System.Windows.Forms.ToolTip(this.components);
			this.tabControl = new System.Windows.Forms.TabControl();
			this.backButton = new System.Windows.Forms.Button();
			this.namespaceTextBox = new System.Windows.Forms.TextBox();
			this.webServicesTabPage = new System.Windows.Forms.TabPage();
			this.refreshButton = new System.Windows.Forms.Button();
			this.webServicesTreeViewImageList = new System.Windows.Forms.ImageList(this.components);
			this.propertyColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.referenceNameLabel = new System.Windows.Forms.Label();
			this.urlPanel = new System.Windows.Forms.Panel();
			this.goButton = new System.Windows.Forms.Button();
			this.webServicesTreeView = new System.Windows.Forms.TreeView();
			this.wsdlTabPage = new System.Windows.Forms.TabPage();
			this.cancelButton = new System.Windows.Forms.Button();
			this.namespaceLabel = new System.Windows.Forms.Label();
			this.addButton = new System.Windows.Forms.Button();
			this.tabControlImageList = new System.Windows.Forms.ImageList(this.components);
			this.abortButton = new System.Windows.Forms.Button();
			this.referenceNameTextBox = new System.Windows.Forms.TextBox();
			this.urlComboBox = new System.Windows.Forms.ComboBox();
			this.valueColumnHeader = new System.Windows.Forms.ColumnHeader();
			this.forwardButton = new System.Windows.Forms.Button();
			this.urlLabel = new System.Windows.Forms.Label();
			this.splitter = new System.Windows.Forms.Splitter();
			this.bottomPanel = new System.Windows.Forms.Panel();
			this.webServicePropertiesListView = new System.Windows.Forms.ListView();
			this.tabControl.SuspendLayout();
			this.webServicesTabPage.SuspendLayout();
			this.urlPanel.SuspendLayout();
			this.bottomPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl
			// 
			this.tabControl.Controls.Add(this.wsdlTabPage);
			this.tabControl.Controls.Add(this.webServicesTabPage);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.ImageList = this.tabControlImageList;
			this.tabControl.Location = new System.Drawing.Point(0, 32);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(722, 371);
			this.tabControl.TabIndex = 1;
			// 
			// backButton
			// 
			this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.backButton.Location = new System.Drawing.Point(4, 4);
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(28, 24);
			this.backButton.TabIndex = 3;
			this.tips.SetToolTip(this.backButton, resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BackButtonTooltip"));
			this.backButton.Click += new System.EventHandler(this.BackButtonClick);
			// 
			// namespaceTextBox
			// 
			this.namespaceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.namespaceTextBox.Location = new System.Drawing.Point(144, 32);
			this.namespaceTextBox.Name = "namespaceTextBox";
			this.namespaceTextBox.Size = new System.Drawing.Size(472, 21);
			this.namespaceTextBox.TabIndex = 3;
			this.namespaceTextBox.Text = "";
			// 
			// webServicesTabPage
			// 
			this.webServicesTabPage.BackColor = System.Drawing.Color.White;
			this.webServicesTabPage.Controls.Add(this.webServicePropertiesListView);
			this.webServicesTabPage.Controls.Add(this.splitter);
			this.webServicesTabPage.Controls.Add(this.webServicesTreeView);
			this.webServicesTabPage.ImageIndex = 0;
			this.webServicesTabPage.Location = new System.Drawing.Point(4, 23);
			this.webServicesTabPage.Name = "webServicesTabPage";
			this.webServicesTabPage.Size = new System.Drawing.Size(714, 344);
			this.webServicesTabPage.TabIndex = 1;
			this.webServicesTabPage.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.WebServicesTabPageTitle");
			// 
			// refreshButton
			// 
			this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.refreshButton.Location = new System.Drawing.Point(88, 4);
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Size = new System.Drawing.Size(28, 24);
			this.refreshButton.TabIndex = 6;
			this.tips.SetToolTip(this.refreshButton, resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RefreshButtonTooltip"));
			this.refreshButton.Click += new System.EventHandler(this.RefreshButtonClick);
			// 
			// webServicesTreeViewImageList
			// 
			this.webServicesTreeViewImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.webServicesTreeViewImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// propertyColumnHeader
			// 
			this.propertyColumnHeader.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.PropertyColumnHeader");
			this.propertyColumnHeader.Width = 209;
			// 
			// referenceNameLabel
			// 
			this.referenceNameLabel.Location = new System.Drawing.Point(8, 8);
			this.referenceNameLabel.Name = "referenceNameLabel";
			this.referenceNameLabel.Size = new System.Drawing.Size(112, 16);
			this.referenceNameLabel.TabIndex = 0;
			this.referenceNameLabel.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ReferenceNameLabel");
			// 
			// urlPanel
			// 
			this.urlPanel.Controls.Add(this.urlComboBox);
			this.urlPanel.Controls.Add(this.urlLabel);
			this.urlPanel.Controls.Add(this.goButton);
			this.urlPanel.Controls.Add(this.refreshButton);
			this.urlPanel.Controls.Add(this.abortButton);
			this.urlPanel.Controls.Add(this.forwardButton);
			this.urlPanel.Controls.Add(this.backButton);
			this.urlPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.urlPanel.Location = new System.Drawing.Point(0, 0);
			this.urlPanel.Name = "urlPanel";
			this.urlPanel.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.urlPanel.Size = new System.Drawing.Size(722, 32);
			this.urlPanel.TabIndex = 0;
			// 
			// goButton
			// 
			this.goButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.goButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.goButton.Location = new System.Drawing.Point(690, 4);
			this.goButton.Name = "goButton";
			this.goButton.Size = new System.Drawing.Size(28, 24);
			this.goButton.TabIndex = 2;
			this.tips.SetToolTip(this.goButton, resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.GoButtonTooltip"));
			this.goButton.Click += new System.EventHandler(this.GoButtonClick);
			// 
			// webServicesTreeView
			// 
			this.webServicesTreeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.webServicesTreeView.HotTracking = true;
			this.webServicesTreeView.ImageList = this.webServicesTreeViewImageList;
			this.webServicesTreeView.Location = new System.Drawing.Point(0, 0);
			this.webServicesTreeView.Name = "webServicesTreeView";
			this.webServicesTreeView.ShowRootLines = false;
			this.webServicesTreeView.Size = new System.Drawing.Size(280, 344);
			this.webServicesTreeView.TabIndex = 0;
			this.webServicesTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.WebServicesTreeViewAfterSelect);
			// 
			// wsdlTabPage
			// 
			this.wsdlTabPage.ImageIndex = 1;
			this.wsdlTabPage.Location = new System.Drawing.Point(4, 23);
			this.wsdlTabPage.Name = "wsdlTabPage";
			this.wsdlTabPage.Size = new System.Drawing.Size(714, 344);
			this.wsdlTabPage.TabIndex = 0;
			this.wsdlTabPage.Text = "WSDL";
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.cancelButton.Location = new System.Drawing.Point(632, 32);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 23);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = resourceService.GetString("Global.CancelButtonText");
			this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
			// 
			// namespaceLabel
			// 
			this.namespaceLabel.Location = new System.Drawing.Point(8, 32);
			this.namespaceLabel.Name = "namespaceLabel";
			this.namespaceLabel.Size = new System.Drawing.Size(112, 16);
			this.namespaceLabel.TabIndex = 1;
			this.namespaceLabel.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.NamespaceLabel");
			// 
			// addButton
			// 
			this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.addButton.Enabled = false;
			this.addButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.addButton.Location = new System.Drawing.Point(632, 8);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(80, 23);
			this.addButton.TabIndex = 4;
			this.addButton.Text = resourceService.GetString("Global.AddButtonText");
			this.addButton.Click += new System.EventHandler(this.AddButtonClick);
			// 
			// tabControlImageList
			// 
			this.tabControlImageList.ImageSize = new System.Drawing.Size(16, 16);
			this.tabControlImageList.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// abortButton
			// 
			this.abortButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.abortButton.Location = new System.Drawing.Point(60, 4);
			this.abortButton.Name = "abortButton";
			this.abortButton.Size = new System.Drawing.Size(28, 24);
			this.abortButton.TabIndex = 5;
			this.tips.SetToolTip(this.abortButton, resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.StopButtonTooltip"));
			this.abortButton.Click += new System.EventHandler(this.AbortButtonClick);
			// 
			// referenceNameTextBox
			// 
			this.referenceNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.referenceNameTextBox.Location = new System.Drawing.Point(144, 8);
			this.referenceNameTextBox.Name = "referenceNameTextBox";
			this.referenceNameTextBox.Size = new System.Drawing.Size(472, 21);
			this.referenceNameTextBox.TabIndex = 2;
			this.referenceNameTextBox.Text = "";
			// 
			// urlComboBox
			// 
			this.urlComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
						| System.Windows.Forms.AnchorStyles.Right)));
			this.urlComboBox.Location = new System.Drawing.Point(208, 4);
			this.urlComboBox.Name = "urlComboBox";
			this.urlComboBox.Size = new System.Drawing.Size(472, 21);
			this.urlComboBox.TabIndex = 1;
			this.urlComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.UrlComboBoxKeyDown);
			this.urlComboBox.SelectedIndexChanged += new System.EventHandler(this.UrlComboBoxSelectedIndexChanged);
			// 
			// valueColumnHeader
			// 
			this.valueColumnHeader.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ValueColumnHeader");
			this.valueColumnHeader.Width = 162;
			// 
			// forwardButton
			// 
			this.forwardButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.forwardButton.Location = new System.Drawing.Point(32, 4);
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new System.Drawing.Size(28, 24);
			this.forwardButton.TabIndex = 4;
			this.tips.SetToolTip(this.forwardButton, resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ForwardButtonTooltip"));
			this.forwardButton.Click += new System.EventHandler(this.ForwardButtonClick);
			// 
			// urlLabel
			// 
			this.urlLabel.Location = new System.Drawing.Point(116, 4);
			this.urlLabel.Name = "urlLabel";
			this.urlLabel.Size = new System.Drawing.Size(84, 24);
			this.urlLabel.TabIndex = 0;
			this.urlLabel.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.UrlAddressLabel");
			this.urlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// splitter
			// 
			this.splitter.Location = new System.Drawing.Point(280, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(3, 344);
			this.splitter.TabIndex = 4;
			this.splitter.TabStop = false;
			// 
			// bottomPanel
			// 
			this.bottomPanel.Controls.Add(this.referenceNameLabel);
			this.bottomPanel.Controls.Add(this.namespaceTextBox);
			this.bottomPanel.Controls.Add(this.namespaceLabel);
			this.bottomPanel.Controls.Add(this.referenceNameTextBox);
			this.bottomPanel.Controls.Add(this.cancelButton);
			this.bottomPanel.Controls.Add(this.addButton);
			this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomPanel.Location = new System.Drawing.Point(0, 403);
			this.bottomPanel.Name = "bottomPanel";
			this.bottomPanel.Size = new System.Drawing.Size(722, 64);
			this.bottomPanel.TabIndex = 2;
			// 
			// webServicePropertiesListView
			// 
			this.webServicePropertiesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.propertyColumnHeader,
						this.valueColumnHeader});
			this.webServicePropertiesListView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webServicePropertiesListView.Location = new System.Drawing.Point(283, 0);
			this.webServicePropertiesListView.Name = "webServicePropertiesListView";
			this.webServicePropertiesListView.Size = new System.Drawing.Size(431, 344);
			this.webServicePropertiesListView.TabIndex = 3;
			this.webServicePropertiesListView.View = System.Windows.Forms.View.Details;
			this.webServicePropertiesListView.Resize += new System.EventHandler(this.WebServicePropertiesListViewResize);
			// 
			// AddWebReferenceDialog
			// 
			this.AcceptButton = this.goButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(722, 467);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.bottomPanel);
			this.Controls.Add(this.urlPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(400, 300);
			this.Name = "AddWebReferenceDialog";
			this.ShowInTaskbar = false;
			this.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DialogTitle");
			this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogClosing);
			this.tabControl.ResumeLayout(false);
			this.webServicesTabPage.ResumeLayout(false);
			this.urlPanel.ResumeLayout(false);
			this.bottomPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		
		void BackButtonClick(object sender, System.EventArgs e)
		{
			try {
				webBrowser.GoBack();
			} catch (Exception) { }
		}
		
		void ForwardButtonClick(object sender, System.EventArgs e)
		{
			try {
				webBrowser.GoForward();
			} catch (Exception) { }
		}
		
		void AbortButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Stop();
			StopDiscovery();
			this.addButton.Enabled = false;
		}

		void RefreshButtonClick(object sender, System.EventArgs e)
		{
			webBrowser.Refresh();
			urlComboBox.Text = webBrowser.LocationURL;
		}

		void GoButtonClick(object sender, System.EventArgs e)
		{							
			BrowseUrl(urlComboBox.Text);
		}
		
		void BrowseUrl(string url)
		{
			object urlObject = (object)urlComboBox.Text;
			object flags = null;
			object targetframename = (object)"_top";
			object headers = null;
			object postdata = null;
			
			webBrowser.Focus();
			webBrowser.Navigate2(ref urlObject, ref flags, ref targetframename, ref postdata, ref headers);						
		}
		
		ServiceDescription serviceDescription = null;
		ServiceDescription ServiceDescription {
			get {
				return serviceDescription;				
			}
			set {				
				this.addButton.Enabled = (value != null);
				serviceDescription = value;								
			}
		}
		
		void AddButtonClick(object sender, EventArgs e)
		{			
			try {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				if (!IsValidReferenceName) {
					messageService.ShowError(resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidReferenceNameError"));
					return;					
				}
				
				if (!IsValidNamespace) {
					messageService.ShowError(resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.InvalidNamespaceError"));
					return;										
				}
								
				ArrayList fileList = WebReference.GenerateWebProxyCode(namespaceTextBox.Text, referenceNameTextBox.Text, project, serviceDescription);
				if(fileList != null) {
					referenceInformations.AddRange(fileList);
				}
				
				DialogResult = DialogResult.OK;
				Close();
			} catch (Exception ex) {
				messageService.ShowError(ex);
			}
		}

		void CancelButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		void WebBrowserNavigateComplete2(object sender, DWebBrowserEvents2_NavigateComplete2Event e)
		{
			Cursor = Cursors.Default;	
			urlComboBox.Text = webBrowser.LocationURL;
				
			namespaceTextBox.Text = GetDefaultNamespace();
			referenceNameTextBox.Text = WebReference.GetNamespaceFromUri(webBrowser.LocationURL);
		}
		
		void WebServicesTreeViewAfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{						
			this.webServicePropertiesListView.BeginUpdate();
						
			ListViewItem item;
			
			this.webServicePropertiesListView.Items.Clear();
					
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			if(e.Node.Tag is ServiceDescription) 
			{
				ServiceDescription desc = (ServiceDescription)e.Node.Tag;
				item = new ListViewItem();				
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RetrievalUriProperty");
				item.SubItems.Add(desc.RetrievalUrl);
				this.webServicePropertiesListView.Items.Add(item);
			}
			else if(e.Node.Tag is Service)
			{
				Service service = (Service)e.Node.Tag;
				item = new ListViewItem();				
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty");
				item.SubItems.Add(service.Documentation);
				this.webServicePropertiesListView.Items.Add(item);

			}
			else if(e.Node.Tag is Port) 
			{
				Port port = (Port)e.Node.Tag;

				item = new ListViewItem();				
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty");
				item.SubItems.Add(port.Documentation);
				this.webServicePropertiesListView.Items.Add(item);
				
				item = new ListViewItem();				
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BindingProperty");
				item.SubItems.Add(port.Binding.Name);
				this.webServicePropertiesListView.Items.Add(item);
				
				item = new ListViewItem();				
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ServiceNameProperty");
				item.SubItems.Add(port.Service.Name);												
				this.webServicePropertiesListView.Items.Add(item);

			}
			else if(e.Node.Tag is Operation) 
			{
				Operation operation = (Operation)e.Node.Tag;
				
				item = new ListViewItem();
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty");
				item.SubItems.Add(operation.Documentation);
				this.webServicePropertiesListView.Items.Add(item);

				item = new ListViewItem();
				item.Text = resourceService.GetString("ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ParametersProperty");
				item.SubItems.Add(operation.ParameterOrderString);
				this.webServicePropertiesListView.Items.Add(item);
			}
			
			this.webServicePropertiesListView.EndUpdate();		
		}
		
		void WebServicePropertiesListViewResize(object sender, EventArgs e)
		{
			// resize the column headers
			this.propertyColumnHeader.Width = this.webServicePropertiesListView.Width / 2;
			this.valueColumnHeader.Width = this.webServicePropertiesListView.Width / 2;
		}
		
		void WebBrowserBeforeNavigate2(object sender, DWebBrowserEvents2_BeforeNavigate2Event e)
		{
			Cursor = Cursors.WaitCursor;
			ServiceDescription = null;
			ClearWebService();
			StartDiscovery((string)e.uRL);
		}

		void WebBrowserNavigateError(object sender, DWebBrowserEvents2_NavigateErrorEvent e)
		{
			Cursor = Cursors.Default;
			ClearWebService();
			ServiceDescription = null;
		}

		void InitWebBrowser()
		{
			this.webBrowser = new ICSharpCode.SharpDevelop.BrowserDisplayBinding.AxWebBrowser();			
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).BeginInit();

			this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser.Enabled = true;
			this.webBrowser.Location = new System.Drawing.Point(0, 23);
			//this.webBrowser.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("webBrowser.OcxState")));
			this.webBrowser.Size = new System.Drawing.Size(336, 280);
			this.webBrowser.TabIndex = 3;
			this.webBrowser.NavigateComplete2 += new DWebBrowserEvents2_NavigateComplete2EventHandler(this.WebBrowserNavigateComplete2);
			this.webBrowser.NavigateError += new DWebBrowserEvents2_NavigateErrorEventHandler(this.WebBrowserNavigateError);			
			this.webBrowser.BeforeNavigate2 += new DWebBrowserEvents2_BeforeNavigate2EventHandler(this.WebBrowserBeforeNavigate2);
			
			this.wsdlTabPage.Controls.AddRange(new System.Windows.Forms.Control[] {this.webBrowser});
			((System.ComponentModel.ISupportInitialize)(this.webBrowser)).EndInit();
		}
		
		void InitImages()
		{
			this.goButton.Image = this.resourceService.GetBitmap("Icons.16x16.RunProgramIcon");
			this.refreshButton.Image = this.resourceService.GetBitmap("Icons.16x16.BrowserRefresh");
			this.backButton.Image = this.resourceService.GetBitmap("Icons.16x16.BrowserBefore");
			this.forwardButton.Image = this.resourceService.GetBitmap("Icons.16x16.BrowserAfter");
			this.abortButton.Image = this.resourceService.GetBitmap("Icons.16x16.BrowserCancel");

			this.tabControlImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Class"));
			this.tabControlImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.HTMLIcon"));

			// Treeview
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Assembly"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Assembly"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Class"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Class"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Interface"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Interface"));
			this.webServicesTreeViewImageList.Images.Add(resourceService.GetBitmap("Icons.16x16.Library"));
			
			this.Icon = resourceService.GetIcon("Icons.16x16.ClosedWebReferencesFolder");	
		}
	
		/// <summary>
		/// Gets the namespace to be used with the generated web reference code.
		/// </summary>
		string GetDefaultNamespace()
		{
			string defaultNamespace = String.Empty;
			
			if (project.StandardNamespace.Length > 0) {
				defaultNamespace = String.Concat(project.StandardNamespace, ".", WebReference.GetNamespaceFromUri(webBrowser.LocationURL));
			} else {
				defaultNamespace = WebReference.GetNamespaceFromUri(webBrowser.LocationURL);
			}

			return defaultNamespace;
		}
		
		/// <summary>
		/// Updates the web services tree view after we have navigated to a different
		/// url.
		/// </summary>
		void DisplayWebService(ServiceDescription desc)
		{
			webServicesTreeView.BeginUpdate();
			
			try {
				ClearWebService();
	
				ServiceDescription = desc;	
				
				if(desc == null) 
					return;
								
				TreeNode rootNode = new TreeNode(WebReference.GetNamespaceFromUri(webBrowser.LocationURL));			
				rootNode.Tag = desc;
				rootNode.ImageIndex = 6;
				rootNode.SelectedImageIndex = 6;
				webServicesTreeView.Nodes.Add(rootNode);
				
				foreach(Service svc in desc.Services) 
				{
					// add a Service node
					TreeNode serviceNode = new TreeNode(svc.Name);				
					serviceNode.Tag = svc;
					serviceNode.ImageIndex = 0;
					serviceNode.SelectedImageIndex = 1;
					rootNode.Nodes.Add(serviceNode);
					
					foreach(Port port in svc.Ports) 
					{					
						TreeNode portNode = new TreeNode(port.Name);
						portNode.Tag = port;
						portNode.ImageIndex = 2;
						portNode.SelectedImageIndex = 3;
						serviceNode.Nodes.Add(portNode);
						
											
						// get the operations
						foreach(Operation operation in desc.PortTypes[port.Name].Operations) 
						{
							TreeNode operationNode = new TreeNode(operation.Name);
							operationNode.Tag = operation;
							operationNode.ImageIndex = 4;
							operationNode.SelectedImageIndex = 5;
							portNode.Nodes.Add(operationNode);
						}				
					}															
				}
				webServicesTreeView.ExpandAll();	
								
			} finally {
				webServicesTreeView.EndUpdate();
			}
		}	
		
		bool IsValidNamespace {
			get {
				bool valid = false;
				
				if (namespaceTextBox.Text.Length > 0) {
					
					// Can only begin with a letter or '_'
					char ch = namespaceTextBox.Text[0];
					if (Char.IsLetter(ch) || (ch == '_')) {
						valid = true;
						for (int i = 1; i < namespaceTextBox.Text.Length; ++i) {
							ch = namespaceTextBox.Text[i];
							// Can only contain letters, digits or '_'
							if (!Char.IsLetterOrDigit(ch) && (ch != '.') && (ch != '_')) {
								valid = false;
								break;
							}
						}
					}					
				}
				
				return valid;
			}
		}
		
		bool IsValidReferenceName {
			get {
				bool valid = false;
				
				if (referenceNameTextBox.Text.Length > 0) {
					if (referenceNameTextBox.Text.IndexOf('\\') == -1) {
						if (!ContainsInvalidDirectoryChar(referenceNameTextBox.Text)) {
						    	valid = true;
						}
					}
				}
				
				return valid;
			}
		}
		
		bool ContainsInvalidDirectoryChar(string item)
		{
			bool hasInvalidChar = false;
			
			foreach (char ch in Path.InvalidPathChars) {
				if (item.IndexOf(ch) >= 0) {
					hasInvalidChar = true;
					break;
				}
			}
				
			return hasInvalidChar;
		}
		
		void ClearWebService()
		{
			webServicePropertiesListView.Items.Clear();
			webServicesTreeView.Nodes.Clear();
		}

		/// <summary>
		/// Starts the search for web services at the specified url.
		/// </summary>
		void StartDiscovery(string url)
		{
			// Abort previous discovery.
			StopDiscovery();
			
			// Start new discovery.
			DiscoverAnyAsync asyncDelegate = new DiscoverAnyAsync(discoveryClientProtocol.DiscoverAny);
			AsyncCallback callback = new AsyncCallback(DiscoveryCompleted);
			IAsyncResult result = asyncDelegate.BeginInvoke(url, callback, discoveryClientProtocol);
		}
		
		/// <summary>
		/// Called after an asynchronous web services search has
		/// completed.
		/// </summary>
		void DiscoveryCompleted(IAsyncResult result)
		{
			DiscoveryClientProtocol protocol = (DiscoveryClientProtocol)result.AsyncState;
					
			// Check that we are still waiting for this particular callback.
			bool wanted = false;
			lock (this) {
				wanted = Object.ReferenceEquals(discoveryClientProtocol, protocol);
			}
			
			if (wanted) {
				DisplayWebServiceHandler displayHandler = new DisplayWebServiceHandler(DisplayWebService);
				try {
					DiscoverAnyAsync asyncDelegate = (DiscoverAnyAsync)((AsyncResult)result).AsyncDelegate;
					DiscoveryDocument doc = asyncDelegate.EndInvoke(result);
					Invoke(displayHandler, new object[] {GetServiceDescription(protocol)});
				} catch (Exception) {
					Invoke(displayHandler, new object[] {null});
				}
			} 
		}
		
		/// <summary>
		/// Stops any outstanding asynchronous discovery requests.
		/// </summary>
		void StopDiscovery()
		{
			lock (this) {
				if (discoveryClientProtocol != null) {
					try {
						discoveryClientProtocol.Abort();
					} catch (NotImplementedException) {};
					discoveryClientProtocol.Dispose();
				}
				discoveryClientProtocol = new DiscoveryClientProtocol();
			}
		}
		
		void DialogClosing(object sender, CancelEventArgs e)
		{
			StopDiscovery();
		}
		
		ServiceDescription GetServiceDescription(DiscoveryClientProtocol protocol)
		{
			ServiceDescription desc = null;
			protocol.ResolveOneLevel();
			
			foreach (DictionaryEntry entry in protocol.References) {
				ContractReference contractRef = entry.Value as ContractReference;				
				if (contractRef != null) {
					desc = contractRef.Contract;
					break;
				}
			}
			
			return desc;
		}
		
		void UrlComboBoxKeyDown(object sender, KeyEventArgs e)
		{
			if((e.KeyValue == '\r') && (urlComboBox.Text != null) && (urlComboBox.Text != "")) {
				BrowseUrl(urlComboBox.Text);
			}
		}
		
		void UrlComboBoxSelectedIndexChanged(object sender, System.EventArgs e)
		{
			BrowseUrl(urlComboBox.Text);
		}		
		
		#region Url Autocompletion
		
		[Flags]
		enum AutoCompleteFlags : uint
		{
			Default             = 0x00000000,
			FileSystem          = 0x00000001,
			UrlHistory          = 0x00000002,
			UrlMenu             = 0x00000004,
			UseTab              = 0x00000008,
			FileSystemOnly      = 0x00000010,
			UrlAll              = UrlHistory|UrlMenu,
			FileSystemDirs      = 0x00000020,
			AutoSuggestForceOn  = 0x10000000,
			AutoSuggestForceOff = 0x20000000,
			AutoAppendForceOn   = 0x40000000,
			AutoAppendForceOff  = 0x80000000
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct COMBOBOXINFO {
			public Int32 cbSize;
			public RECT rcItem;
			public RECT rcButton;
			public ComboBoxButtonState buttonState;
			public IntPtr hwndCombo;
			public IntPtr hwndEdit;
			public IntPtr hwndList;
		}
		
		enum ComboBoxButtonState {
			STATE_SYSTEM_NONE = 0,
			STATE_SYSTEM_INVISIBLE = 0x00008000,
			STATE_SYSTEM_PRESSED = 0x00000008
		}
		
		[StructLayout(LayoutKind.Sequential)]
		struct RECT 
		{
		        public int left;
		        public int top;
		        public int right;
		        public int bottom;
		}
		
		[DllImport("user32.dll")]
		static extern bool GetComboBoxInfo(IntPtr hWnd, ref COMBOBOXINFO pcbi);
		
		[DllImport("shlwapi.dll")]
		static extern int SHAutoComplete(IntPtr Handle, uint Flags);
		
		/// <summary>
		/// Sets up autocompletion for the url combo box.
		/// </summary>
		void InitAutoCompletion()
		{
			COMBOBOXINFO cbi = new COMBOBOXINFO();
			cbi.cbSize = Marshal.SizeOf(cbi);
			if(GetComboBoxInfo(urlComboBox.Handle, ref cbi)) {
				if(cbi.hwndEdit != IntPtr.Zero) {
					SHAutoComplete(cbi.hwndEdit, (uint)AutoCompleteFlags.Default);
					AddMRUList();
			    }
			}
		}
		
		/// <summary>
		/// Reads MRU list from registry and adds it to the url combo box.
		/// </summary>
		void AddMRUList()
		{
			try {
				RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Internet Explorer\TypedURLs");
				foreach (string name in key.GetValueNames()) {
					urlComboBox.Items.Add((string)key.GetValue(name));
				}
				
			} catch (Exception) { };
		}
		
		#endregion
	}
}
