using System;
using Altaxo.Main.GUI;
using System.Windows.Forms;
using System.Threading;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Windows Form Workspace window contained in a magic tab control page.
	/// </summary>
	public class BeautyWorkspaceWindow 
		: 
		System.Windows.Forms.Form, 
		Main.GUI.IWorkbenchWindowView
	{
		IWorkbenchWindowController m_Controller;

		EventHandler setTitleEvent = null;
		Crownwood.Magic.Controls.TabPage tabPage = null;
		


		public BeautyWorkspaceWindow()
		{
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed (e);
			if(m_Controller!=null)
				m_Controller.EhView_OnClosed();
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if(m_Controller!=null)
				e.Cancel = m_Controller.EhView_OnClosing(e.Cancel);

			base.OnClosing (e);
		}
		

		#region IWorkbenchView Members

		public System.Windows.Forms.Form Form
		{
			get
			{
				return this;
			}
		}

		public IWorkbenchWindowController Controller
		{
			get
			{
				return m_Controller;
			}
			set
			{
				m_Controller = value;
			}
		}

		public void SetChild(IWorkbenchContentView child)
		{
			System.Windows.Forms.Control fc = (System.Windows.Forms.Control)child;
			if(this.Controls.Count>0)
				this.Controls.Clear();

			this.Controls.Add(fc);
			fc.Dock = System.Windows.Forms.DockStyle.Fill;
			
			// the child wants to override my menu, so set it to null
			this.Menu = null;
		}
	
		public void SetTitle(string title)
		{
			this.Text = title;
		}

		public virtual void OnWindowSelected(EventArgs e)
		{
			if (WindowSelected != null) 
			{
				WindowSelected(this, e);
			}
		}
		
		public virtual void OnWindowDeselected(EventArgs e)
		{
			if (WindowDeselected != null) 
			{
				WindowDeselected(this, e);
			}
		}
		
		public event EventHandler WindowSelected;
		public event EventHandler WindowDeselected;
		public event EventHandler TitleChanged;
		public event EventHandler CloseEvent;
	

		#endregion

		public Crownwood.Magic.Controls.TabPage TabPage 
		{
			get 
			{
				return tabPage;
			}
			set 
			{
				tabPage = value;
			}
		}

		/*
		void ThreadSafeSelectWindow()
		{
			if (tabPage  != null) 
			{
				tabPage.Select();
			}
			
			Activate();
			Select();
			this.Controls[0].Focus();
			
			/* LELLID TODO move this in the controller class or elsewhere
			foreach (IViewContent viewContent in WorkbenchSingleton.Workbench.ViewContentCollection) 
			{
				if (viewContent != this.content) 
				{
					viewContent.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);
				}
			}
			*/
		/*
			OnWindowSelected(EventArgs.Empty);
		}
		*/


		public void SelectWindow()
		{
			System.Console.WriteLine("BeautyWorkspaceWindow::SelectWindow called");

			if(null!=Controller)
				Controller.SelectWindow();

			
			/* LELLID temporarily removed 
			
			try 
			{
				MethodInvoker mi = new MethodInvoker(this.ThreadSafeSelectWindow);
				EndInvoke(this.BeginInvoke(mi));
				Thread.Sleep(0);
			} 
			catch (ThreadInterruptedException) 
			{
				//Simply exit....
			}
			catch (Exception) 
			{
			}
			*/
		}
	}
}
