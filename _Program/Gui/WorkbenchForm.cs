using System;
using System.Windows.Forms;
namespace Altaxo.Gui
{
	/// <summary>
	/// Summary description for WorkbenchForm.
	/// </summary>
	public class WorkbenchForm : System.Windows.Forms.Form
	{
		public WorkbenchForm(System.Windows.Forms.Form parent)
		{

			if(null!=parent)
				this.MdiParent = parent;


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


		protected override void OnClosed(System.EventArgs e)
		{
			// if(null!=m_Ctrl)	m_Ctrl.EhView_Closed(e);
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			// if(null!=m_Ctrl)	m_Ctrl.EhView_Closing(e);
		}

		protected void EhMdiChildActivate(object sender, EventArgs e)
		{
			/*
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild==this)
			{
				
				// if no toolbar present already, create a toolbar
				if(null==m_GraphToolsToolBar)
					m_GraphToolsToolBar = CreateGraphToolsToolbar();

				// restore the parent - so the toolbar is shown
				m_GraphToolsToolBar.Parent = (System.Windows.Forms.Form)(App.Current);
			
				}
				*/
		}

		protected void EhMdiChildDeactivate(object sender, EventArgs e)
		{
			/*
			if(((System.Windows.Forms.Form)sender).ActiveMdiChild!=this)
			{
				
				if(null!=m_GraphToolsToolBar)
					m_GraphToolsToolBar.Parent=null;
			
				}
		*/
		}
	}
}
