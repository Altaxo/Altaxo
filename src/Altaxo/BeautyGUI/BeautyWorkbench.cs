// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

// <originalsource>SharpDevelop 0.96</originalsource>
// <originalfile>Base/Gui/Workbench/BeautyWorkbench</originalfile>
using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;


//using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.Core.Services;

using Reflector.UserInterface;

using Altaxo;

using ICSharpCode.SharpDevelop.Services;



namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class BeautyWorkbench : IWorkbench, Altaxo.IMainController
	{
		readonly static string mainMenuPath    = "/SharpDevelop/Workbench/MainMenu";
		readonly static string viewContentPath = "/SharpDevelop/Workbench/Views";
		
		PadContentCollection viewContentCollection       = new PadContentCollection();
		ViewContentCollection workbenchContentCollection = new ViewContentCollection();
		
		bool closeAll = false;
		
		bool            fullscreen;
		FormWindowState defaultWindowState = FormWindowState.Normal;
		Rectangle       normalBounds       = new Rectangle(0, 0, 640, 480);
		
		IWorkbenchLayout layout = null;
		
		protected static PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		

		BeautyWorkbenchWindow m_View;
		string m_Title;


		public BeautyWorkbenchWindow View
		{
			get { return m_View; }
			set { m_View = value; }
		}
		
		public object ViewObject
		{
			get 
			{
				return m_View;
			}
		}

		public bool IsClosingAll
		{
			get { return closeAll; }
		}

		// Lellid: move this to the view
		public bool FullScreen {
			get
			{
				return fullscreen;
			}
			set
			{
				fullscreen = value;
				if(null!=View)
					View.FullScreen = value;
			}
		}
		
		public string Title {
			get {
				return m_Title;
			}
			set {
				m_Title = value;
				
				if(null!=View)
					View.Title = value;
			}
		}
		
		EventHandler windowChangeEventHandler;
		
		public IWorkbenchLayout WorkbenchLayout {
			get {
				return layout;
			}
			set {
				if (layout != null) {
					layout.ActiveWorkbenchWindowChanged -= windowChangeEventHandler;
					layout.Detach();
				}
				value.Attach(this);
				layout = value;
				layout.ActiveWorkbenchWindowChanged += windowChangeEventHandler;
			}
		}
		
		public PadContentCollection PadContentCollection {
			get {
				Debug.Assert(viewContentCollection != null);
				return viewContentCollection;
			}
		}
		
		public ViewContentCollection ViewContentCollection {
			get {
				Debug.Assert(workbenchContentCollection != null);
				return workbenchContentCollection;
			}
		}
		
		public IWorkbenchWindow ActiveWorkbenchWindow {
			get {
				if (layout == null) {
					return null;
				}
				return layout.ActiveWorkbenchwindow;
			}
		}
		
		public BeautyWorkbench(BeautyWorkbenchWindow window, Altaxo.AltaxoDocument doc)
		{
			
			m_View = window;
			m_View.Controller = this;
			
			// we construct the main document
			if(null==m_Doc)
				m_Doc = new AltaxoDocument();
			else
				m_Doc = doc;

			// we initialize the printer variables
			m_PrintDocument = new System.Drawing.Printing.PrintDocument();
			// we set the print document default orientation to landscape
			m_PrintDocument.DefaultPageSettings.Landscape=true;
			m_PageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			m_PageSetupDialog.Document = m_PrintDocument;
			m_PrintDialog = new System.Windows.Forms.PrintDialog();
			m_PrintDialog.Document = m_PrintDocument;

			// we create the menu and assign it to the view
			//this.InitializeMenu();
			//View.MainViewMenu = this.m_MainMenu;


			// wir konstruieren zu jeder Tabelle im Dokument ein GrafTabView
			CreateNewWorksheet(Doc.CreateNewTable("WKS0",false));

			// we construct a empty graph by default
			CreateNewGraph(Doc.CreateNewGraphDocument());

#if LellidMod
#else
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Text = resourceService.GetString("MainWindow.DialogName");
			Icon = resourceService.GetIcon("Icons.SharpDevelopIcon");
#endif
	
			windowChangeEventHandler = new EventHandler(OnActiveWindowChanged);
			
			if(View!=null)
			{
				View.StartPosition = FormStartPosition.Manual;
			
				View.AllowDrop      = true;

				this.WorkbenchLayout = new MdiWorkbenchLayout();
			}
		}
		
		public void InitializeWorkspace()
		{
			View.Menu = null;
			
			//			statusBarManager.Control.Dock = DockStyle.Bottom;
			
			ActiveWorkbenchWindowChanged += new EventHandler(View.UpdateMenu);
			
			View.MenuComplete += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
#if LellidMod
#else
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			projectService.CurrentProjectChanged += new ProjectEventHandler(SetProjectTitle);
			projectService.CombineOpened         += new CombineEventHandler(CombineOpened);

			fileService.FileRemoved += new FileEventHandler(CheckRemovedFile);
			fileService.FileRenamed += new FileEventHandler(CheckRenamedFile);
			
			fileService.RecentOpen.RecentFileChanged    += new EventHandler(UpdateMenu);
			fileService.RecentOpen.RecentProjectChanged += new EventHandler(UpdateMenu);
			
			fileService.FileRemoved += new FileEventHandler(fileService.RecentOpen.FileRemoved);
			fileService.FileRenamed += new FileEventHandler(fileService.RecentOpen.FileRenamed);
#endif
	
			//TopMenu.Selected += new CommandHandler(OnTopMenuSelected);
			//TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);
			View.CreateMainMenu();
			View.CreateToolBars();
		}
		
		
		
		public void CloseContent(IViewContent content)
		{
			if (propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				StoreMemento(content);
			}
			if (workbenchContentCollection.Contains(content)) 
			{
				workbenchContentCollection.Remove(content);
			}
			content.Dispose();
		}
		
		public void CloseAllViews()
		{
			try {
				closeAll = true;
				ViewContentCollection fullList = new ViewContentCollection(workbenchContentCollection);
				foreach (IViewContent content in fullList) {
					IWorkbenchWindow window = content.WorkbenchWindow;
					window.CloseWindow(false);
				}
			} finally {
				closeAll = false;
				OnActiveWindowChanged(null, null);
			}
		}
		
		public virtual void ShowView(IViewContent content)
		{
			Debug.Assert(layout != null);
			ViewContentCollection.Add(content);
			if (propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				try {
					IXmlConvertable memento = GetStoredMemento(content);
					if (memento != null) {
						((IMementoCapable)content).SetMemento(memento);
					}
				} catch (Exception e) {
					Console.WriteLine("Can't get/set memento : " + e.ToString());
				}
			}
			
			layout.ShowView(content);
			content.WorkbenchWindow.SelectWindow();
		}
		
		public virtual void ShowPad(IPadContent content)
		{
			PadContentCollection.Add(content);
						
			if (layout != null) {
				layout.ShowPad(content);
			}
		}
		
		public void RedrawAllComponents()
		{
			View.UpdateMenu(null, null);
			
			foreach (IViewContent content in workbenchContentCollection) {
				content.RedrawContent();
			}
			foreach (IPadContent content in viewContentCollection) {
				content.RedrawContent();
			}
			layout.RedrawAllComponents();
//			statusBarManager.RedrawStatusbar();
		}
		
		public IXmlConvertable GetStoredMemento(IViewContent content)
		{
			if (content != null && content.ContentName != null) {
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string directory = propertyService.ConfigDirectory + "temp";
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
				string fullFileName = directory + Path.DirectorySeparatorChar + fileName;
				// check the file name length because it could be more than the maximum length of a file name
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (fileUtilityService.IsValidFileName(fullFileName) && File.Exists(fullFileName)) {
					IXmlConvertable prototype = ((IMementoCapable)content).CreateMemento();
					XmlDocument doc = new XmlDocument();
					doc.Load(fullFileName);
					
					return (IXmlConvertable)prototype.FromXmlElement((XmlElement)doc.DocumentElement.ChildNodes[0]);
				}
			}
			return null;
		}
		
		public void StoreMemento(IViewContent content)
		{
			if (content.ContentName == null) {
				return;
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "temp";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<Mementoable/>");
			
			XmlAttribute fileAttribute = doc.CreateAttribute("file");
			fileAttribute.InnerText = content.ContentName;
			doc.DocumentElement.Attributes.Append(fileAttribute);
			
			
			IXmlConvertable memento = ((IMementoCapable)content).CreateMemento();
			
			doc.DocumentElement.AppendChild(memento.ToXmlElement(doc));
			
			string fileName = content.ContentName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			// check the file name length because it could be more than the maximum length of a file name
			string fullFileName = directory + Path.DirectorySeparatorChar + fileName;
			if (fileUtilityService.IsValidFileName(fullFileName)) {
				fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fullFileName, FileErrorPolicy.ProvideAlternative);
			}
		}
		
		// interface IMementoCapable
		public IXmlConvertable CreateMemento()
		{
			WorkbenchMemento memento   = new WorkbenchMemento();
			memento.Bounds             = normalBounds;
			memento.DefaultWindowState = fullscreen ? defaultWindowState : View.WindowState;
			memento.WindowState        = View.WindowState;
			memento.FullScreen         = fullscreen;
			return memento;
		}
		
		public void SetMemento(IXmlConvertable xmlMemento)
		{
			if (xmlMemento != null) {
				WorkbenchMemento memento = (WorkbenchMemento)xmlMemento;
				
				View.Bounds      = normalBounds = memento.Bounds;
				View.WindowState = memento.WindowState;
				defaultWindowState = memento.DefaultWindowState;
				FullScreen  = memento.FullScreen;
			}
		}
		
		
		
		
//		protected void OnTopMenuSelected(MenuCommand mc)
//		{
//			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
//			statusBarService.SetMessage(mc.Description);
//		}
		
//		protected void OnTopMenuDeselected(MenuCommand mc)
//		{
//			SetStandardStatusBar(null, null);
//		}
		
		
		
		

		void SetStandardStatusBar(object sender, EventArgs e)
		{
#if LellidMod
#else
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
#endif
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (!closeAll && ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		
		
		

		public IPadContent GetPad(Type type)
		{
			foreach (IPadContent pad in PadContentCollection) {
				if (pad.GetType() == type) {
					return pad;
				}
			}
			return null;
		}

		
		
		
		
		public void UpdateViews(object sender, EventArgs e)
		{
			IPadContent[] contents = (IPadContent[])(AddInTreeSingleton.AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this)).ToArray(typeof(IPadContent));
			foreach (IPadContent content in contents) {
				ShowPad(content);
			}
		}
		
		
		
			
		public event EventHandler ActiveWorkbenchWindowChanged;
		#region IMainController Members


		/// <summary>
		/// Das Wichtigste - das eigentliche Dokument
		/// </summary>
		public AltaxoDocument m_Doc=null;

		private System.Windows.Forms.PageSetupDialog m_PageSetupDialog;

		private System.Drawing.Printing.PrintDocument m_PrintDocument;

		private System.Windows.Forms.PrintDialog m_PrintDialog;


		/// <summary>
		/// Flag that indicates that the Application is about to be closed.
		/// </summary>
		private bool m_ApplicationIsClosing;

		#region Properties
		public  System.Windows.Forms.PageSetupDialog PageSetupDialog
		{
			get { return m_PageSetupDialog; }
		}

		public  System.Drawing.Printing.PrintDocument PrintDocument
		{
			get { return m_PrintDocument; }
		}


		public System.Windows.Forms.PrintDialog PrintDialog
		{
			get { return m_PrintDialog; }
		}


		/// <summary>
		/// Indicates if true that the Application is about to be closed. Can be used by child forms to prevent the confirmation dialog that 
		/// normally appears also during close of the application, since the child windows also receive the closing message in this case.
		/// </summary>
		public bool IsClosing
		{
			get { return this.m_ApplicationIsClosing; }
		}

		#endregion

		public Altaxo.Graph.GUI.IGraphController CreateNewGraph()
		{
			return CreateNewGraph(Doc.CreateNewGraphDocument());
		}

	

		public Altaxo.Graph.GUI.IGraphController CreateNewGraph(Altaxo.Graph.GraphDocument graph)
		{
			//Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			//Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm(this.View.Form);
			//wbv_controller.View = wbvform;

			if(graph==null)
				graph = this.Doc.CreateNewGraphDocument();

			Altaxo.Graph.GUI.GraphController ctrl = new Altaxo.Graph.GUI.GraphController(graph);
			Altaxo.Graph.GUI.GraphView view = new Altaxo.Graph.GUI.GraphView();
			ctrl.View = view;

			this.workbenchContentCollection.Add((IViewContent)ctrl);

      if(this.layout!=null)
				this.layout.ShowView(ctrl);
			
			//wbv_controller.Content = ctrl;

			//wbvform.Show();
			return ctrl;
		}

	

		Altaxo.IMainView Altaxo.IMainController.View
		{
			get
			{
				// TODO:  Add BeautyWorkbench.Altaxo.IMainController.View getter implementation
				return (Altaxo.IMainView) this.m_View;
			}
		}

		public Altaxo.Worksheet.GUI.IWorksheetController CreateNewWorksheet(Altaxo.Data.DataTable table)
		{
			//Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			//Altaxo.Main.GUI.BeautyWorkspaceWindow wbvform = new Altaxo.Main.GUI.BeautyWorkspaceWindow(this.View.Form);
			//wbv_controller.View = wbvform;

			Altaxo.Worksheet.GUI.WorksheetController ctrl = new Altaxo.Worksheet.GUI.WorksheetController(this.Doc.CreateNewTableLayout(table));
			Altaxo.Worksheet.GUI.WorksheetView view = new Altaxo.Worksheet.GUI.WorksheetView();
			ctrl.View = view;

			this.workbenchContentCollection.Add((IViewContent)ctrl);

			if(this.layout!=null)
				this.layout.ShowView(ctrl);
			

			//wbv_controller.Content = ctrl;
			
			//wbvform.Show();
			return ctrl;
		}

		public void EhView_Closing(CancelEventArgs e)
		{
			// TODO:  Add BeautyWorkbench.EhView_Closing implementation
		}

		public void EhView_Closed(EventArgs e)
		{
			// TODO:  Add BeautyWorkbench.EhView_Closed implementation
		}

		/// <summary>This will remove the GraphController <paramref>ctrl</paramref> from the graph forms collection.</summary>
		/// <param name="ctrl">The GraphController to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the graph forms collection.</remarks>
		public void RemoveGraph(Altaxo.Graph.GUI.GraphController ctrl)
		{
			if(this.workbenchContentCollection.Contains((IViewContent)ctrl))
				this.workbenchContentCollection.Remove((IViewContent)ctrl);
			else if(ctrl.ParentWorkbenchWindowController !=null && this.workbenchContentCollection.Contains((IViewContent)ctrl.ParentWorkbenchWindowController))
				this.workbenchContentCollection.Remove((IViewContent)ctrl.ParentWorkbenchWindowController);
		}

		

		/// <summary>This will remove the Worksheet <paramref>ctrl</paramref> from the corresponding forms collection.</summary>
		/// <param name="ctrl">The Worksheet to remove.</param>
		/// <remarks>No exception is thrown if the Form frm is not a member of the worksheet forms collection.</remarks>
		public void RemoveWorksheet(Altaxo.Worksheet.GUI.WorksheetController ctrl)
		{
			if(this.workbenchContentCollection.Contains((IViewContent)ctrl))
				this.workbenchContentCollection.Remove((IViewContent)ctrl);
			else if(ctrl.ParentWorkbenchWindowController !=null && this.workbenchContentCollection.Contains((IViewContent)ctrl.ParentWorkbenchWindowController))
				this.workbenchContentCollection.Remove((IViewContent)ctrl.ParentWorkbenchWindowController);
		}


		public void EhView_CloseMessage()
		{
			// TODO:  Add BeautyWorkbench.EhView_CloseMessage implementation
		}

		/// <summary>
		/// The document which is visualized by the controller, contains all data tables, graph, worksheet views and graph views
		/// </summary>
		public AltaxoDocument Doc
		{
			get	{	return m_Doc; }
		}


		#endregion
	}
}
