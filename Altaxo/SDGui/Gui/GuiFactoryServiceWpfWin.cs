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
  public class GuiFactoryServiceWpfWin : Altaxo.Main.Services.GUIFactoryService
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

    public IWin32Window MainWindow
    {
      get
      {
        return (IWin32Window)Current.Workbench.ViewObject;
      }
    }

		public System.Windows.Window MainWindowWpf
		{
			get
			{
				return (System.Windows.Window)Current.Workbench.ViewObject;
			}
		}


    public override bool InvokeRequired()
    {
      return Current.Workbench.SynchronizingObject.InvokeRequired;
    }


    public override object Invoke(Delegate act, object[] args)
    {
      return Current.Workbench.SynchronizingObject.Invoke(act,args);
    }

		public override IAsyncResult BeginInvoke(Delegate act, object[] args)
		{
			return Current.Workbench.SynchronizingObject.BeginInvoke(act, args);
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
			return Evaluate(InternalShowDialog, controller, title, showApplyButton);
		}

			  /// <summary>
    /// Shows a configuration dialog for an object.
    /// </summary>
    /// <param name="controller">The controller to show in the dialog</param>
    /// <param name="title">The title of the dialog to show.</param>
    /// <param name="showApplyButton">If true, the "Apply" button is visible on the dialog.</param>
    /// <returns>True if the object was successfully configured, false otherwise.</returns>
    private bool InternalShowDialog(IMVCAController controller, string title, bool showApplyButton)
    {

      if (controller.ViewObject == null)
      {
        FindAndAttachControlTo(controller);
      }

      if (controller.ViewObject == null)
        throw new ArgumentException("Can't find a view object for controller of type " + controller.GetType());

      if (controller is Altaxo.Gui.Scripting.IScriptController)
      {
        var dlgctrl = new Altaxo.Gui.Scripting.ScriptExecutionDialog((Altaxo.Gui.Scripting.IScriptController)controller);
				dlgctrl.Owner = MainWindowWpf;
        return (true == dlgctrl.ShowDialog());
      }
			else if (controller.ViewObject is System.Windows.UIElement)
			{
				var dlgview = new DialogShellViewWpf((System.Windows.UIElement)controller.ViewObject);
				var dlgctrl = new DialogShellController(dlgview, controller, title, showApplyButton);
				return true == dlgview.ShowDialog();
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
		public override void ErrorMessageBox(string errortxt, string title)
		{
			Evaluate(System.Windows.MessageBox.Show, MainWindowWpf, errortxt, title ?? "Error(s)!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
		}

    public override void InfoMessageBox(string infotxt, string title)
    {
      Evaluate(System.Windows.MessageBox.Show,MainWindowWpf, infotxt, title ?? "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
			return System.Windows.MessageBoxResult.Yes == Evaluate(System.Windows.MessageBox.Show, MainWindowWpf, txt, caption, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, defaultanswer ? System.Windows.MessageBoxResult.OK : System.Windows.MessageBoxResult.No);
    }

    /// <summary>
    /// Shows a message box with a questtion to be answered either by YES, NO, or CANCEL.
    /// </summary>
    /// <param name="txt">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultAnswer">If true, the default answer is "yes", if false the default answer is "no", if null the default answer is "Cancel".</param>
    /// <returns>True if the user answered with Yes, false if the user answered No, null if the user pressed Cancel.</returns>
    public override bool? YesNoCancelMessageBox(string text, string caption, bool? defaultAnswer)
    {
      var defaultButton = System.Windows.MessageBoxResult.Cancel;
      if(defaultAnswer!=null)
				defaultButton = ((bool)defaultAnswer) ? System.Windows.MessageBoxResult.Yes : System.Windows.MessageBoxResult.No;

			var result = Evaluate(System.Windows.MessageBox.Show, MainWindowWpf, text, caption, System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question, defaultButton);

			if (result == System.Windows.MessageBoxResult.Yes)
        return true;
			else if (result == System.Windows.MessageBoxResult.No)
      return false;
      else
      return null;
    }


    public override bool ShowBackgroundCancelDialog(int millisecondsDelay, IExternalDrivenBackgroundMonitor monitor, System.Threading.Thread thread)
    {
			if (InvokeRequired())
				throw new ApplicationException("Trying to show a BackgroundCancelDialog initiated by a background thread. This nesting is not supported"); 

      for (int i = 0; i < millisecondsDelay && thread.IsAlive; i += 10)
        System.Threading.Thread.Sleep(10);

      if (thread.IsAlive)
      {
        var dlg = new BackgroundCancelDialogWpf(thread, monitor);
        if (thread.IsAlive)
        {
					dlg.Owner = MainWindowWpf;
					return true == dlg.ShowDialog();
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
      if (stb.Length > 0)
        stb.Length -= 1; // account for the trailing | char

      return stb.ToString();
    }

		public override bool ShowOpenFileDialog(OpenFileOptions options)
		{
			if (InvokeRequired())
			{
				return (bool)MainWindowWpf.Dispatcher.Invoke((Func<OpenFileOptions, bool>)InternalShowOpenFileDialog, new object[] { options });
			}
			else
			{
				return InternalShowOpenFileDialog(options);
			}
		}

		private bool InternalShowOpenFileDialog(OpenFileOptions options)
		{
			var dlg = new Microsoft.Win32.OpenFileDialog();

			dlg.Filter = GetFilterString(options);
			dlg.FilterIndex = options.FilterIndex;
			dlg.Multiselect = options.Multiselect;
			if (options.Title != null)
				dlg.Title = options.Title;
			if (options.InitialDirectory != null)
				dlg.InitialDirectory = options.InitialDirectory;
			dlg.RestoreDirectory = options.RestoreDirectory;


			if(true==dlg.ShowDialog(MainWindowWpf))
			{
				options.FileName = dlg.FileName;
				options.FileNames = dlg.FileNames;
				return true;
			}
			else
			return false;
		}


		public override bool ShowSaveFileDialog(SaveFileOptions options)
		{
			if (InvokeRequired())
				return (bool)MainWindowWpf.Dispatcher.Invoke((Func<SaveFileOptions, bool>)InternalShowSaveFileDialog, new object[] { options });
			else
				return InternalShowSaveFileDialog(options);
		}

    private bool InternalShowSaveFileDialog(SaveFileOptions options)
    {
			var dlg = new Microsoft.Win32.SaveFileDialog();
      dlg.Filter = GetFilterString(options);
      dlg.FilterIndex = options.FilterIndex;
      //dlg.Multiselect = options.Multiselect;
      if (options.Title != null)
        dlg.Title = options.Title;
      if (options.InitialDirectory != null)
        dlg.InitialDirectory = options.InitialDirectory;
      dlg.RestoreDirectory = options.RestoreDirectory;

      if(true== dlg.ShowDialog(MainWindowWpf))
      {
        options.FileName = dlg.FileName;
        options.FileNames = dlg.FileNames;
				return true;
      }
      else
      {
        options.FileName = null;
        options.FileNames = null;
				return false;
      }
		}

		#region Clipboard

		private class ClipDataWrapper : System.Windows.Forms.DataObject, IClipboardSetDataObject
		{
			public void SetCommaSeparatedValues(string text) { this.SetData(DataFormats.CommaSeparatedValue, text); }
		}
		private class ClipGetDataWrapper : IClipboardGetDataObject
		{
			System.Windows.Forms.DataObject _dao;

			public ClipGetDataWrapper(System.Windows.Forms.DataObject value)
			{
				_dao = value;
			}

			public string[] GetFormats() { return _dao.GetFormats(); }
			public bool GetDataPresent(string format) { return _dao.GetDataPresent(format); }
			public bool GetDataPresent(System.Type type) { return _dao.GetDataPresent(type); }
			public object GetData(string format) { return _dao.GetData(format); }
			public object GetData(System.Type type) { return _dao.GetData(type); }
			public bool ContainsFileDropList() { return _dao.ContainsFileDropList(); }
			public System.Collections.Specialized.StringCollection GetFileDropList() { return _dao.GetFileDropList(); }
			public bool ContainsImage() { return _dao.ContainsImage(); }
			public System.Drawing.Image GetImage() { return _dao.GetImage(); }

		}

		public override IClipboardSetDataObject GetNewClipboardDataObject()
		{
			return new ClipDataWrapper();
		}

		public override IClipboardGetDataObject OpenClipboardDataObject()
		{
			var dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;
			return new ClipGetDataWrapper(dao);
		}


		public override void SetClipboardDataObject(IClipboardSetDataObject dataObject, bool copy)
		{
			System.Windows.Forms.Clipboard.SetDataObject(dataObject, copy);
		}


		#endregion


		#region Context menu

		/// <summary>
		/// Creates and shows a context menu.
		/// </summary>
		/// <param name="x">The x coordinate of the location where to show the context menu.</param>
		/// <param name="y">The y coordinate of the location where to show the context menu.</param>
		/// <param name="owner">The object that will be owner of this context menu.</param>
		/// <param name="addInTreePath">Add in tree path used to build the context menu.</param>
		/// <returns>The context menu. Returns Null if there is no registered context menu provider</returns>
		public override void ShowContextMenu(object parent, object owner, string addInTreePath, double x, double y)
		{
			foreach (var entry in RegistedContextMenuProviders)
			{
				if (ReflectionService.IsSubClassOfOrImplements(parent.GetType(), entry.Key))
				{
					entry.Value(parent, owner, addInTreePath, x, y);
					return;
				}
			}
		}

		#endregion


	
	}
}
