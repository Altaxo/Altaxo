using System;
using System.Windows.Forms;
using Altaxo.Serialization;

namespace Altaxo.Main.GUI
{
	/// <summary>
	/// Summary description for WorkbenchForm.
	/// </summary>
	public class WorkbenchForm 
		:
		System.Windows.Forms.Form,
		Main.GUI.IWorkbenchWindowView
	{
		IWorkbenchWindowController m_Controller;

		

		public WorkbenchForm()
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

		public Form Form
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

		#endregion
	}
}
