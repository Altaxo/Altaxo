using System;
using ICSharpCode.SharpDevelop.Gui;

namespace Altaxo.Main.GUI
{
	
	public class WindowsMdiWorkbenchLayout : ICSharpCode.SharpDevelop.Gui.IWorkbenchLayout
	{

		protected IWorkbenchWindow m_ActiveWorkbenchWindow;
		protected IWorkbench       m_Workbench;
		protected System.Windows.Forms.Form m_WorkbenchForm;

		/// <summary>
		/// The active workbench window.
		/// </summary>
		public IWorkbenchWindow ActiveWorkbenchwindow 
		{
			get 
			{
				return m_ActiveWorkbenchWindow;
			}
		}
		
		/// <summary>
		/// Attaches this layout manager to a workbench object.
		/// </summary>
		public void Attach(IWorkbench workbench)
		{
			m_Workbench = workbench;

			// 
			if(workbench.ViewObject!=null)
			{
				System.Diagnostics.Debug.Assert(workbench.ViewObject is System.Windows.Forms.Form,"The workbench view must be a windows form in order to match the layout manager");
				m_WorkbenchForm = (System.Windows.Forms.Form)workbench.ViewObject;
				m_WorkbenchForm.IsMdiContainer = true;
				m_WorkbenchForm.MdiChildActivate += new EventHandler(this.EhView_MdiChildActivate);				
			}
		}
		
		/// <summary>
		/// Detaches this layout manager from the current workspace.
		/// </summary>
		public void Detach()
		{
			// Shut down the selection change handler first - since a lot of changes
			// are now to do
			if(null!=m_WorkbenchForm)
			{
				m_WorkbenchForm.MdiChildActivate -= new EventHandler(this.EhView_MdiChildActivate);				
				
				// strip all the windows from the content
				foreach (System.Windows.Forms.Form form in m_WorkbenchForm.MdiChildren) 
				{
					Altaxo.Main.GUI.IWorkbenchWindowView wv = form as Altaxo.Main.GUI.IWorkbenchWindowView;
					if(wv!=null)
					{
						IWorkbenchWindow f = (IWorkbenchWindow)wv.Controller;
						wv.Controller.Content=null; // strip off the content
						//f.ViewContent = null;
						wv.SetChild(null); // strip of the content view
						f.CloseWindow(true); // now the window can be safely closed without harming the content
					}
				}

				m_WorkbenchForm.IsMdiContainer = false;
			}

			m_Workbench = null;
			m_WorkbenchForm = null;
		}
		
		/// <summary>
		/// Shows a new <see cref="IPadContent"/>.
		/// </summary>
		public void ShowPad(IPadContent content)
		{
		}
		
		/// <summary>
		/// Activates a pad (Show only makes it visible but Activate does
		/// bring it to foreground)
		/// </summary>
		public void ActivatePad(IPadContent content)
		{
		}
		
		/// <summary>
		/// Hides a new <see cref="IPadContent"/>.
		/// </summary>
		public void HidePad(IPadContent content)
		{
		}
		
		/// <summary>
		/// returns true, if padContent is visible;
		/// </summary>
		public bool IsVisible(IPadContent padContent)
		{
			return false;
		}
		
		/// <summary>
		/// Re-initializes all components of the layout manager.
		/// </summary>
		public void RedrawAllComponents()
		{
		}
		
		/// <summary>
		/// Shows a new <see cref="IViewContent"/>.
		/// </summary>
		public IWorkbenchWindow ShowView(IViewContent content)
		{
			m_Workbench.ViewContentCollection.Add(content);

			Altaxo.Main.GUI.IWorkbenchWindowController wbv_controller = new Altaxo.Main.GUI.WorkbenchWindowController();
			Altaxo.Main.GUI.WorkbenchForm wbvform = new Altaxo.Main.GUI.WorkbenchForm();
			wbvform.MdiParent = this.m_WorkbenchForm;
			wbv_controller.View = wbvform;

			wbv_controller.Content = (Altaxo.Main.GUI.IWorkbenchContentController)content;
			
			wbvform.Show();			
			content.WorkbenchWindow.SelectWindow();
			return wbv_controller;
		}
		

		/// <summary>
		/// Is called from the view when the currently active MDI child window has changed
		/// </summary>
		protected void EhView_MdiChildActivate(object sender, System.EventArgs e)
		{
			System.Windows.Forms.Form activatedWindow = m_WorkbenchForm.ActiveMdiChild;

			if(activatedWindow is Altaxo.Main.GUI.WorkbenchForm)
			{
				m_ActiveWorkbenchWindow = ((Altaxo.Main.GUI.WorkbenchForm)activatedWindow).Controller;
			
			
				foreach(IViewContent content in m_Workbench.ViewContentCollection)
					if(!object.Equals(content.WorkbenchWindow,m_ActiveWorkbenchWindow))
						content.WorkbenchWindow.OnWindowDeselected(EventArgs.Empty);

				// TODO change this to .OnSelectWindow since the window is already selected
				m_ActiveWorkbenchWindow.SelectWindow();
			}

			OnActiveWindowChanged(this,EventArgs.Empty);
		}


		void OnActiveWindowChanged(object sender, EventArgs e)
		{
			if (ActiveWorkbenchWindowChanged != null) 
			{
				ActiveWorkbenchWindowChanged(this, e);
			}
		}

		/// <summary>
		/// Is called, when the workbench window which the user has into
		/// the foreground (e.g. editable) changed to a new one.
		/// </summary>
		public event EventHandler ActiveWorkbenchWindowChanged;
	}
}
