/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002 Dr. Dirk Lellinger
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


using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

using Altaxo.Serialization;
using Altaxo.Main.GUI;

using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Gui
{
	public interface IExtendedWorkbench : IWorkbench
	{
		/// <summary>
		/// Returns the GUI component (a Windows Form for Windows) that is associated with that workbench.
		/// </summary>
		object ViewObject { get; }
	}

}


namespace Altaxo
{
	#region MVC Interface definitions
	

	/// <summary>
	/// This interface has to be implemented by all forms that are controlled by a <see cref="Altaxo.IMainViewEventSink"/>.
	/// </summary>
	public interface IMainView
	{
		/// <summary>
		/// Returns the Windows forms (i.e. in almost all cases - itself).
		/// </summary>
		System.Windows.Forms.Form Form { get; }
	

		/// <summary>
		/// Sets the contoller for this view.
		/// </summary>
		Altaxo.IMainViewEventSink Controller { set; }

		/// <summary>
		/// Sets the main menu for the main window
		/// </summary>
		System.Windows.Forms.MainMenu MainViewMenu {	set; }
	}

	#endregion

	public interface IMainViewEventSink
	{
		/// <summary>
		/// Called if the view is about to be closed.
		/// </summary>
		/// <param name="e">CancelEventArgs</param>
		void EhView_Closing(System.ComponentModel.CancelEventArgs e);

		/// <summary>
		/// Called if the view is closed now.
		/// </summary>
		/// <param name="e">EventArgs</param>
		void EhView_Closed(System.EventArgs e);

		/// <summary>
		/// This is called if the Close message or shutdown is captured from the view
		/// </summary>
		void EhView_CloseMessage();
	
	}




	/// <summary>
	/// The class that controls the main window, i.e. the MDI parent window, of the application.
	/// </summary>
	public class AltaxoWorkbench : IExtendedWorkbench, IMainViewEventSink
	{

		/// <summary>
		/// The Gui component of this controller.
		/// </summary>
		public    IMainView m_View;
		
		/// <summary>
		/// The layout manager - responsible to layout the application window.
		/// </summary>
		protected ICSharpCode.SharpDevelop.Gui.IWorkbenchLayout m_Layout;


		// protected System.Collections.ArrayList m_WorkbenchViews = new System.Collections.ArrayList();
		protected ViewContentCollection m_ViewContentCollection = new ViewContentCollection();
		protected PadContentCollection m_PadContentCollection = new PadContentCollection();

		protected IWorkbenchWindow m_ActiveWorkbenchWindow;

		protected string m_Title;

	

		
		#region Serialization
		
		public class AltaxoWorkbenchMemento
		{
			public AltaxoWorkbenchMemento(MainController ctrl)
			{
			}
			public AltaxoWorkbenchMemento()
			{
			}
		}

		[Altaxo.Serialization.Xml.XmlSerializationSurrogateFor(typeof(AltaxoWorkbenchMemento),0)]
			public new class XmlSerializationSurrogate0 : Altaxo.Serialization.Xml.IXmlSerializationSurrogate
		{
			public void Serialize(object obj, Altaxo.Serialization.Xml.IXmlSerializationInfo info)
			{
				AltaxoWorkbenchMemento s = (AltaxoWorkbenchMemento)obj;
			}
			public object Deserialize(object o, Altaxo.Serialization.Xml.IXmlDeserializationInfo info, object parent)
			{
				
				AltaxoWorkbenchMemento s = null!=o ? (AltaxoWorkbenchMemento)o : new AltaxoWorkbenchMemento();
				return s;
			}
		}

		#endregion
	
		public AltaxoWorkbench(IMainView view)
		{
			m_View = view;
			m_View.Controller = this;


		

			// attach a layout before creating the first windows
			this.WorkbenchLayout = new WindowsMdiWorkbenchLayout();
		}

	

		#region Properties
	

		

		#endregion

		#region IMainController members
		

		/// <summary>
		/// The view that is controlled by this controller.
		/// </summary>
		public IMainView View
		{
			get { return m_View; }
		}

		


		public void EhView_Closing(System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true; // in doubt cancel the closing

			if(Current.Project.IsDirty)
			{
				System.Windows.Forms.DialogResult dlgres = System.Windows.Forms.MessageBox.Show(this.View.Form,"Do you want to save your document?","Attention",System.Windows.Forms.MessageBoxButtons.YesNoCancel);
				if(dlgres==System.Windows.Forms.DialogResult.Yes)
				{
					Current.ProjectService.SaveProject();
					if(!Current.Project.IsDirty)
						e.Cancel = false;
				}
				else if(dlgres==System.Windows.Forms.DialogResult.No)
				{
					e.Cancel = false;
				}
				else if(dlgres==System.Windows.Forms.DialogResult.Cancel)
				{
					e.Cancel = true;
				}
			}
			else // the document is not dirty
			{
				e.Cancel = false;
			}



			// update the closing flag - if e.Cancel is true, the application is not longer in the closing state
			Current.ApplicationIsClosing = (false==e.Cancel);
		}

