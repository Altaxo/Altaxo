#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using ICSharpCode.Core;

//using ICSharpCode.Core.WinForms;
using ICSharpCode.Core.Presentation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Xml;

namespace Altaxo.Gui.SharpDevelop
{
	public class AltaxoSDWorkbench : Altaxo.Gui.Common.IWorkbench, ICSharpCode.SharpDevelop.Gui.IWorkbench // , System.Windows.Forms.IWin32Window
	{
		private WpfWorkbench _wb;

		#region "Serialization"

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor("AltaxoSDGui", "ICSharpCode.SharpDevelop.Gui.Workbench1", 0)]
		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoSDWorkbench), 1)]
		private class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
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

		#endregion "Serialization"

		public AltaxoSDWorkbench()
		{
			_wb = new WpfWorkbench();
			_wb.mainMenuPath = "/Altaxo/Workbench/MainMenu";
			_wb.viewContentPath = "/Altaxo/Workbench/Pads";
			_wb.toolBarPath = "/Altaxo/Workbench/ToolBar";
			_wb.Icon = null; // PresentationResourceService.GetBitmapSource("Icons.MainApplicationIcon");

			// events
			_wb.Closing += EhOnClosing;

			_wb.Initialized += EhInitialized;
		}

		private void EhInitialized(object sender, System.EventArgs e)
		{
			_wb.Initialized -= EhInitialized;
			if (Current.ComManager != null && Current.ComManager.ApplicationWasStartedWithEmbeddingArg)
				_wb.Visibility = System.Windows.Visibility.Hidden;
		}

		private void EhOnClosing(object sender, CancelEventArgs e)
		{
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
				// Stop the Com-Server already here, it seems to be too late if we try to stop it in OnClosed()
				var comManager = Current.ComManager as Altaxo.Com.ComManager;
				if (null != comManager)
					comManager.StopLocalServer();
			}
		}

		#region IWorkbench Members

		public System.Windows.Forms.IWin32Window MainWin32Window
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

		#endregion IWorkbench Members

		#region IMementoCapable Members

		public Properties CreateMemento()
		{
			return _wb.CreateMemento();
		}

		public void SetMemento(Properties memento)
		{
			_wb.SetMemento(memento);
		}

		#endregion IMementoCapable Members

		#region IWin32Window Members

		public System.IntPtr Handle
		{
			get { return ((IWin32Window)_wb).Handle; }
		}

		#endregion IWin32Window Members

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

		#endregion Altaxo.Gui.Common.IWorkbench Members
	}
}