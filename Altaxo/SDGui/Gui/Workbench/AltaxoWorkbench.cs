// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using Altaxo.Geometry;
using ICSharpCode.Core;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.WinForms;
using ICSharpCode.SharpDevelop.Workbench;

using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Workbench implementation using WPF and AvalonDock.
	/// </summary>
	partial class AltaxoWorkbench : FullScreenEnabledWindow, IWorkbench, System.Windows.Forms.IWin32Window, Altaxo.Gui.Common.IWorkbench
	{
		private const string mainMenuPath = "/Altaxo/Workbench/MainMenu";
		private const string viewContentPath = "/Altaxo/Workbench/Pads";
		private const string toolBarPath = "/Altaxo/Workbench/ToolBar";

		public event EventHandler ActiveWorkbenchWindowChanged;

		public event EventHandler ActiveViewContentChanged;

		public event EventHandler ActiveContentChanged;

		public event EventHandler<ViewContentEventArgs> ViewOpened;

		public event EventHandler<ViewContentEventArgs> ViewClosed;

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "ICSharpCode.SharpDevelop.Gui.Workbench1", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoWorkbench), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AltaxoWorkbench s = (AltaxoWorkbench)obj;
			}

			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return o;
			}
		}

		#endregion "Serialization"

		internal void OnViewOpened(ViewContentEventArgs e)
		{
			ViewOpened?.Invoke(this, e);
		}

		internal void OnViewClosed(ViewContentEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, e);
			}
		}

		public System.Windows.Forms.IWin32Window MainWin32Window { get { return this; } }
		public Window MainWindow { get { return this; } }

		IntPtr System.Windows.Forms.IWin32Window.Handle
		{
			get
			{
				if (System.Windows.PresentationSource.FromVisual(this) is System.Windows.Interop.IWin32Window wnd)
					return wnd.Handle;
				else
					return IntPtr.Zero;
			}
		}

		private List<PadDescriptor> padDescriptorCollection = new List<PadDescriptor>();
		private SDStatusBar statusBar = new SDStatusBar();
		private ToolBar[] toolBars;

		public AltaxoWorkbench()
		{
			this.SynchronizingObject = new WpfSynchronizeInvoke(this.Dispatcher);
			SD.Services.AddService(typeof(IStatusBarService), new StatusBarService(statusBar));
			InitializeComponent();
			InitFocusTrackingEvents();

			// if the application was started as a COM server, hide the window until there is an explicite COM command to show the window
			if (Current.ComManager != null && Current.ComManager.ApplicationWasStartedWithEmbeddingArg)
			{
				this.Visibility = System.Windows.Visibility.Collapsed;
				this.ShowInTaskbar = false;
			}
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);

			// TODO: why is next line neccessary
			// HwndSource.FromHwnd(this.MainWin32Window.Handle).AddHook(SingleInstanceHelper.WndProc);

			// validate after PresentationSource is initialized
			Rect bounds = new Rect(Left, Top, Width, Height);
			bounds = FormLocationHelper.Validate(bounds.TransformToDevice(this).ToSystemDrawing()).ToWpf().TransformFromDevice(this);
			SetBounds(bounds);
			// Set WindowState after PresentationSource is initialized, because now bounds and location are properly set.
			this.WindowState = lastNonMinimizedWindowState;
		}

		private void SetBounds(Rect bounds)
		{
			this.Left = bounds.Left;
			this.Top = bounds.Top;
			this.Width = bounds.Width;
			this.Height = bounds.Height;
		}

		public void Initialize()
		{
			UpdateFlowDirection();

			var padDescriptors = AddInTree.BuildItems<PadDescriptor>(viewContentPath, this, false);
			((SharpDevelopServiceContainer)SD.Services).AddFallbackProvider(new PadServiceProvider(padDescriptors));
			foreach (PadDescriptor content in padDescriptors)
			{
				ShowPad(content);
			}

			mainMenu.ItemsSource = MenuService.CreateMenuItems(this, this, mainMenuPath, activationMethod: "MainMenu", immediatelyExpandMenuBuildersForShortcuts: true);

			toolBars = ToolBarService.CreateToolBars(this, this, toolBarPath);
			foreach (ToolBar tb in toolBars)
			{
				DockPanel.SetDock(tb, Dock.Top);
				dockPanel.Children.Insert(1, tb);
			}
			DockPanel.SetDock(statusBar, Dock.Bottom);
			dockPanel.Children.Insert(dockPanel.Children.Count - 2, statusBar);

			// TODO ModifiedForAltaxo the following two lines where outcommented
			//Core.WinForms.MenuService.ExecuteCommand = ExecuteCommand;
			//Core.WinForms.MenuService.CanExecuteCommand = CanExecuteCommand;

			UpdateMenu();

			AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));

			ICSharpCode.SharpDevelop.FileService.FileRemoved += CheckRemovedOrReplacedFile;
			ICSharpCode.SharpDevelop.FileService.FileReplaced += CheckRemovedOrReplacedFile;
			ICSharpCode.SharpDevelop.FileService.FileRenamed += CheckRenamedFile;

			ICSharpCode.SharpDevelop.FileService.FileRemoved += ((RecentOpen)SD.FileService.RecentOpen).FileRemoved;
			ICSharpCode.SharpDevelop.FileService.FileRenamed += ((RecentOpen)SD.FileService.RecentOpen).FileRenamed;

			requerySuggestedEventHandler = new EventHandler(CommandManager_RequerySuggested);
			CommandManager.RequerySuggested += requerySuggestedEventHandler;
			SD.ResourceService.LanguageChanged += OnLanguageChanged;

			SD.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}

		private void ExecuteCommand(ICommand command, object caller)
		{
			ServiceSingleton.GetRequiredService<IAnalyticsMonitor>()
				.TrackFeature(command.GetType().FullName, "Menu");
			var routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				var target = FocusManager.GetFocusedElement(this);
				if (routedCommand.CanExecute(caller, target))
					routedCommand.Execute(caller, target);
			}
			else
			{
				if (command.CanExecute(caller))
					command.Execute(caller);
			}
		}

		private bool CanExecuteCommand(ICommand command, object caller)
		{
			var routedCommand = command as RoutedCommand;
			if (routedCommand != null)
			{
				var target = FocusManager.GetFocusedElement(this);
				return routedCommand.CanExecute(caller, target);
			}
			else
			{
				return command.CanExecute(caller);
			}
		}

		// keep a reference to the event handler to prevent it from being garbage collected
		// (CommandManager.RequerySuggested only keeps weak references to the event handlers)
		private EventHandler requerySuggestedEventHandler;

		private void CommandManager_RequerySuggested(object sender, EventArgs e)
		{
			UpdateMenu();
		}

		private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			e.Handled = true;
			if (e.Uri.Scheme == "mailto")
			{
				try
				{
					Process.Start(e.Uri.ToString());
				}
				catch
				{
					// catch exceptions - e.g. incorrectly installed mail client
				}
			}
			else
			{
				ICSharpCode.SharpDevelop.FileService.OpenFile(e.Uri.ToString());
			}
		}

		private void CheckRemovedOrReplacedFile(object sender, FileEventArgs e)
		{
			foreach (OpenedFile file in SD.FileService.OpenedFiles)
			{
				if (FileUtility.IsBaseDirectory(e.FileName, file.FileName))
				{
					foreach (IViewContent content in file.RegisteredViewContents.ToArray())
					{
						// content.WorkbenchWindow can be null if multiple view contents
						// were in the same WorkbenchWindow and both should be closed
						// (e.g. Windows Forms Designer, Subversion History View)
						if (content.WorkbenchWindow != null)
						{
							content.WorkbenchWindow.CloseWindow(true);
						}
					}
				}
			}
			//Editor.PermanentAnchorService.FileDeleted(e);
		}

		private void CheckRenamedFile(object sender, FileRenameEventArgs e)
		{
			if (e.IsDirectory)
			{
				foreach (OpenedFile file in SD.FileService.OpenedFiles)
				{
					if (file.FileName != null && FileUtility.IsBaseDirectory(e.SourceFile, file.FileName))
					{
						file.FileName = new FileName(FileUtility.RenameBaseDirectory(file.FileName, e.SourceFile, e.TargetFile));
					}
				}
			}
			else
			{
				OpenedFile file = SD.FileService.GetOpenedFile(e.SourceFile);
				if (file != null)
				{
					file.FileName = new FileName(e.TargetFile);
				}
			}
			//Editor.PermanentAnchorService.FileRenamed(e);
		}

		private void UpdateMenu()
		{
			MenuService.UpdateStatus(mainMenu.ItemsSource);
			foreach (ToolBar tb in toolBars)
			{
				ToolBarService.UpdateStatus(tb.ItemsSource);
			}
		}

		private void OnLanguageChanged(object sender, EventArgs e)
		{
			MenuService.UpdateText(mainMenu.ItemsSource);
			UpdateFlowDirection();
		}

		private void UpdateFlowDirection()
		{
			/*
			UILanguage language = UILanguageService.GetLanguage(ResourceService.Language);
			Core.WinForms.RightToLeftConverter.IsRightToLeft = language.IsRightToLeft;
			this.FlowDirection = language.IsRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
			App.Current.Resources[GlobalStyles.FlowDirectionKey] = this.FlowDirection;
			*/
		}

		public ICollection<IViewContent> ViewContentCollection
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return WorkbenchWindowCollection.SelectMany(w => w.ViewContents).ToList().AsReadOnly();
			}
		}

		public ICollection<IViewContent> PrimaryViewContents
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return (from window in WorkbenchWindowCollection
								where window.ViewContents.Count > 0
								select window.ViewContents[0]
				).ToList().AsReadOnly();
			}
		}

		public IList<IWorkbenchWindow> WorkbenchWindowCollection
		{
			get
			{
				SD.MainThread.VerifyAccess();
				if (workbenchLayout != null)
					return workbenchLayout.WorkbenchWindows;
				else
					return new IWorkbenchWindow[0];
			}
		}

		public IList<PadDescriptor> PadContentCollection
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return padDescriptorCollection.AsReadOnly();
			}
		}

		private IWorkbenchWindow activeWorkbenchWindow;

		public IWorkbenchWindow ActiveWorkbenchWindow
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return activeWorkbenchWindow;
			}
			private set
			{
				if (activeWorkbenchWindow != value)
				{
					if (activeWorkbenchWindow != null)
					{
						activeWorkbenchWindow.ActiveViewContentChanged -= WorkbenchWindowActiveViewContentChanged;
					}

					activeWorkbenchWindow = value;

					if (value != null)
					{
						value.ActiveViewContentChanged += WorkbenchWindowActiveViewContentChanged;
					}

					if (ActiveWorkbenchWindowChanged != null)
					{
						ActiveWorkbenchWindowChanged(this, EventArgs.Empty);
					}
					WorkbenchWindowActiveViewContentChanged(null, null);
				}
			}
		}

		private void WorkbenchWindowActiveViewContentChanged(object sender, EventArgs e)
		{
			if (workbenchLayout != null)
			{
				// update ActiveViewContent
				IWorkbenchWindow window = this.ActiveWorkbenchWindow;
				if (window != null)
					this.ActiveViewContent = window.ActiveViewContent;
				else
					this.ActiveViewContent = null;

				// update ActiveContent
				this.ActiveContent = workbenchLayout.ActiveContent;
			}
		}

		private bool activeWindowWasChanged;

		private void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (activeWindowWasChanged)
				return;
			activeWindowWasChanged = true;
			Dispatcher.BeginInvoke(new Action(
				delegate
				{
					activeWindowWasChanged = false;
					if (workbenchLayout != null)
					{
						this.ActiveContent = workbenchLayout.ActiveContent;
						this.ActiveWorkbenchWindow = workbenchLayout.ActiveWorkbenchWindow;
					}
					else
					{
						this.ActiveContent = null;
						this.ActiveWorkbenchWindow = null;
					}
				}));
		}

		private IViewContent activeViewContent;

		public IViewContent ActiveViewContent
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return activeViewContent;
			}
			private set
			{
				if (activeViewContent != value)
				{
					activeViewContent = value;

					if (ActiveViewContentChanged != null)
					{
						ActiveViewContentChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		private IServiceProvider activeContent;

		public IServiceProvider ActiveContent
		{
			get
			{
				SD.MainThread.VerifyAccess();
				return activeContent;
			}
			private set
			{
				if (activeContent != value)
				{
					activeContent = value;

					if (ActiveContentChanged != null)
					{
						ActiveContentChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		private IWorkbenchLayout workbenchLayout;

		public IWorkbenchLayout WorkbenchLayout
		{
			get
			{
				return workbenchLayout;
			}
			set
			{
				SD.MainThread.VerifyAccess();

				if (workbenchLayout != null)
				{
					workbenchLayout.ActiveContentChanged -= OnActiveWindowChanged;
					workbenchLayout.Detach();
				}
				if (value != null)
				{
					value.Attach(this);
					value.ActiveContentChanged += OnActiveWindowChanged;
				}
				workbenchLayout = value;
				OnActiveWindowChanged(null, null);
			}
		}

		public bool IsActiveWindow
		{
			get
			{
				return IsActive;
			}
		}

		public void ShowView(IViewContent content)
		{
			ShowView(content, true);
		}

		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			SD.MainThread.VerifyAccess();
			if (content == null)
				throw new ArgumentNullException("content");
			if (ViewContentCollection.Contains(content))
				throw new ArgumentException("ViewContent was already shown");
			System.Diagnostics.Debug.Assert(WorkbenchLayout != null);

			LoadViewContentMemento(content);

			WorkbenchLayout.ShowView(content, switchToOpenedView);
		}

		public void ShowPad(PadDescriptor content)
		{
			SD.MainThread.VerifyAccess();
			if (content == null)
				throw new ArgumentNullException("content");
			if (padDescriptorCollection.Contains(content))
				throw new ArgumentException("Pad is already loaded");

			padDescriptorCollection.Add(content);

			if (WorkbenchLayout != null)
			{
				WorkbenchLayout.ShowPad(content);
			}
		}

		public PadDescriptor GetPad(Type type)
		{
			SD.MainThread.VerifyAccess();
			if (type == null)
				throw new ArgumentNullException("type");
			foreach (PadDescriptor pad in PadContentCollection)
			{
				if (pad.Class == type.FullName)
				{
					return pad;
				}
			}
			return null;
		}

		public void CloseAllViews()
		{
			SD.MainThread.VerifyAccess();
			foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection.ToArray())
			{
				window.CloseWindow(false);
			}
		}

		public bool CloseAllSolutionViews(bool force)
		{
			bool result = true;
			foreach (IWorkbenchWindow window in this.WorkbenchWindowCollection.ToArray())
			{
				if (window.ActiveViewContent != null && window.ActiveViewContent.CloseWithSolution)
					result &= window.CloseWindow(force);
			}
			return result;
		}

		#region ViewContent Memento Handling

		private FileName viewContentMementosFileName;

		private FileName ViewContentMementosFileName
		{
			get
			{
				if (viewContentMementosFileName == null)
				{
					viewContentMementosFileName = SD.PropertyService.ConfigDirectory.CombineFile("LastViewStates.xml");
				}
				return viewContentMementosFileName;
			}
		}

		private Properties LoadOrCreateViewContentMementos()
		{
			try
			{
				return Properties.Load(this.ViewContentMementosFileName) ?? new Properties();
			}
			catch (Exception ex)
			{
				LoggingService.Warn("Error while loading the view content memento file. Discarding any saved view states.", ex);
				return new Properties();
			}
		}

		private static string GetMementoKeyName(IViewContent viewContent)
		{
			return String.Concat(viewContent.GetType().FullName.GetHashCode().ToString("x", CultureInfo.InvariantCulture), ":", FileUtility.NormalizePath(viewContent.PrimaryFileName).ToUpperInvariant());
		}

		public static bool LoadDocumentProperties
		{
			get { return SD.PropertyService.Get("SharpDevelop.LoadDocumentProperties", true); }
			set { SD.PropertyService.Set("SharpDevelop.LoadDocumentProperties", value); }
		}

		/// <summary>
		/// Stores the memento for the view content.
		/// Such mementos are automatically loaded in ShowView().
		/// </summary>
		public void StoreMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent.GetService<IMementoCapable>();
			if (mementoCapable != null && LoadDocumentProperties)
			{
				if (viewContent.PrimaryFileName == null)
					return;

				string key = GetMementoKeyName(viewContent);
				LoggingService.Debug("Saving memento of '" + viewContent.ToString() + "' to key '" + key + "'");

				Properties memento = mementoCapable.CreateMemento();
				Properties p = this.LoadOrCreateViewContentMementos();
				p.SetNestedProperties(key, memento);
				FileUtility.ObservedSave(new NamedFileOperationDelegate(p.Save), this.ViewContentMementosFileName, FileErrorPolicy.Inform);
			}
		}

		private void LoadViewContentMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent.GetService<IMementoCapable>();
			if (mementoCapable != null && LoadDocumentProperties)
			{
				if (viewContent.PrimaryFileName == null)
					return;

				try
				{
					string key = GetMementoKeyName(viewContent);
					LoggingService.Debug("Trying to restore memento of '" + viewContent.ToString() + "' from key '" + key + "'");

					mementoCapable.SetMemento(this.LoadOrCreateViewContentMementos().NestedProperties(key));
				}
				catch (Exception e)
				{
					MessageService.ShowException(e, "Can't get/set memento");
				}
			}
		}

		#endregion ViewContent Memento Handling

		private System.Windows.WindowState lastNonMinimizedWindowState = System.Windows.WindowState.Normal;
		private Rect restoreBoundsBeforeClosing;

		protected override void OnStateChanged(EventArgs e)
		{
			base.OnStateChanged(e);
			if (this.WindowState != System.Windows.WindowState.Minimized)
				lastNonMinimizedWindowState = this.WindowState;
		}

		public void StoreWorkbenchStateInPropertyService()
		{
			var bounds = this.RestoreBounds;
			if (bounds.IsEmpty)
				bounds = restoreBoundsBeforeClosing;

			var workbenchState = new WorkbenchState
			{
				WindowState = lastNonMinimizedWindowState,
				Bounds = new RectangleD2D(bounds.X, bounds.Y, bounds.Width, bounds.Height)
			};

			Current.PropertyService.SetValue(PropertyKeyWorkbenchState, workbenchState);
		}

		public void RestoreWorkbenchStateFromPropertyService()
		{
			var workbenchState = Current.PropertyService.GetValue(PropertyKeyWorkbenchState, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => null);
			Rect bounds = new Rect(10, 10, 750, 550);
			lastNonMinimizedWindowState = System.Windows.WindowState.Maximized;

			if (null != workbenchState)
			{
				if (!workbenchState.Bounds.IsEmpty)
					bounds = new Rect(workbenchState.Bounds.X, workbenchState.Bounds.Y, workbenchState.Bounds.Width, workbenchState.Bounds.Height);
				lastNonMinimizedWindowState = workbenchState.WindowState;
			}
			// bounds are validated after PresentationSource is initialized (see OnSourceInitialized)
			SetBounds(bounds);
		}

		#region Workbench state

		private static readonly Altaxo.Main.Properties.PropertyKey<WorkbenchState> PropertyKeyWorkbenchState =
		new Altaxo.Main.Properties.PropertyKey<WorkbenchState>(
			"F8B02744-B011-4A34-838D-ABFE5429CF59",
			"App\\WorkbenchState",
			Altaxo.Main.Properties.PropertyLevel.Application);

		public class WorkbenchState
		{
			public RectangleD2D Bounds { get; set; }

			public System.Windows.WindowState WindowState { get; set; }

			#region Serialization

			/// <summary>
			///
			/// </summary>
			[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(WorkbenchState), 0)]
			private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
			{
				public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
				{
					var s = (WorkbenchState)obj;

					info.AddValue("Bounds", s.Bounds);
					info.AddValue("WindowState", (int)s.WindowState);
				}

				public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
				{
					var s = o as WorkbenchState ?? new WorkbenchState();

					s.Bounds = (RectangleD2D)info.GetValue("Bounds", null);
					s.WindowState = (System.Windows.WindowState)info.GetInt32("WindowState");
					return s;
				}
			}

			#endregion Serialization
		}

		#endregion Workbench state

		protected override void OnClosing(CancelEventArgs e)
		{
			base.OnClosing(e);

			// Altaxo stuff
			Altaxo.Main.IProjectService projectService = Altaxo.Current.ProjectService;
			if (projectService != null && (Current.ComManager == null || Current.ComManager.IsInEmbeddedMode == false))
			{
				if (projectService.CurrentOpenProject != null && projectService.CurrentOpenProject.IsDirty)
				{
					projectService.AskForSavingOfProject(e);
				}
			}

			if (!e.Cancel)
			{
				// see IShutdownService.Shutdown() for a description of the shutdown procedure

				var shutdownService = (ShutdownService)SD.ShutdownService;
				if (shutdownService.CurrentReasonPreventingShutdown != null)
				{
					MessageService.ShowMessage(shutdownService.CurrentReasonPreventingShutdown);
					e.Cancel = true;
					return;
				}

				((AltaxoWorkbench)SD.Workbench).WorkbenchLayout.StoreConfiguration();
				restoreBoundsBeforeClosing = this.RestoreBounds;

				this.WorkbenchLayout = null;

				shutdownService.SignalShutdownToken();
				foreach (PadDescriptor padDescriptor in this.PadContentCollection)
				{
					padDescriptor.Dispose();
				}
			}

			if (!e.Cancel)
			{
				// Stop the Com-Server already here, it seems to be too late if we try to stop it in OnClosed()
				if (Current.ComManager is Altaxo.Com.ComManager comManager)
					comManager.StopLocalServer();
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			Altaxo.Main.IProjectService projectService = Altaxo.Current.ProjectService;
			projectService.DisposeProjectAndSetToNull();

			base.OnClosed(e);
		}

		protected override void OnDragEnter(DragEventArgs e)
		{
			try
			{
				base.OnDragEnter(e);
				if (!e.Handled)
				{
					e.Effects = GetEffect(e.Data);
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				MessageService.ShowException(ex);
			}
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			try
			{
				base.OnDragOver(e);
				if (!e.Handled)
				{
					e.Effects = GetEffect(e.Data);
					e.Handled = true;
				}
			}
			catch (Exception ex)
			{
				MessageService.ShowException(ex);
			}
		}

		private DragDropEffects GetEffect(IDataObject data)
		{
			try
			{
				if (data != null && data.GetDataPresent(DataFormats.FileDrop))
				{
					string[] files = (string[])data.GetData(DataFormats.FileDrop);
					if (files != null)
					{
						foreach (string file in files)
						{
							if (File.Exists(file))
							{
								return DragDropEffects.Link;
							}
						}
					}
				}
			}
			catch (COMException)
			{
				// Ignore errors getting the data (e.g. happens when dragging attachments out of Thunderbird)
			}
			return DragDropEffects.None;
		}

		protected override void OnDrop(DragEventArgs e)
		{
			try
			{
				base.OnDrop(e);
				if (!e.Handled && e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
				{
					e.Handled = true;
					string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
					if (files == null)
						return;
					// Handle opening the files outside the drop event, so that the drag source doesn't think
					// the operation is still in progress while we're showing a "file cannot be opened" error message.
					Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action<string[]>(HandleDrop), files);
				}
			}
			catch (Exception ex)
			{
				MessageService.ShowException(ex);
			}
		}

		private void HandleDrop(string[] files)
		{
			foreach (string file in files)
			{
				if (File.Exists(file))
				{
					var fileName = FileName.Create(file);

					if (Current.ProjectService.IsAltaxoProjectFileExtension(System.IO.Path.GetExtension(file)))
					{
						Current.ProjectService.OpenProject(file, false);
					}
					else
					{
						SD.FileService.OpenFile(fileName);
					}
				}
			}
		}

		private void InitFocusTrackingEvents()
		{
#if DEBUG
			this.PreviewLostKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewLostKeyboardFocus);
			this.PreviewGotKeyboardFocus += new KeyboardFocusChangedEventHandler(WpfWorkbench_PreviewGotKeyboardFocus);
#endif
		}

		[Conditional("DEBUG")]
		internal static void FocusDebug(string format, params object[] args)
		{
#if DEBUG
			if (enableFocusDebugOutput)
				LoggingService.DebugFormatted(format, args);
#endif
		}

#if DEBUG
		private static bool enableFocusDebugOutput;

		private void WpfWorkbench_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("GotKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}

		private void WpfWorkbench_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			FocusDebug("LostKeyboardFocus: oldFocus={0}, newFocus={1}", e.OldFocus, e.NewFocus);
		}

		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{
			base.OnPreviewKeyDown(e);
			if (!e.Handled && e.Key == Key.D && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
			{
				enableFocusDebugOutput = !enableFocusDebugOutput;

				StringWriter output = new StringWriter();
				output.WriteLine("Keyboard.FocusedElement = " + GetElementName(Keyboard.FocusedElement));
				output.WriteLine("ActiveContent = " + GetElementName(this.ActiveContent));
				output.WriteLine("ActiveViewContent = " + GetElementName(this.ActiveViewContent));
				output.WriteLine("ActiveWorkbenchWindow = " + GetElementName(this.ActiveWorkbenchWindow));
				((AvalonDockLayout)workbenchLayout).WriteState(output);
				LoggingService.Debug(output.ToString());
				e.Handled = true;
			}
			if (!e.Handled && e.Key == Key.F && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
			{
				if (TextOptions.GetTextFormattingMode(this) == TextFormattingMode.Display)
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
				else
					TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
				SD.StatusBar.SetMessage("TextFormattingMode=" + TextOptions.GetTextFormattingMode(this));
			}
			if (!e.Handled && e.Key == Key.R && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
			{
				switch (TextOptions.GetTextRenderingMode(this))
				{
					case TextRenderingMode.Auto:
					case TextRenderingMode.ClearType:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Grayscale);
						break;

					case TextRenderingMode.Grayscale:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.Aliased);
						break;

					default:
						TextOptions.SetTextRenderingMode(this, TextRenderingMode.ClearType);
						break;
				}
				SD.StatusBar.SetMessage("TextRenderingMode=" + TextOptions.GetTextRenderingMode(this));
			}
			if (!e.Handled && e.Key == Key.G && e.KeyboardDevice.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt))
			{
				GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
				SD.StatusBar.SetMessage("Total memory = " + (GC.GetTotalMemory(true) / 1024 / 1024f).ToString("f1") + " MB");
			}
		}

#endif

		internal static string GetElementName(object element)
		{
			if (element == null)
				return "<null>";
			else
				return element.GetType().FullName + ": " + element.ToString();
		}

		public string CurrentLayoutConfiguration
		{
			get
			{
				return LayoutConfiguration.CurrentLayoutName;
			}
			set
			{
				LayoutConfiguration.CurrentLayoutName = value;
			}
		}

		public void ActivatePad(PadDescriptor content)
		{
			if (workbenchLayout != null)
				workbenchLayout.ActivatePad(content);
		}

		#region Altaxo.Gui.Common.IWorkbench Members

		public object ViewObject
		{
			get
			{
				return this;
			}
		}

		object Altaxo.Gui.Common.IWorkbench.ActiveViewContent
		{
			get { return ActiveWorkbenchWindow?.ActiveViewContent; }
		}

		System.Collections.ICollection Altaxo.Gui.Common.IWorkbench.ViewContentCollection
		{
			get { return (System.Collections.ICollection)ViewContentCollection; }
		}

		void Altaxo.Gui.Common.IWorkbench.ShowView(object content)
		{
			this.ShowView((IViewContent)content);
		}

		public void CloseContent(object content)
		{
			((IViewContent)content).WorkbenchWindow.CloseWindow(true);
		}

		public bool CloseAllSolutionViews()
		{
			this.CloseAllViews();
			return true;
		}

		public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			Current.Gui.Execute(EhProjectChanged_Nosync, sender, e);
		}

		private void EhProjectChanged_Nosync(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			// UpdateMenu(null, null); // 2006-11-07 hope this is not needed any longer because of the menu update timer
			System.Text.StringBuilder title = new System.Text.StringBuilder();

			if (Current.ComManager != null && Current.ComManager.IsInEmbeddedMode && null != Current.ComManager.EmbeddedObject)
			{
				// we are in embedded mode
				title.Append(ResourceService.GetString("MainWindow.DialogName"));
				title.Append(" ");
				title.Append(Current.ComManager.EmbeddedObject.GetType().Name);

				if (Current.ComManager.EmbeddedObject is Altaxo.Graph.Gdi.GraphDocument)
				{
					title.Append(" ");
					title.Append((Current.ComManager.EmbeddedObject as Altaxo.Graph.Gdi.GraphDocument).Name);
				}

				if (Altaxo.Current.ProjectService.CurrentOpenProject != null && Altaxo.Current.ProjectService.CurrentOpenProject.IsDirty)
					title.Append("*");

				title.Append(" in ");
				title.Append(Current.ComManager.ContainerDocumentName);
				title.Append(" - ");
				title.Append(Current.ComManager.ContainerApplicationName);
			}
			else // normal, non-embedded mode
			{
				title.Append(ResourceService.GetString("MainWindow.DialogName"));
				if (Altaxo.Current.ProjectService != null)
				{
					if (Altaxo.Current.ProjectService.CurrentProjectFileName == null)
					{
						title.Append(" - ");
						title.Append(ResourceService.GetString("Altaxo.Project.UntitledName"));
					}
					else
					{
						title.Append(" - ");
						title.Append(Altaxo.Current.ProjectService.CurrentProjectFileName);
					}
					if (Altaxo.Current.ProjectService.CurrentOpenProject != null && Altaxo.Current.ProjectService.CurrentOpenProject.IsDirty)
						title.Append("*");
				}
			}

			this.MainWindow.Title = title.ToString();
		}

		public ISynchronizeInvoke SynchronizingObject { get; }

		#endregion Altaxo.Gui.Common.IWorkbench Members
	}
}