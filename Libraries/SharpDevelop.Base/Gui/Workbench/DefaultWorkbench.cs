// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krueger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Services;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// This is the a Workspace with a multiple document interface.
	/// </summary>
	public class DefaultWorkbench : Form, IWorkbench
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
		
		public bool FullScreen {
			get {
				return fullscreen;
			}
			set {
				fullscreen = value;
				if (fullscreen) {
					defaultWindowState = WindowState;
					// - Hide window to prevet any further animations.
					// - It fixes .NET Framework bug where the bounds of
					//   visible window are set incorectly too.
					Visible            = false;
					FormBorderStyle    = FormBorderStyle.None;
					WindowState        = FormWindowState.Maximized;
					Visible            = true;
				} else {
					FormBorderStyle = FormBorderStyle.Sizable;
					Bounds          = normalBounds;
					WindowState     = defaultWindowState;
				}
			}
		}
		
		public string Title {
			get {
				return Text;
			}
			set {
				Text = value;
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
				System.Diagnostics.Debug.Assert(viewContentCollection != null);
				return viewContentCollection;
			}
		}
		
		public ViewContentCollection ViewContentCollection {
			get {
				System.Diagnostics.Debug.Assert(workbenchContentCollection != null);
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
		
		public DefaultWorkbench()
		{
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			Text = resourceService.GetString("MainWindow.DialogName");
			Icon = resourceService.GetIcon("Icons.SharpDevelopIcon");
			
			windowChangeEventHandler = new EventHandler(OnActiveWindowChanged);
			
			StartPosition = FormStartPosition.Manual;
			
			AllowDrop      = true;
		}
		
		public void InitializeWorkspace()
		{
			Menu = null;
			
			//			statusBarManager.Control.Dock = DockStyle.Bottom;
			
			ActiveWorkbenchWindowChanged += new EventHandler(UpdateMenu);
			
			MenuComplete += new EventHandler(SetStandardStatusBar);
			SetStandardStatusBar(null, null);
			
//			TopMenu.Selected   += new CommandHandler(OnTopMenuSelected);
//			TopMenu.Deselected += new CommandHandler(OnTopMenuDeselected);
			CreateMainMenu();
			CreateToolBars();
		}
		
		public void CloseContent(IViewContent content)
		{
			if (propertyService.GetProperty("SharpDevelop.LoadDocumentProperties", true) && content is IMementoCapable) {
				StoreMemento(content);
			}
			if (ViewContentCollection.Contains(content)) {
				ViewContentCollection.Remove(content);
			}
			OnViewClosed(new ViewContentEventArgs(content));
			content.Dispose();
			content = null;
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
			System.Diagnostics.Debug.Assert(layout != null);
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
			OnViewOpened(new ViewContentEventArgs(content));
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
			UpdateMenu(null, null);
			
			foreach (IViewContent content in workbenchContentCollection) {
				content.RedrawContent();
				if (content.WorkbenchWindow != null) {
					content.WorkbenchWindow.RedrawContent();
				}
			}
			foreach (IPadContent content in viewContentCollection) {
				content.RedrawContent();
			}
			layout.RedrawAllComponents();
//			statusBarManager.RedrawStatusbar();
		}
		
		public IXmlConvertable GetStoredMemento(IViewContent content)
		{
			if (content != null && content.FileName != null) {
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				string directory = propertyService.ConfigDirectory + "temp";
				if (!Directory.Exists(directory)) {
					Directory.CreateDirectory(directory);
				}
				string fileName = content.FileName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
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
			if (content.FileName == null) {
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
			fileAttribute.InnerText = content.FileName;
			doc.DocumentElement.Attributes.Append(fileAttribute);
			
			
			IXmlConvertable memento = ((IMementoCapable)content).CreateMemento();
			
			doc.DocumentElement.AppendChild(memento.ToXmlElement(doc));
			
			string fileName = content.FileName.Substring(3).Replace('/', '.').Replace('\\', '.').Replace(Path.DirectorySeparatorChar, '.');
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			// check the file name length because it could be more than the maximum length of a file name
			string fullFileName = directory + Path.DirectorySeparatorChar + fileName;
			if (fileUtilityService.IsValidFileName(fullFileName)) {
				fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fullFileName, FileErrorPolicy.ProvideAlternative);
			}
			doc.RemoveAll();
		}
		
		// interface IMementoCapable
		public IXmlConvertable CreateMemento()
		{
			WorkbenchMemento memento   = new WorkbenchMemento();
			memento.Bounds             = normalBounds;
			memento.DefaultWindowState = fullscreen ? defaultWindowState : WindowState;
			memento.WindowState        = WindowState;
			memento.FullScreen         = fullscreen;
			return memento;
		}
		
		public void SetMemento(IXmlConvertable xmlMemento)
		{
			if (xmlMemento != null) {
				WorkbenchMemento memento = (WorkbenchMemento)xmlMemento;
				
				Bounds      = normalBounds = memento.Bounds;
				WindowState = memento.WindowState;
				defaultWindowState = memento.DefaultWindowState;
				FullScreen  = memento.FullScreen;
			}
		}
		
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			if (WindowState == FormWindowState.Normal) {
				normalBounds = Bounds;
			}
			
		}
		
		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			if (WindowState == FormWindowState.Normal) {
				normalBounds = Bounds;
			}
		}
		
//		protected void OnTopMenuSelected(MenuCommand mc)
//		{
//			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
//			
//			statusBarService.SetMessage(mc.Description);
//		}
//		
//		protected void OnTopMenuDeselected(MenuCommand mc)
//		{
//			SetStandardStatusBar(null, null);
//		}
		
		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);
			
		}
		
		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			layout.Detach();
			foreach (IPadContent content in PadContentCollection) {
				content.Dispose();
			}
		}
		
		void SetStandardStatusBar(object sender, EventArgs e)
		{
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (!closeAll && ActiveWorkbenchWindowChanged != null) {
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		public CommandBarManager commandBarManager = new CommandBarManager();
		public CommandBar   TopMenu  = null;
		public CommandBar[] ToolBars = null;
		
		public IPadContent GetPad(Type type)
		{
			foreach (IPadContent pad in PadContentCollection) {
				if (pad.GetType() == type) {
					return pad;
				}
			}
			return null;
		}
		void CreateMainMenu()
		{
			TopMenu = new CommandBar(CommandBarStyle.Menu);
			try {
				IAddInTreeNode node = AddInTreeSingleton.AddInTree.GetTreeNode(mainMenuPath);
				CommandBarItem[] items = (CommandBarItem[])(node.BuildChildItems(this)).ToArray(typeof(CommandBarItem));
				TopMenu.Items.Clear();
				TopMenu.Items.AddRange(items);
			} catch (TreePathNotFoundException) {
			}
		}
		
		void UpdateMenu(object sender, EventArgs e)
		{
			UpdateMenus();
			UpdateToolbars();
		}
		
		public void UpdateMenus()
		{
			// update menu
			foreach (object o in TopMenu.Items) {
				if (o is IStatusUpdate) {
					((IStatusUpdate)o).UpdateStatus();
				}
			}
		}
		
		public void UpdateToolbars()
		{
			foreach (CommandBar commandBar in ToolBars) {
				foreach (object item in commandBar.Items) {
					if (item is IStatusUpdate) {
						((IStatusUpdate)item).UpdateStatus();
					}
				}
			}
		}
		
		// this method simply copies over the enabled state of the toolbar,
		// this assumes that no item is inserted or removed.
		// TODO : make this method more add-in tree like, currently with Windows.Forms
		//        toolbars this is not possible. (drawing fragments, slow etc.)
		void CreateToolBars()
		{
			if (ToolBars == null) {
				ToolbarService toolBarService = (ToolbarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(ToolbarService));
				CommandBar[] toolBars = toolBarService.CreateToolbars();
				ToolBars = toolBars;
			}
		}
		
		public void UpdatePadContents(object sender, EventArgs e)
		{
			try {
				IPadContent[] contents = (IPadContent[])(AddInTreeSingleton.AddInTree.GetTreeNode(viewContentPath).BuildChildItems(this)).ToArray(typeof(IPadContent));
				foreach (IPadContent content in contents) {
					ShowPad(content);
				}
			}  catch (TreePathNotFoundException) {
			}
		}
			// Handle keyboard shortcuts
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (this.commandBarManager.PreProcessMessage(ref msg)) {
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
		
		protected override void OnDragEnter(DragEventArgs e)
		{
			base.OnDragEnter(e);
			if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop)) {
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
				foreach (string file in files) {
					if (File.Exists(file)) {
						e.Effect = DragDropEffects.Copy;
						return;
					}
				}
			}
			e.Effect = DragDropEffects.None;
		}
		
		protected override void OnDragDrop(DragEventArgs e)
		{
			base.OnDragDrop(e);
			// TODO: D&D events
		}

		protected virtual void OnViewOpened(ViewContentEventArgs e)
		{
			if (ViewOpened != null) {
				ViewOpened(this, e);
			}
		}
		
		protected virtual void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
		
		public event ViewContentEventHandler ViewOpened;
		public event ViewContentEventHandler ViewClosed;
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
