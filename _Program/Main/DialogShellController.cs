using System;

namespace Altaxo.Main
{
	#region Interfaces

	/// <summary>
	/// This interface is intended to provide a "shell" as a dialog which can host a user control.
	/// </summary>
	public interface IDialogShellView
	{
		/// <summary>
		/// Returns either the view itself if the view is a form, or the form where this view is contained into, if it is a control or so.
		/// </summary>
		System.Windows.Forms.Form Form { get; }

		/// <summary>
		/// Get / sets the controler of this view.
		/// </summary>
		IDialogShellController Controller { get; set; }
	}

	/// <summary>
	/// This interface can be used by all controllers where the user input needs to
	/// be applied to the document being controlled.
	/// </summary>
	public interface IApplyController
	{
		/// <summary>
		/// Called when the user input has to be applied to the document being controlled.
		/// </summary>
		/// <returns>True if the apply was successfull, otherwise false.</returns>
		/// <remarks>This function is called in two cases: Either the user pressed OK or the user pressed Apply.</remarks>
		bool Apply();
	}

	/// <summary>
	/// Interface to the DialogShellController.
	/// </summary>
	public interface IDialogShellController
	{
		/// <summary>
		/// Called when the user presses the OK button. 
		/// </summary>
		void EhOK();
		
		/// <summary>
		/// Called when the user presses the Cancel button.
		/// </summary>
		void EhCancel();
		
		/// <summary>
		/// Called when the user presses the Apply button.
		/// </summary>
		void EhApply();
	}

	#endregion

	/// <summary>
	/// Controls the <see cref="DialogShellView"/>.
	/// </summary>
	public class DialogShellController : IDialogShellController
	{
		private IDialogShellView m_View;
		private IApplyController m_HostedController;


		/// <summary>
		/// Creates the controller.
		/// </summary>
		/// <param name="view">The view this controller is controlling.</param>
		/// <param name="hostedController">The controller that controls the UserControl shown in the client area of the form.</param>
		public DialogShellController(IDialogShellView view, IApplyController hostedController)
		{
			View = view;
			m_HostedController = hostedController;
		}

		/// <summary>
		/// Get / sets the view of this controller.
		/// </summary>
		IDialogShellView View
		{
			get { return m_View; }
			set
			{
				m_View = value;
				m_View.Controller = this;
			}
		}

		/// <summary>
		/// Shows the form as modal dialog.
		/// </summary>
		/// <param name="owner">The owner of this form.</param>
		/// <returns>True onto success (the user presses OK).</returns>
		public bool ShowDialog(System.Windows.Forms.Form owner)
		{
			return System.Windows.Forms.DialogResult.OK==m_View.Form.ShowDialog(owner);
		}


		#region IDialogShellController Members

		/// <summary>
		/// Called when the user presses the OK button. Calls the Apply method of the
		/// hosted controller, then closes the form.
		/// </summary>
		public void EhOK()
		{
			bool bSuccess = true;
			if(null!=m_HostedController)
				bSuccess = m_HostedController.Apply();

			if(bSuccess) // if successfull applied, close the form
			{
				View.Form.DialogResult = System.Windows.Forms.DialogResult.OK;
				View.Form.Close();
			}
		}

		/// <summary>
		/// Called when the user presses the Cancel button. Then closes the form.
		/// </summary>
		public void EhCancel()
		{
			View.Form.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			View.Form.Close();
		}

		/// <summary>
		/// Called when the user presses the Apply button. Calls the Apply method of the
		/// hosted controller.
		/// </summary>
		public void EhApply()
		{
			bool bSuccess = true;
			if(null!=m_HostedController)
				bSuccess = m_HostedController.Apply();
		}

		#endregion
	}
}
