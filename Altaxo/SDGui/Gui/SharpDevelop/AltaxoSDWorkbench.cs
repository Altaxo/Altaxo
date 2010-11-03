using System.Diagnostics;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.Core;
//using ICSharpCode.Core.WinForms;
using ICSharpCode.Core.Presentation;

using ICSharpCode.SharpDevelop.Gui;



namespace Altaxo.Gui.SharpDevelop
{
	public class AltaxoSDWorkbench : Altaxo.Gui.Common.IWorkbench, ICSharpCode.SharpDevelop.Gui.IWorkbench, System.Windows.Forms.IWin32Window
	{
		WpfWorkbench _wb;

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "ICSharpCode.SharpDevelop.Gui.Workbench1", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoSDWorkbench), 1)]
		class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AltaxoSDWorkbench s = (AltaxoSDWorkbench)obj;
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				return o;
			}
		}

		#endregion



		public AltaxoSDWorkbench()
		{
			_wb = new WpfWorkbench();
			_wb.mainMenuPath = "/Altaxo/Workbench/MainMenu";
			_wb.viewContentPath = "/Altaxo/Workbench/Pads";
			_wb.toolBarPath = "/Altaxo/Workbench/ToolBar";
			_wb.Icon = PresentationResourceService.GetBitmapSource("Icons.32x32.Altaxo");

			// events
			_wb.Closing += EhOnClosing;
		}

		void EhOnClosing(object sender, CancelEventArgs e)
		{
			Altaxo.Main.IProjectService projectService = Altaxo.Current.ProjectService;

			if (projectService != null)
			{
				if (projectService.CurrentOpenProject != null && projectService.CurrentOpenProject.IsDirty)
				{
					projectService.AskForSavingOfProject(e);
				}
			}
		}

		#region IWorkbench Members

		public IWin32Window MainWin32Window
		{
			get { return _wb.MainWin32Window; }
		}

		public ISynchronizeInvoke SynchronizingObject
		{
			get { return _wb.SynchronizingObject; }
		}

		public System.Windows.Window MainWindow
		{
			get { return _wb.MainWindow; }
		}

		public IStatusBarService StatusBar
		{
			get { return _wb.StatusBar; }
		}

		public bool FullScreen
		{
			get
			{
				return _wb.FullScreen;
			}
			set
			{
				_wb.FullScreen = value;
			}
		}

		public string Title
		{
			get
			{
				return _wb.Title;
			}
			set
			{
				_wb.Title = value;
			}
		}

		System.Collections.Generic.ICollection<IViewContent> IWorkbench.ViewContentCollection
		{
			get { return _wb.ViewContentCollection; }
		}

		public System.Collections.Generic.ICollection<IViewContent> PrimaryViewContents
		{
			get { return _wb.PrimaryViewContents; }
		}

		public System.Collections.Generic.IList<IWorkbenchWindow> WorkbenchWindowCollection
		{
			get { return _wb.WorkbenchWindowCollection; }
		}

		public System.Collections.Generic.IList<PadDescriptor> PadContentCollection
		{
			get { return _wb.PadContentCollection; }
		}

		public IWorkbenchWindow ActiveWorkbenchWindow
		{
			get { return _wb.ActiveWorkbenchWindow; }
		}

		IViewContent IWorkbench.ActiveViewContent
		{
			get { return _wb.ActiveViewContent; }
		}

		public event System.EventHandler ActiveViewContentChanged
		{
			add
			{
				_wb.ActiveViewContentChanged += value;
			}
			remove
			{
				_wb.ActiveContentChanged -= value;
			}
		}

		public object ActiveContent
		{
			get { return _wb.ActiveContent; }
		}

		public event System.EventHandler ActiveContentChanged
		{
			add
			{
				_wb.ActiveContentChanged += value;
			}
			remove
			{
				_wb.ActiveContentChanged -= value;
			}
		}

		public IWorkbenchLayout WorkbenchLayout
		{
			get
			{
				return _wb.WorkbenchLayout;
			}
			set
			{
				_wb.WorkbenchLayout = value;
			}
		}

		public bool IsActiveWindow
		{
			get { return _wb.IsActiveWindow; }
		}

		public void Initialize()
		{
			_wb.Initialize();
		}

		public void ShowView(IViewContent content)
		{
			_wb.ShowView(content);
		}

		public void ShowView(IViewContent content, bool switchToOpenedView)
		{
			_wb.ShowView(content, switchToOpenedView);
		}

		public void ShowPad(PadDescriptor content)
		{
			_wb.ShowPad(content);
		}

		public void UnloadPad(PadDescriptor content)
		{
			_wb.UnloadPad(content);
		}

		public PadDescriptor GetPad(System.Type type)
		{
			return _wb.GetPad(type);
		}

		public event ViewContentEventHandler ViewOpened
		{
			add
			{
				_wb.ViewOpened += value;
			}
			remove
			{
				_wb.ViewOpened -= value;
			}
		}

		public event ViewContentEventHandler ViewClosed
		{
			add
			{
				_wb.ViewClosed += value;
			}
			remove
			{
				_wb.ViewClosed -= value;
			}
		}

		#endregion

		#region IMementoCapable Members

		public Properties CreateMemento()
		{
			return _wb.CreateMemento();
		}

		public void SetMemento(Properties memento)
		{
			_wb.SetMemento(memento);
		}

		#endregion

		#region IWin32Window Members

		public System.IntPtr Handle
		{
			get { return ((IWin32Window)_wb).Handle; }
		}

		#endregion

		#region Altaxo.Gui.Common.IWorkbench Members

		public object ViewObject
		{
			get
			{
				return _wb;
			}
		}

		public object ActiveViewContent
		{
			get { return null != _wb.ActiveWorkbenchWindow ? _wb.ActiveWorkbenchWindow.ActiveViewContent : null; }
		}

		public System.Collections.ICollection ViewContentCollection
		{
			get { return (System.Collections.ICollection)_wb.ViewContentCollection; }
		}

		public void ShowView(object content)
		{
			_wb.ShowView((IViewContent)content);
		}

		public void CloseContent(object content)
		{
			((IViewContent)content).WorkbenchWindow.CloseWindow(true);
		}

		public void CloseAllViews()
		{
			_wb.CloseAllViews();
		}

		public bool CloseAllSolutionViews()
		{
			_wb.CloseAllViews();
			return true;
		}


		public event System.EventHandler ActiveWorkbenchWindowChanged
		{
			add
			{
				_wb.ActiveWorkbenchWindowChanged += value;
			}
			remove
			{
				_wb.ActiveWorkbenchWindowChanged -= value;
			}
		}

		public void EhProjectChanged(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			Current.Gui.Execute(EhProjectChanged_Nosync, sender, e);
		}
		private void EhProjectChanged_Nosync(object sender, Altaxo.Main.ProjectEventArgs e)
		{
			// UpdateMenu(null, null); // 2006-11-07 hope this is not needed any longer because of the menu update timer
			System.Text.StringBuilder title = new System.Text.StringBuilder();
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

			this.MainWindow.Title = title.ToString();
		}

		#endregion




	}
}
