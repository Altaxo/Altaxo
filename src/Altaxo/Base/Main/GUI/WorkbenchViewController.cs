using System;

namespace Altaxo.Main.GUI
{

	public interface IWorkbenchContentController
	{
		IWorkbenchContentView View { get; set; }

		IWorkbenchWindowController ParentWorkbenchWindowController { get; set; }

		
		/// <summary>
		/// Closes the view and sets the view to null. (But leaves the controller itself intact).
		/// </summary>
		void CloseView();

		/// <summary>
		/// If the controller has no view, this function creates a default view and attaches to it.
		/// </summary>
		void CreateView();

	}

	public interface IWorkbenchContentView
	{
		
	}
	/// <summary>
	/// Summary description for IWorkbenchViewController.
	/// </summary>
	public interface IWorkbenchWindowController
	{
		Main.GUI.IWorkbenchWindowView View { get; set; }
		IWorkbenchContentController Content { get; set; }
		void CloseView();

	}

	public interface IWorkbenchWindowView
	{
		System.Windows.Forms.Form Form { get; }
		IWorkbenchWindowController Controller { get; set; }
		void SetChild(IWorkbenchContentView content);
		void Close();

		/// <summary>
		/// Sets the title of this window.
		/// </summary>
		/// <param name="title">The title of this window.</param>
		void SetTitle(string title);
	}

	public class WorkbenchWindowController : IWorkbenchWindowController
	{
		protected Main.GUI.IWorkbenchWindowView m_View;
		protected IWorkbenchContentController m_Content;
		/// <summary>
		/// The windows title.
		/// </summary>
		protected string m_Title;

		public Main.GUI.IWorkbenchWindowView View
		{
			get { return m_View; }
			set
			{
				if(m_View!=null)
					m_View.Controller=null;

				m_View = value;

				if(m_View!=null)
					m_View.Controller = this;
			}
		}

		public IWorkbenchContentController Content
		{
			get { return m_Content; }
			set 
			{
				IWorkbenchContentController oldContent = m_Content;
				m_Content = value;

				if(oldContent != null)
				{
					oldContent.ParentWorkbenchWindowController = null;
				}

				if(m_Content!=null)
				{
					m_Content.ParentWorkbenchWindowController = this;
				}
				
				if(this.View!=null)
					View.SetChild(m_Content.View);
			}
		}

		public void CloseView()
		{
			if(null!=m_Content)
				m_Content.CloseView();

			if(View!=null)
				View.Close();
			
			this.View=null;
		}


		#region ICSharpCode.SharpDevelop.Gui

		/// <summary>
		/// The window title.
		/// </summary>
		public string Title 
		{
			get { return m_Title; }
			set 
			{
				m_Title = value;
				if(View!=null)
					View.SetTitle(m_Title);
			}
		}
		
		/// <summary>
		/// The current view content which is shown inside this window.
		/// </summary>
		public ICSharpCode.SharpDevelop.Gui.IViewContent ViewContent 
		{
			get { return this.Content as ICSharpCode.SharpDevelop.Gui.IViewContent; }
		}
		
		/// <summary>
		/// Closes the window, if force == true it closes the window
		/// without ask, even the content is dirty.
		/// </summary>
		public void CloseWindow(bool force)
		{
			CloseView();
		}
		
		/// <summary>
		/// Brings this window to front and sets the user focus to this
		/// window.
		/// </summary>
		public void SelectWindow()
		{
		}

		
		//		void OnWindowSelected(EventArgs e);
		public void OnWindowDeselected(EventArgs e)
		{
		}
		
		/// <summary>
		/// Is called when the window is selected.
		/// </summary>
		public event EventHandler WindowSelected;
		
		/// <summary>
		/// Is called when the window is deselected.
		/// </summary>
		public event EventHandler WindowDeselected;
		
		/// <summary>
		/// Is called when the title of this window has changed.
		/// </summary>
		public event EventHandler TitleChanged;
		
		/// <summary>
		/// Is called after the window closes.
		/// </summary>
		public event EventHandler CloseEvent;


		#endregion

	}

}