		public void EhView_Closed(System.EventArgs e)
		{
			View.Controller=null; // we are no longer the controller
		}

		public void EhView_CloseMessage()
		{
			Current.ApplicationIsClosing = true;
		}


		#endregion



		#region ICSharpCode


		/// <summary>
		/// The title shown in the title bar.
		/// </summary>
		public string Title 
		{
			get
			{
				return m_Title;
			}
			set
			{
				m_Title = value;
			}
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		public ViewContentCollection ViewContentCollection 
		{
			get 
			{
				return m_ViewContentCollection;
			}
		}
		
		/// <summary>
		/// A collection in which all active workspace windows are saved.
		/// </summary>
		public PadContentCollection PadContentCollection 
		{
			get 
			{
				return m_PadContentCollection;
			}
		}

		/// <summary>
		/// Returns the GUI component associated with this controller
		/// </summary>
		public object ViewObject
		{
			get { return this.View; }
		}
		
		/// <summary>
		/// The active workbench window.
		/// </summary>
		public IWorkbenchWindow ActiveWorkbenchWindow 
		{
			get 
			{
				return m_Layout==null ? null : m_Layout.ActiveWorkbenchwindow;
			}
		}
		
		public IWorkbenchLayout WorkbenchLayout 
		{
			get
			{
				return m_Layout;
			}
			set
			{
				if(m_Layout !=null)
				{
					m_Layout.ActiveWorkbenchWindowChanged -= new EventHandler(this.EhLayout_ActiveWorkbenchWindowChanged);
					m_Layout.Detach();
				}

				m_Layout = value;

				if(m_Layout!=null)
				{
					m_Layout.Attach(this);
					m_Layout.ActiveWorkbenchWindowChanged += new EventHandler(this.EhLayout_ActiveWorkbenchWindowChanged);
				}
			}
		}
		
		/// <summary>
		/// Inserts a new <see cref="IViewContent"/> object in the workspace.
		/// </summary>
		public void ShowView(IViewContent content)
		{
			if(null!=m_Layout)
				m_Layout.ShowView(content);
		}
		
		/// <summary>
		/// Inserts a new <see cref="IPadContent"/> object in the workspace.
		/// </summary>
		public void ShowPad(IPadContent content)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Returns a pad from a specific type.
		/// </summary>
		public IPadContent GetPad(Type type)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Closes the IViewContent content when content is open.
		/// </summary>
		public void CloseContent(IViewContent content)
		{
			if (m_ViewContentCollection.Contains(content)) 
			{
				m_ViewContentCollection.Remove(content);
			}
			content.Dispose();
		}
		
		// TODO I feel a better place to do that is the layout manager
		/// <summary>
		/// Closes all views inside the workbench.
		/// </summary>
		public void CloseAllViews()
		{
			try 
			{
				Current.ApplicationIsClosing = true;
				ViewContentCollection fullList = new ViewContentCollection(m_ViewContentCollection);
				foreach (IViewContent content in fullList) 
				{
					IWorkbenchWindow window = content.WorkbenchWindow;
					window.CloseWindow(false);
				}
			} 
			finally 
			{
				Current.ApplicationIsClosing = false;
				if(null!=ActiveWorkbenchWindowChanged)
					ActiveWorkbenchWindowChanged(this,EventArgs.Empty);
			}
		}

		void EhLayout_ActiveWorkbenchWindowChanged(object sender, EventArgs e)
		{
			if (!Current.ApplicationIsClosing && ActiveWorkbenchWindowChanged != null) 
			{
				ActiveWorkbenchWindowChanged(this, e);
			}
		}
		
		/// <summary>
		/// Re-initializes all components of the workbench, should be called
		/// when a special property is changed that affects layout stuff.
		/// (like language change) 
		/// </summary>
		public void RedrawAllComponents()
		{
		}
		
		/// <summary>
		/// Creates a new memento from the state.
		/// </summary>
		public ICSharpCode.Core.Properties.IXmlConvertable CreateMemento()
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Sets the state to the given memento.
		/// </summary>
		public void SetMemento(ICSharpCode.Core.Properties.IXmlConvertable memento)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Is called, when the workbench window which the user has into
		/// the foreground (e.g. editable) changed to a new one.
		/// </summary>
		public event EventHandler ActiveWorkbenchWindowChanged;

		#endregion

	
	}




}
