using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;

using Altaxo.Main.Services;
using Altaxo.Gui;
using Altaxo.Gui.Common;

namespace Altaxo.Gui
{
	public class WinFormsGuiFactoryService : Altaxo.Main.Services.GUIFactoryService
	{
		private System.Windows.Forms.PageSetupDialog _pageSetupDialog;
		private System.Windows.Forms.PrintDialog _printDialog;


		protected override System.Type GuiControlType
		{
			get
			{
				return typeof(System.Windows.Forms.Control);
			}
		}

		public Form MainWindow
		{
			get
			{
				return (Form)Current.Workbench.ViewObject;
			}
		}

		public override bool InvokeRequired()
		{
			return MainWindow.InvokeRequired;
		}


		public override void Invoke(Action act)
		{
			MainWindow.Invoke(act);
		}


		public override void Invoke(Delegate inv)
		{
			MainWindow.Invoke(inv);
		}


	
		/// <summary>
		/// Shows a configuration dialog for an object.
		/// </summary>
		/// <param name="controller">The controller to show in the dialog</param>
		/// <param name="title">The title of the dialog to show.</param>
		/// <param name="showApplyButton">If true, the "Apply" button is visible on the dialog.</param>
		/// <returns>True if the object was successfully configured, false otherwise.</returns>
		public override bool ShowDialog(IMVCAController controller, string title, bool showApplyButton)
		{

			if (controller.ViewObject == null)
			{
				FindAndAttachControlTo(controller);
			}

			if (controller.ViewObject == null)
				throw new ArgumentException("Can't find a view object for controller of type " + controller.GetType());

			if (controller is Altaxo.Gui.Scripting.IScriptController)
			{
				System.Windows.Forms.Form dlgctrl = new Altaxo.Gui.Scripting.ScriptExecutionDialog((Altaxo.Gui.Scripting.IScriptController)controller);
				return (System.Windows.Forms.DialogResult.OK == dlgctrl.ShowDialog(MainWindow));
			}
			else
			{
				DialogShellView dlgview = new DialogShellView((System.Windows.Forms.UserControl)controller.ViewObject);
				DialogShellController dlgctrl = new DialogShellController(dlgview, controller, title, showApplyButton);
				return DialogResult.OK == dlgview.ShowDialog(MainWindow);
			}
		}

