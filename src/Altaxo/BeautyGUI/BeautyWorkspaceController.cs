using System;
using System.Collections;

namespace Altaxo.Main.GUI
{

	public class BeautyWorkspaceController 
		:
		IWorkbenchWindowController,
		ICSharpCode.SharpDevelop.Gui.IWorkbenchWindow	
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
					this.Title = m_Content.ContentName;
				
					if(this.View!=null)
						View.SetChild(m_Content.WorkbenchContentView);
				}
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

		public void EhView_OnClosed()
		{
			this.View = null;

			// before the childs get the close event,
			// we sent out the message that the window is deselected
			if(null!=WindowDeselected)
				WindowDeselected(null,null);

			if(this.m_Content!=null)
				m_Content.HostWindowClosed();
		}

		public bool EhView_OnClosing(bool cancel)
		{
			bool bCancelContent=false;

			if(this.m_Content!=null)
				bCancelContent = this.m_Content.HostWindowClosing();
			
			return cancel | bCancelContent;
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
				string oldTitle = m_Title;

				m_Title = value;
				if(View!=null)
					View.SetTitle(m_Title);

				if(oldTitle!=m_Title && null!=this.TitleChanged)
					TitleChanged(this,EventArgs.Empty);
			}
		}
		
		/// <summary>
		/// The current view content which is shown inside this window.
		/// </summary>
		public ICSharpCode.SharpDevelop.Gui.IViewContent ViewContent 
		{
			get { return this.Content as ICSharpCode.SharpDevelop.Gui.IViewContent; }
		}
		

		public ArrayList SubViewContents 
		{
			get 
			{
				throw new NotImplementedException("SubViewContent is not implemented now");
			
				/*
				return subViewContents;
				*/
			}
		}
		
		public ICSharpCode.SharpDevelop.Gui.IBaseViewContent ActiveViewContent 
		{
			get 
			{
				return this.m_Content;

				/*
				if (viewTabControl != null && viewTabControl.SelectedIndex > 0) 
				{
					return (IBaseViewContent)subViewContents[viewTabControl.SelectedIndex - 1];
				}
				return content;
				
				*/
			}
		}

		public void SwitchView(int viewNumber)
		{

			throw new NotImplementedException();

			/*
			if (viewTabControl != null) 
			{
				this.viewTabControl.SelectedIndex = viewNumber;
			}
			*/

			
		}

		public void AttachSecondaryViewContent(ICSharpCode.SharpDevelop.Gui.ISecondaryViewContent subViewContent)
		{
			throw new NotImplementedException();
			/*
			TabPage newPage;
			
			if (subViewContents == null) 
			{
				subViewContents = new ArrayList();
				
				viewTabControl      = new TabControl();
				viewTabControl.Alignment = TabAlignment.Bottom;
				viewTabControl.Dock = DockStyle.Fill;
				viewTabControl.SelectedIndexChanged += new EventHandler(viewTabControlIndexChanged);
				
				tabPage.Controls.Clear();
				tabPage.Controls.Add(viewTabControl);
				
				newPage = new TabPage(stringParserService.Parse(content.TabPageText));
				newPage.Tag = content;
				content.Control.Dock = DockStyle.Fill;
				newPage.Controls.Add(content.Control);
				viewTabControl.TabPages.Add(newPage);
			}
			subViewContent.WorkbenchWindow = this;
			subViewContents.Add(subViewContent);
			
			newPage = new TabPage(stringParserService.Parse(subViewContent.TabPageText));
			newPage.Tag = subViewContent;
			subViewContent.Control.Dock = DockStyle.Fill;
			newPage.Controls.Add(subViewContent.Control);
			viewTabControl.TabPages.Add(newPage);
		}
		
		int oldIndex = -1;
		void viewTabControlIndexChanged(object sender, EventArgs e)
		{
			if (oldIndex > 0) 
			{
				ISecondaryViewContent secondaryViewContent = subViewContents[oldIndex - 1] as ISecondaryViewContent;
				if (secondaryViewContent != null) 
				{
					secondaryViewContent.Deselected();
				}
			}
			
			if (viewTabControl.SelectedIndex > 0) 
			{
				ISecondaryViewContent secondaryViewContent = subViewContents[viewTabControl.SelectedIndex - 1] as ISecondaryViewContent;
				if (secondaryViewContent != null) 
				{
					secondaryViewContent.Selected();
				}
			}
			oldIndex = viewTabControl.SelectedIndex;
			*/

		}

		/// <summary>
		/// Closes the window, if force == true it closes the window
		/// without ask, even the content is dirty.
		/// </summary>
		public void CloseWindow(bool force)
		{
			if(force)
			{
				CloseView();
			}
			else
			{
				if(View!=null)
					View.Close();
			}
		}
		
		/// <summary>
		/// Brings this window to front and sets the user focus to this
		/// window.
		/// </summary>
		public void SelectWindow()
		{
			
			if(this.WindowSelected!=null)
				WindowSelected(null,null);
		}

		
		//		void OnWindowSelected(EventArgs e);
		public void OnWindowDeselected(EventArgs e)
		{
			if(null!=WindowDeselected)
				WindowDeselected(null,e);
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
