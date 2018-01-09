using Altaxo.AddInItems;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Startup;
using Altaxo.Main;
using Altaxo.Main.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Viewmodel class for the workbench
	/// </summary>
	public class AltaxoWorkbench : IWorkbenchEx
	{
		private const string mainMenuPathPostFix = "/Workbench/MainMenu";
		private const string documentContextMenuPathPostFix = "/Workbench/OpenFileTab/ContextMenu";
		private const string padContentPathPostFix = "/Workbench/Pads";
		private const string toolBarPathPostFix = "/Workbench/ToolBar";

		private object _mainWindow;

		/// <summary>
		/// Gets/sets if the main window is collapsed. It is e.g. set to false if this app starts as COM server.
		/// </summary>
		private bool _isMainWindowCollapsed;

		/// <summary>
		/// Gets/sets if the is main window is shown in the taskbar.
		/// </summary>
		private bool _isMainWindowShownInTaskbar = true;

		/// <summary>
		/// The current title of the main window.
		/// </summary>
		private string _mainWindowTitle;

		/// <summary>
		/// The current icon resource string of the main window.
		/// </summary>
		private string _mainWindowIconResource;

		/// <summary>
		/// The source of the items for the main menu.
		/// </summary>
		private object _mainMenuItemsSource;

		/// <summary>
		/// The items source for the toolbar tray. Currently, this is a collection of toolbars.
		/// </summary>
		private object _toolBarTrayItemsSource;

		/// <summary>
		/// Context menu items for the tabs in the document area
		/// </summary>
		private object _documentContextMenuItemsSource;

		/// <summary>
		/// The Gui component of the status bar.
		/// </summary>
		private object _statusBarView;

		/// <summary>
		/// The pad descriptor collection. Each pad descriptor corresponds to a toolbar window in the Gui.
		/// The controller for the toolbar window is padDescriptor.PadContent.
		/// </summary>
		private ObservableCollection<IPadContent> _padContentCollection;

		/// <summary>The pad descriptor collection as read only collection (is exposed for binding to the main window).</summary>
		private ReadOnlyObservableCollection<IPadContent> _padContentCollectionAsReadOnly;

		/// <summary>
		/// The document collection. Each member of this collection a controller for a document, which usually belongs to the currently open project.
		/// </summary>
		private ObservableCollection<IViewContent> _documentCollection;

		/// <summary>The document collection as read only collection (is exposed for binding to the main window).
		/// </summary>
		private ReadOnlyObservableCollection<IViewContent> _documentCollectionAsReadOnly;

		/// <summary>
		/// Deserialized layout of the docking manager.
		/// </summary>
		private string _dockingLayoutAsString;

		/// <summary>
		/// True if the main window is in full screen mode
		/// </summary>
		private bool _isInFullScreenMode;

		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		protected IWorkbenchContent _activeContent;

		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		protected IViewContent _activeViewContent;

		/// <summary>
		/// Gets whether this application is the active application in Windows.
		/// </summary>
		private bool _isActiveWindow;

		/// <summary>
		/// The last state of the workbench (size, position, maximized or normal) that was not a minimized state;
		/// </summary>
		private WorkbenchState _workbenchState;

		/// <summary>
		/// The chosen theme for the dock manager.
		/// </summary>
		private string _dockManagerTheme;

		public event EventHandler ActiveViewContentChanged;

		public event EventHandler ActiveContentChanged;

		public event EventHandler<ViewContentEventArgs> ViewOpened;

		public event EventHandler<ViewContentEventArgs> ViewClosed;

		public event PropertyChangedEventHandler PropertyChanged;

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "ICSharpCode.SharpDevelop.Gui.Workbench1", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "Altaxo.Gui.Workbench1", 1)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoWorkbench), 2)]
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

		public IList<IPadContent> PadContentCollection
		{
			get
			{
				return _padContentCollectionAsReadOnly;
			}
		}

		public AltaxoWorkbench()
		{
			_padContentCollection = new ObservableCollection<IPadContent>();
			_padContentCollectionAsReadOnly = new ReadOnlyObservableCollection<IPadContent>(_padContentCollection);

			_documentCollection = new ObservableCollection<IViewContent>();
			_documentCollectionAsReadOnly = new ReadOnlyObservableCollection<IViewContent>(_documentCollection);
			_documentCollection.CollectionChanged += EhDocumentCollectionChanged;

			// if the application was started as a COM server, hide the window until there is an explicit COM command to show the window
			if (Current.ComManager?.ApplicationWasStartedWithEmbeddingArg ?? false)
			{
				this.IsCollapsed = true;
				this.IsShownInTaskbar = false;
			}
		}

		public void Initialize(object mainWindow)
		{
			var startupSettings = Current.GetRequiredService<StartupSettings>();
			string appNamePrefix = "/" + startupSettings.ApplicationName;

			_mainWindow = mainWindow;

			// Initialize main menu items
			MainMenuItemsSource = MenuService.CreateMenuItems((System.Windows.UIElement)_mainWindow, this, appNamePrefix + mainMenuPathPostFix, activationMethod: "MainMenu", immediatelyExpandMenuBuildersForShortcuts: true);

			// Initialize toolbars
			ToolBarTrayItemsSource = ToolBarService.CreateToolBars(_mainWindow, this, appNamePrefix + toolBarPathPostFix);

			// Initialize context menu of document tabs
			DocumentContextMenuItemsSource = MenuService.CreateMenuItems((System.Windows.UIElement)_mainWindow, this, appNamePrefix + documentContextMenuPathPostFix, activationMethod: "MainMenu", immediatelyExpandMenuBuildersForShortcuts: true);

			// Initialize pads (tool windows)
			var padDescriptors = AddInTree.BuildItems<PadDescriptor>(appNamePrefix + padContentPathPostFix, this, false);
			_padContentCollection.AddRange(padDescriptors.Select(x => x.PadContent));

			// Initialize status bar
			var statusBarService = Current.GetService<IStatusBarService>();
			if (statusBarService is IMVCController statusBarController)
			{
				if (statusBarController.ViewObject == null)
					Current.Gui.FindAndAttachControlTo(statusBarController);
				_statusBarView = statusBarController.ViewObject;
			}

			// Initialize Icon
			IconSource = "Icons." + startupSettings.ApplicationName + "ApplicationIcon";

			Current.IProjectService.ProjectChanged += EhProjectChanged;
			Title = Current.IProjectService.GetMainWindowTitle();

			WorkbenchServices.StatusBar.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}

		public void Initialize()
		{
			throw new NotImplementedException();
		}

		protected virtual void OnPropertyChanged(string nameOfProperty)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameOfProperty));
		}

		public object MainWindow
		{
			get
			{
				return _mainWindow;
			}
		}

		public object ViewObject
		{
			get
			{
				return _mainWindow;
			}
		}

		/// <summary>
		/// Gets the main menu items source. This is used to bind mainMenu.ItemsSource if the main window to this property.
		/// </summary>
		/// <value>
		/// The main menu items source.
		/// </value>
		public object MainMenuItemsSource
		{
			get
			{
				return _mainMenuItemsSource;
			}
			set
			{
				_mainMenuItemsSource = value;
				OnPropertyChanged(nameof(MainMenuItemsSource));
			}
		}

		/// <summary>
		/// Gets the items source for the tool bar tray.
		/// </summary>
		/// <value>
		/// The main menu items source.
		/// </value>
		public object ToolBarTrayItemsSource
		{
			get
			{
				return _toolBarTrayItemsSource;
			}
			set
			{
				_toolBarTrayItemsSource = value;
				OnPropertyChanged(nameof(ToolBarTrayItemsSource));
			}
		}

		/// <summary>
		/// Gets the document context menu items source. This is used to bind to the main window's document context menu.
		/// </summary>
		/// <value>
		/// The document context menu items source.
		/// </value>
		public object DocumentContextMenuItemsSource
		{
			get
			{
				return _documentContextMenuItemsSource;
			}
			set
			{
				_documentContextMenuItemsSource = value;
				OnPropertyChanged(nameof(DocumentContextMenuItemsSource));
			}
		}

		/// <summary>
		/// Gets or sets the view object of the status bar. Bindable property.
		/// </summary>
		/// <value>
		/// The status bar view.
		/// </value>
		public object StatusBarView
		{
			get
			{
				return _statusBarView;
			}
			set
			{
				if (!(_statusBarView == value))
				{
					_statusBarView = value;
					OnPropertyChanged(nameof(StatusBarView));
				}
			}
		}

		public string Title
		{
			get
			{
				return _mainWindowTitle;
			}
			set
			{
				if (!(_mainWindowTitle == value))
				{
					_mainWindowTitle = value;
					OnPropertyChanged(nameof(Title));
				}
			}
		}

		public string IconSource
		{
			get
			{
				return _mainWindowIconResource;
			}
			set
			{
				if (!(_mainWindowIconResource == value))
				{
					_mainWindowIconResource = value;
					OnPropertyChanged(nameof(IconSource));
				}
			}
		}

		/// <summary>
		/// Gets/sets if the is main window is shown in the taskbar.
		/// </summary>
		public bool IsShownInTaskbar
		{
			get
			{
				return _isMainWindowShownInTaskbar;
			}
			set
			{
				if (!(_isMainWindowShownInTaskbar == value))
				{
					_isMainWindowShownInTaskbar = value;
					OnPropertyChanged(nameof(IsShownInTaskbar));
				}
			}
		}

		/// <summary>
		/// Gets/sets if the main window is collapsed. It is e.g. set to false if this app starts as COM server.
		/// </summary>
		public bool IsCollapsed
		{
			get
			{
				return _isMainWindowCollapsed;
			}
			set
			{
				if (!(_isMainWindowCollapsed == value))
				{
					_isMainWindowCollapsed = value;
					OnPropertyChanged(nameof(IsCollapsed));
				}
			}
		}

		#region Dock manager theme

		private static readonly Altaxo.Main.Properties.PropertyKey<string> PropertyKeyWorkbenchDockingTheme =