		/// <summary>
		/// Shows a message box with the error text.
		/// </summary>
		/// <param name="errortxt">The error text.</param>
		public override void ErrorMessageBox(string errortxt)
		{
			System.Windows.Forms.MessageBox.Show(MainWindow, errortxt, "Error(s)!", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
		}

		public override void InfoMessageBox(string infotxt)
		{
			System.Windows.Forms.MessageBox.Show(MainWindow, infotxt, "Information", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);
		}

		/// <summary>
		/// Shows a message box with a question to be answered either yes or no.
		/// </summary>
		/// <param name="txt">The question text.</param>
		/// <param name="caption">The caption of the dialog box.</param>
		/// <param name="defaultanswer">If true, the default answer is "yes", otherwise "no".</param>
		/// <returns>True if the user answered with Yes, otherwise false.</returns>
		public override bool YesNoMessageBox(string txt, string caption, bool defaultanswer)
		{
			return System.Windows.Forms.DialogResult.Yes == System.Windows.Forms.MessageBox.Show(MainWindow, txt, caption, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question, defaultanswer ? System.Windows.Forms.MessageBoxDefaultButton.Button1 : System.Windows.Forms.MessageBoxDefaultButton.Button2);
		}

		public override bool ShowBackgroundCancelDialog(int millisecondsDelay, System.Threading.Thread thread, IExternalDrivenBackgroundMonitor monitor)
		{
			for (int i = 0; i < millisecondsDelay && thread.IsAlive; i += 10)
				System.Threading.Thread.Sleep(10);

			if (thread.IsAlive)
			{
				BackgroundCancelDialog dlg = new BackgroundCancelDialog(thread, monitor);

				if (thread.IsAlive)
				{
					System.Windows.Forms.DialogResult r = dlg.ShowDialog(MainWindow);
					return r == System.Windows.Forms.DialogResult.OK ? false : true;
				}
			}
			return false;
		}
		  


		/// <summary>
		/// Shows a page setup dialog.
		/// </summary>
		/// <returns>True if the user close the dialog with OK, false otherwise.</returns>
		public override bool ShowPageSetupDialog()
		{
			bool result = System.Windows.Forms.DialogResult.OK == _pageSetupDialog.ShowDialog(MainWindow);
			if (true == result)
			{
				Current.PrintingService.UpdateBoundsAndMargins();
			}
			return result;
		}
	

		/// <summary>
		/// Shows a print dialog.
		/// </summary>
		/// <returns>True if the user close the dialog with OK, false otherwise.</returns>
		public override bool ShowPrintDialog()
		{
			return System.Windows.Forms.DialogResult.OK == _printDialog.ShowDialog(MainWindow);
		}

		/// <summary>
		/// Sets the print document for both the page setup dialog and the print dialog.
		/// </summary>
		/// <param name="printDocument">The document to set.</param>
		public override void SetPrintDocument(System.Drawing.Printing.PrintDocument printDocument)
		{
			if (_pageSetupDialog == null)
				_pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
			if (_printDialog == null)
				_printDialog = new System.Windows.Forms.PrintDialog();

			_pageSetupDialog.Document = printDocument;
			_printDialog.Document = printDocument;
		}

		public override bool ShowPrintPreviewDialog(PrintPageEventHandler printPageEventHandler, QueryPageSettingsEventHandler queryPageSettingsEventHandler)
		{
			try
			{
				System.Windows.Forms.PrintPreviewDialog dlg = new System.Windows.Forms.PrintPreviewDialog();
				Current.PrintingService.PrintDocument.PrintPage += printPageEventHandler;
				Current.PrintingService.PrintDocument.QueryPageSettings += queryPageSettingsEventHandler;
				dlg.Document = Current.PrintingService.PrintDocument;
				dlg.ShowDialog(MainWindow);
				dlg.Dispose();
				return true;
			}
			catch (Exception ex)
			{
				Current.Gui.ErrorMessageBox(ex.ToString());
			}
			finally
			{
				Current.PrintingService.PrintDocument.PrintPage -= printPageEventHandler;
				Current.PrintingService.PrintDocument.QueryPageSettings -= queryPageSettingsEventHandler;
			}
			return false;
		}

		private string GetFilterString(OpenFileOptions options)
		{
			StringBuilder stb = new StringBuilder();
			foreach (var entry in options.FilterList)
			{
				stb.Append(entry.Value);
				stb.Append('|');
				stb.Append(entry.Key);
				stb.Append('|');
			}
			return stb.ToString();
		}

		public override bool ShowOpenFileDialog(OpenFileOptions options)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = GetFilterString(options);
			dlg.FilterIndex = options.FilterIndex;
			dlg.Multiselect = options.Multiselect;
			if(options.DialogTitle!=null)
				dlg.Title = options.DialogTitle;
			if (options.InitialDirectory != null)
				dlg.InitialDirectory = options.InitialDirectory;
			dlg.RestoreDirectory = options.RestoreDirectory;


			DialogResult r = dlg.ShowDialog(MainWindow);
			if (r == DialogResult.OK)
			{
				options.FileName = dlg.FileName;
				options.FileNames = dlg.FileNames;
			}

			return r == DialogResult.OK;
		}

		public override bool ShowSaveFileDialog(SaveFileOptions options)
		{
			var dlg = new SaveFileDialog();
			dlg.Filter = GetFilterString(options);
			dlg.FilterIndex = options.FilterIndex;
			//dlg.Multiselect = options.Multiselect;
			if (options.DialogTitle != null)
				dlg.Title = options.DialogTitle;
			if (options.InitialDirectory != null)
				dlg.InitialDirectory = options.InitialDirectory;
			dlg.RestoreDirectory = options.RestoreDirectory;

			DialogResult r = dlg.ShowDialog(MainWindow);
			if (r == DialogResult.OK)
			{
				options.FileName = dlg.FileName;
				options.FileNames = dlg.FileNames;
			}
			else
			{
				options.FileName = null;
				options.FileNames = null;
			}

			return r == DialogResult.OK;
		}


		#region From other

	

	

	

		#endregion

	}
}
