using System;
using Altaxo.Main.GUI;
using System.Windows.Forms;
using System.Threading;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for WorkbenchForm.
	/// </summary>
	public class BeautyWorkspaceWindow : System.Windows.Forms.Form, IMdiActivationEventSource, Main.GUI.IWorkbenchWindowView
	{
		IWorkbenchWindowController m_Controller;

		


		public BeautyWorkspaceWindow(System.Windows.Forms.Form parent)
		{

			//if(null!=parent)
				//this.MdiParent = parent;


			// register event so to be informed when activated
			if(parent is IMdiActivationEventSource)
			{
				((IMdiActivationEventSource)parent).MdiChildDeactivateBefore += new EventHandler(this.EhMdiChildDeactivate);
				((IMdiActivationEventSource)parent).MdiChildActivateAfter += new EventHandler(this.EhMdiChildActivate);
			}
			else if(parent!=null)
			{
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildActivate);
				parent.MdiChildActivate += new EventHandler(this.EhMdiChildDeactivate);
			}
		}


		protected void EhMdiChildActivate(object sender, EventArgs e)
		{
			if(null!=this.MdiChildActivateAfter)
				this.MdiChildActivateAfter(sender,e);
		}

		protected void EhMdiChildDeactivate(object sender, EventArgs e)
		{
			if(null!=this.MdiChildDeactivateBefore)
				this.MdiChildDeactivateBefore(sender,e);
		}

		#region IMdiActivationEventSource Members

		public event System.EventHandler MdiChildDeactivateBefore;

		public event System.EventHandler MdiChildActivateBefore;

		public event System.EventHandler MdiChildDeactivateAfter;

		public event System.EventHandler MdiChildActivateAfter;

		#endregion


		EventHandler setTitleEvent = null;
		Crownwood.Magic.Controls.TabPage tabPage = null;
		
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
			OnWindowSelected(EventArgs.Empty);
		}
		
		public void SelectWindow()
		{
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
	}
}
