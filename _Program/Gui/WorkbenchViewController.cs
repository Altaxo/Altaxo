using System;

namespace Altaxo.Gui
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
		IWorkbenchWindowView View { get; set; }
		IWorkbenchContentController Content { get; set; }
		void CloseView();

	}

	public interface IWorkbenchWindowView
	{
		System.Windows.Forms.Form Form { get; }
		IWorkbenchWindowController Controller { get; set; }
		void SetChild(IWorkbenchContentView content);
		void Close();
	}

	public class WorkbenchWindowController : IWorkbenchWindowController
	{
		protected IWorkbenchWindowView m_View;
		protected IWorkbenchContentController m_Content;


		public IWorkbenchWindowView View
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
	}

}