new Altaxo.Main.Properties.PropertyKey<string>(
	 "C689E859-F121-41B3-9038-9B3730D36F2F",
	 "App\\WorkbenchDockingTheme",
	 Altaxo.Main.Properties.PropertyLevel.Application);

		public void StoreWorkbenchDockingThemeInPropertyService()
		{
			Current.PropertyService.SetValue(PropertyKeyWorkbenchDockingTheme, DockManagerTheme);
		}

		public void RestoreWorkbenchDockingThemeFromPropertyService()
		{
			DockManagerTheme = Current.PropertyService.GetValue(PropertyKeyWorkbenchDockingTheme, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => null);
		}

		public string DockManagerTheme
		{
			get
			{
				return _dockManagerTheme;
			}
			set
			{
				if (!(_dockManagerTheme == value))
				{
					_dockManagerTheme = value;
					OnPropertyChanged(nameof(DockManagerTheme));
				}
			}
		}

		#endregion Dock manager theme

		#region Layout configuration

		/// <summary>
		/// Gets / sets the docking layout as string. Whenever this string is updated from the viewmodel, the layout gets loaded in the docking manager (via two way binding).
		/// On the other hand, during unload of the main window, the docking manager updates this string via binding, so that it can be saved afterwards.
		/// </summary>
		/// <value>
		/// The docking layout as deserialized string.
		/// </value>
		public string CurrentLayoutConfiguration
		{
			get
			{
				return _dockingLayoutAsString;
			}
			set
			{
				if (!(_dockingLayoutAsString == value))
				{
					_dockingLayoutAsString = value;
					OnPropertyChanged(nameof(CurrentLayoutConfiguration));
				}
			}
		}

		private static readonly Altaxo.Main.Properties.PropertyKey<string> PropertyKeyWorkbenchDockingLayout =
 new Altaxo.Main.Properties.PropertyKey<string>(
		 "207C2B8E-4D20-43EF-9246-6E0B300069FC",
		 "App\\WorkbenchDockingLayout",
		 Altaxo.Main.Properties.PropertyLevel.Application);

		public void StoreWorkbenchDockingLayoutInPropertyService()
		{
			Current.PropertyService.SetValue(PropertyKeyWorkbenchDockingLayout, CurrentLayoutConfiguration);
		}

		public void RestoreWorkbenchDockingLayoutFromPropertyService()
		{
			CurrentLayoutConfiguration = Current.PropertyService.GetValue(PropertyKeyWorkbenchDockingLayout, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => null);
		}

		#endregion Layout configuration

		public bool FullScreen
		{
			get { return _isInFullScreenMode; }

			set
			{
				if (!(_isInFullScreenMode == value))
				{
					_isInFullScreenMode = value;
					OnPropertyChanged(nameof(FullScreen));
				}
			}
		}

		public void SaveCompleteWorkbenchStateAndLayoutInPropertyService()
		{
			this.StoreWorkbenchStateInPropertyService();
			this.StoreWorkbenchDockingLayoutInPropertyService();
			this.StoreWorkbenchDockingThemeInPropertyService();
		}

		#region WorkbenchState

		/// <summary>
		/// The last state of the workbench that was not a minimized state. Setting this property will update the size and state of the main
		/// workbench window via binding.
		/// </summary>
		public WorkbenchState WorkbenchState
		{
			get { return _workbenchState; }

			set
			{
				if (!object.ReferenceEquals(_workbenchState, value))
				{
					_workbenchState = value;
					OnPropertyChanged(nameof(WorkbenchState));
				}
			}
		}

		private static readonly Altaxo.Main.Properties.PropertyKey<WorkbenchState> PropertyKeyWorkbenchState =
	new Altaxo.Main.Properties.PropertyKey<WorkbenchState>(
			"F8B02744-B011-4A34-838D-ABFE5429CF59",
			"App\\WorkbenchState",
			Altaxo.Main.Properties.PropertyLevel.Application);

		public void StoreWorkbenchStateInPropertyService()
		{
			Current.PropertyService.SetValue(PropertyKeyWorkbenchState, WorkbenchState);
		}

		public void RestoreWorkbenchStateFromPropertyService()
		{
			WorkbenchState = Current.PropertyService.GetValue(PropertyKeyWorkbenchState, Altaxo.Main.Services.RuntimePropertyKind.UserAndApplicationAndBuiltin, () => null);
		}

		#endregion WorkbenchState

		public IList<IViewContent> ViewContentCollection
		{
			get
			{
				return _documentCollectionAsReadOnly;
			}
		}

		public ICollection<IViewContent> PrimaryViewContents
		{
			get
			{
				return _documentCollectionAsReadOnly;
			}
		}

		/// <summary>
		/// The active content, depending on where the focus currently is.
		/// If a document is currently active, this will be equal to ActiveViewContent,
		/// if a pad has the focus, this property will return the IPadContent instance.
		/// </summary>
		public IWorkbenchContent ActiveContent
		{
			get
			{
				return _activeContent;
			}
			set
			{
				if (!object.ReferenceEquals(_activeContent, value))
				{
					_activeContent = value;
					OnPropertyChanged(nameof(ActiveContent));
					ActiveContentChanged?.Invoke(this, EventArgs.Empty);

					if (value is IViewContent viewContent)
						ActiveViewContent = viewContent;
				}
			}
		}

		/// <summary>
		/// The active view content inside the active workbench window.
		/// </summary>
		public IViewContent ActiveViewContent
		{
			get
			{
				return _activeViewContent;
			}
			set
			{
				if (!object.ReferenceEquals(_activeViewContent, value))
				{
					_activeViewContent = value;
					OnPropertyChanged(nameof(ActiveViewContent));
					ActiveViewContentChanged?.Invoke(this, EventArgs.Empty);
				}
				if (null != value)
					ActiveContent = value;
			}
		}

		/// <summary>
		/// Avalon's dock manager does not set ActiveViewContent to null if the last view content is deleted. Therefore,
		/// we have to watch the view content collection, and if the count is null, then set ActiveViewContent to zero.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
		private void EhDocumentCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (_documentCollection.Count == 0)
				ActiveViewContent = null;
		}

		/// <summary>
		/// Gets whether this application is the active application in Windows.
		/// </summary>
		public bool IsActiveWindow
		{
			get
			{
				return _isActiveWindow;
			}
			set
			{
				if (!(_isActiveWindow == value))
				{
					_isActiveWindow = value;
					OnPropertyChanged(nameof(IsActiveWindow));
				}
			}
		}

		public bool CloseAllSolutionViews(bool force)
		{
			bool result = true;
			foreach (var docContent in this._documentCollection.ToArray())
			{
				if (docContent.CloseWithSolution)
				{
					if (_documentCollection.Remove(docContent))
						docContent.Dispose();
				}
			}
			return result;
		}

		public void CloseAllViews()
		{
			var documents = _documentCollection.ToArray();
			_documentCollection.Clear();
			foreach (var document in documents)
				document.Dispose();
		}

		/// <summary>
		/// Gets the content for the provided document. The content is found either by direct comparison of the provided document with the <see cref="IMVCController.ModelObject"/>,
		/// or, if the <see cref="IMVCController.ModelObject"/> is a <see cref="IProjectItemPresentationModel"/>, by comparison to the <see cref="IProjectItemPresentationModel.Document"/>.
		/// </summary>
		/// <typeparam name="T">The type of controller to search for. The argument <see cref="T:Altaxo.Gui.IMVCController" /> will find any controller that uses the provided document.
		/// If you provide a more specific type, only such a controller that implements this type and has the provided document will be returned.</typeparam>
		/// <param name="document">The document to search for.</param>
		/// <returns>
		/// The content (either a <see cref="T:Altaxo.Gui.Workbench.IPadContent" /> or a <see cref="T:Altaxo.Gui.Workbench.IViewContent" /> whose <see cref="P:Altaxo.Gui.IMVCController.ModelObject" /> is the provided document.
		/// </returns>
		public T GetViewModel<T>(object document) where T : IMVCController
		{
			return GetViewModels<T>(document).FirstOrDefault();
		}

		/// <summary>
		/// Gets the content for the provided document. The content is found either by direct comparison of the provided document with the <see cref="IMVCController.ModelObject"/>,
		/// or, if the <see cref="IMVCController.ModelObject"/> is a <see cref="IProjectItemPresentationModel"/>, by comparison to the <see cref="IProjectItemPresentationModel.Document"/>.
		/// </summary>
		/// <typeparam name="T">The type of controller to search for. The argument <see cref="T:Altaxo.Gui.IMVCController" /> will find any controller that uses the provided document.
		/// If you provide a more specific type, only such a controller that implements this type and has the provided document will be returned.</typeparam>
		/// <param name="document">The document to search for.</param>
		/// <returns>
		/// The content (either a <see cref="T:Altaxo.Gui.Workbench.IPadContent" /> or a <see cref="T:Altaxo.Gui.Workbench.IViewContent" /> whose <see cref="P:Altaxo.Gui.IMVCController.ModelObject" /> is the provided document.
		/// </returns>
		public IEnumerable<T> GetViewModels<T>(object document) where T : IMVCController
		{
			foreach (var documentContent in ViewContentCollection)
			{
				if (object.ReferenceEquals(document, documentContent.ModelObject) && typeof(T).IsAssignableFrom(documentContent.GetType()))
					yield return (T)documentContent;
				if (documentContent.ModelObject is IProjectItemPresentationModel presentationModel && object.ReferenceEquals(document, presentationModel.Document) && typeof(T).IsAssignableFrom(documentContent.GetType()))
					yield return (T)documentContent;
			}

			foreach (var padContent in _padContentCollection)
			{
				if (object.ReferenceEquals(document, padContent.ModelObject) && typeof(T).IsAssignableFrom(padContent.GetType()))
					yield return (T)padContent;
			}
		}

		public void EhProjectChanged(object sender, ProjectEventArgs e)
		{
			switch (e.ProjectEventKind)
			{
				case ProjectEventKind.ProjectClosed:
					Title = StringParser.Parse("${AppName}");
					break;

				case ProjectEventKind.ProjectOpened:
				case ProjectEventKind.ProjectRenamed:
				case ProjectEventKind.ProjectDirtyChanged:
					Title = Current.IProjectService.GetMainWindowTitle();
					break;
			}
		}

		#region Document handling

		public void ShowPad(IPadContent content, bool switchToPad)
		{
			if (null == content)
				throw new ArgumentNullException(nameof(content));

			if (!_padContentCollection.Contains(content))
				_padContentCollection.Add(content);

			if (content.ViewObject == null)
				Current.Gui.FindAndAttachControlTo(content);

			content.IsVisible = true;

			if (switchToPad)
			{
				content.IsSelected = true;
			}
		}

		public void ShowView(IViewContent content)
		{
			ShowView(content, true);
		}

		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			if (!_documentCollection.Contains(content))
				_documentCollection.Add(content);

			LoadViewContentMemento(content);

			if (content.ViewObject == null)
				Current.Gui.FindAndAttachControlTo(content);

			content.IsVisible = true;

			if (switchToOpenedView)
			{
				content.IsSelected = true;
			}
		}

		public void ShowView(object content)
		{
			if (content is IViewContent viewContent)
			{
				this.ShowView(viewContent);
			}
			else if (content is IProjectItemPresentationModel)
			{
				var ctrl = (IViewContent)Current.Gui.GetController(new object[] { content }, typeof(IViewContent));
				if (null != ctrl)
					this.ShowView(ctrl);
			}
		}

		public void CloseContent(IViewContent viewcontent)
		{
			if (_documentCollection.Remove(viewcontent))
				viewcontent.Dispose();
		}

		#endregion Document handling

		#region ToolPads

		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		/// <param name="type">The type of pad to search for.</param>
		/// <returns>The pad descriptor of this pad if found; otherwise, null.</returns>
		/// <exception cref="ArgumentNullException">type</exception>
		public PadDescriptor GetPad(Type type)
		{
			if (type == null)
				throw new ArgumentNullException(nameof(type));

			foreach (PadDescriptor pad in PadContentCollection)
			{
				if (pad.Class == type.FullName)
				{
					return pad;
				}
			}
			return null;
		}

		/// <summary>
		/// Activates the specified pad.
		/// </summary>
		/// <param name="content"></param>
		public void ActivatePad(PadDescriptor content)
		{
			if (null == content.PadContent)
				content.CreatePad();

			content.PadContent.IsVisible = true;
		}

		#endregion ToolPads

		#region Loading/storing of content mementos

		private void LoadViewContentMemento(IViewContent viewContent)
		{
			/*
IMementoCapable mementoCapable = viewContent.GetService<IMementoCapable>();
if (mementoCapable != null && LoadDocumentProperties)
{
	if (viewContent.PrimaryFileName == null)
		return;

	try
	{
		string key = GetMementoKeyName(viewContent);
		Current.Log.Debug("Trying to restore memento of '" + viewContent.ToString() + "' from key '" + key + "'");

		mementoCapable.SetMemento(this.LoadOrCreateViewContentMementos().NestedProperties(key));
	}
	catch (Exception e)
	{
		MessageService.ShowException(e, "Can't get/set memento");
	}
}
*/
		}

		#endregion Loading/storing of content mementos

		#region Close

		public void Close()
		{
			App.Current.MainWindow.Close();
		}

		#endregion Close
	}
}