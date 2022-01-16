#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2011 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Input;
using Altaxo.Geometry;
using Altaxo.Gui.Common;
using Altaxo.Main.Services;

namespace Altaxo.Gui
{
  public class GuiFactoryServiceWpfWin : Altaxo.Main.Services.GUIFactoryService
  {
    private Stack<System.Windows.Window> _modalWindows = new Stack<System.Windows.Window>();

    public GuiFactoryServiceWpfWin()
    {
      RegisteredGuiTechnologies.Add(typeof(System.Windows.FrameworkElement));
      RegistedContextMenuProviders.Add(typeof(System.Windows.FrameworkElement), ShowWpfContextMenu);
    }

    private static void ShowWpfContextMenu(object parent, object owner, string addInPath, double x, double y)
    {
      Altaxo.Gui.AddInItems.MenuService.ShowContextMenu((System.Windows.UIElement)parent, owner, addInPath);
    }

    /// <inheritdoc/>
    public override ICommand NewRelayCommand(Action execute, Func<bool>? canExecute = null)
    {
      var result = new RelayCommand(execute, canExecute);
      CommandManager.RequerySuggested += result.EhRequerySuggested; // use the CommandManager instead our own event, because CommandManager maintains a weak reference only
      return result;
    }

    /// <inheritdoc/>
    public override ICommand NewRelayCommand(Action<object> execute, Predicate<object>? canExecute = null)
    {
      var result = new RelayCommand<object>(execute, canExecute);
      CommandManager.RequerySuggested += result.EhRequerySuggested; // use the CommandManager instead our own event, because CommandManager maintains a weak reference only
      return result;
    }

    #region Still dependent on Windows Forms

    public override IntPtr MainWindowHandle
    {
      get
      {
        var visual = (System.Windows.Media.Visual)Current.Workbench.ViewObject;
        var wnd = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.IWin32Window;
        if (wnd is not null)
          return wnd.Handle;
        else
          return IntPtr.Zero;
      }
    }

    public override object MainWindowObject => Current.Workbench.ViewObject;

    public override RectangleD2D GetScreenInformation(double virtual_x, double virtual_y)
    {
      var wa = System.Windows.Forms.Screen.GetWorkingArea(new System.Drawing.Point((int)virtual_x, (int)virtual_y));

      return new RectangleD2D(wa.X, wa.Y, wa.Width, wa.Height);
    }

    #endregion Still dependent on Windows Forms

    public System.Windows.Window MainWindowWpf
    {
      get
      {
        return (System.Windows.Window)Current.Workbench.ViewObject;
      }
    }

    /// <summary>
    /// Gets the topmost modal window. This is either the main windows of Altaxo (if no dialog is open), or the topmost modal dialog window.
    /// </summary>
    /// <value>
    /// The topmost modal window.
    /// </value>
    public System.Windows.Window TopmostModalWindow
    {
      get
      {
        return 0 != _modalWindows.Count ? _modalWindows.Peek() : MainWindowWpf;
      }
    }

    internal bool? InternalShowModalWindow(System.Windows.Window window)
    {
      if (window is null)
        throw new ArgumentNullException(nameof(window));

      window.Owner = TopmostModalWindow;
      _modalWindows.Push(window);
      try
      {
        return window.ShowDialog();
      }
      finally
      {
        _modalWindows.Pop();
      }
    }

    private PointD2D _screenResolution;

    /// <summary>Gets the screen resolution that is set in windows in dots per inch.</summary>
    public override PointD2D ScreenResolutionDpi
    {
      get
      {
        if (_screenResolution.IsEmpty)
        {
          if (Current.Workbench.ViewObject is null)
            return new PointD2D(96, 96); // until we have a workbench, we assume 96 dpi
          var MainWindowPresentationSource = System.Windows.PresentationSource.FromVisual((System.Windows.Window)Current.Workbench.ViewObject);
          if (MainWindowPresentationSource is null)
            return new PointD2D(96, 96); // until we have a valid presentation source, we assume 96 dpi
          var m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
          _screenResolution = new PointD2D(96 * m.M11, 96 * m.M22);
        }
        return _screenResolution;
      }
    }

    private delegate void ActionDelegate();

    /// <summary>
    /// Shows a window as a modal window.
    /// </summary>
    /// <param name="window">The window.</param>
    /// <returns></returns>
    public bool? ShowDialog(System.Windows.Window window)
    {
      return Current.Dispatcher.InvokeIfRequired(InternalShowModalWindow, window);
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
      return Current.Dispatcher.InvokeIfRequired(InternalShowDialog, controller, title, showApplyButton);
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
      if (controller.ViewObject is null)
      {
        FindAndAttachControlTo(controller);
      }

      if (controller.ViewObject is null)
        throw new ArgumentException("Can't find a view object for controller of type " + controller.GetType());

      double startLocationLeft, startLocationTop;
      if (TopmostModalWindow.WindowState == System.Windows.WindowState.Maximized)
      {
        // if the topmost window is maximized, then the Top and Left property are nonsense
        // see https://stackoverflow.com/questions/9812756/window-top-and-left-values-are-not-updated-correctly-when-maximizing-a-window-in
        // about screen see here: https://social.msdn.microsoft.com/Forums/vstudio/en-US/2ca2fab6-b349-4c08-915f-373c71bd636a/show-and-maximize-wpf-window-on-a-specific-screen?forum=wpf
        var screen = ScreenHandler.GetCurrentScreen(TopmostModalWindow);
        startLocationLeft = screen.Bounds.X;
        startLocationTop = screen.Bounds.Y;
      }
      else
      {
        startLocationTop = TopmostModalWindow.Top;
        startLocationLeft = TopmostModalWindow.Left;
      }

      if (controller.ViewObject is IViewRequiresSpecialShellWindow specialView)
      {
        var dlgctrl = (System.Windows.Window?)Activator.CreateInstance(specialView.TypeOfShellWindowRequired, controller) ??
                      throw new InvalidOperationException($"Unable to create instance of type {specialView.TypeOfShellWindowRequired}");

        dlgctrl.Owner = TopmostModalWindow;
        dlgctrl.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
        dlgctrl.Top = startLocationTop;
        dlgctrl.Left = startLocationLeft;
        return (true == InternalShowModalWindow(dlgctrl));
      }

      if (controller.ViewObject is System.Windows.UIElement)
      {
        var dlgview = new DialogShellViewWpf((System.Windows.UIElement)controller.ViewObject)
        {
          Owner = TopmostModalWindow,
          WindowStartupLocation = System.Windows.WindowStartupLocation.Manual,
          Top = startLocationTop,
          Left = startLocationLeft
        };

        var dlgctrl = new DialogShellController(dlgview, controller, title, showApplyButton);
        return true == InternalShowModalWindow(dlgview);
      }
      else
      {
        throw new NotSupportedException("This type of UIElement is not supported: " + controller.ViewObject.GetType().ToString());
        /*
DialogShellView dlgview = new DialogShellView((System.Windows.Forms.UserControl)controller.ViewObject);
DialogShellController dlgctrl = new DialogShellController(dlgview, controller, title, showApplyButton);
return System.Windows.Forms.DialogResult.OK == dlgview.ShowDialog(MainWindow);
*/
      }
    }

    /// <summary>
    /// Shows a message box with the error text.
    /// </summary>
    /// <param name="errortxt">The error text.</param>
    /// <param name="title">The titel (header) of the message box.</param>
    public override void ErrorMessageBox(string errortxt, string title)
    {
      Current.Dispatcher.InvokeIfRequired(() => MessageBox_GuiContextOnly(errortxt, title ?? "Error(s)!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error));
    }

    private void MessageBox_GuiContextOnly(string errortext, string title, System.Windows.MessageBoxButton button, System.Windows.MessageBoxImage image)
    {
      errortext = errortext ?? string.Empty;

      // Due to a bug? in MessageBox it does not show up if the message text is very long
      // In order to avoid that, we limit the number of characters.
      if (errortext.Length > 2000)
      {
        errortext = errortext.Substring(0, 2000) + "...";
      }

      var result = System.Windows.MessageBox.Show(errortext, title, button, image);
    }

    public override void InfoMessageBox(string infotxt, string title)
    {
      Current.Dispatcher.InvokeIfRequired(() => MessageBox_GuiContextOnly(infotxt, title ?? "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information));
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
      if (Current.Workbench is not null)
        return System.Windows.MessageBoxResult.Yes == Current.Dispatcher.InvokeIfRequired(System.Windows.MessageBox.Show, txt, caption, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, defaultanswer ? System.Windows.MessageBoxResult.OK : System.Windows.MessageBoxResult.No);
      else
        return System.Windows.MessageBoxResult.Yes == System.Windows.MessageBox.Show(txt, caption, System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question, defaultanswer ? System.Windows.MessageBoxResult.OK : System.Windows.MessageBoxResult.No);
    }

    /// <summary>
    /// Shows a message box with a questtion to be answered either by YES, NO, or CANCEL.
    /// </summary>
    /// <param name="text">The question text.</param>
    /// <param name="caption">The caption of the dialog box.</param>
    /// <param name="defaultAnswer">If true, the default answer is "yes", if false the default answer is "no", if null the default answer is "Cancel".</param>
    /// <returns>True if the user answered with Yes, false if the user answered No, null if the user pressed Cancel.</returns>
    public override bool? YesNoCancelMessageBox(string text, string caption, bool? defaultAnswer)
    {
      var defaultButton = System.Windows.MessageBoxResult.Cancel;
      if (defaultAnswer is not null)
        defaultButton = ((bool)defaultAnswer) ? System.Windows.MessageBoxResult.Yes : System.Windows.MessageBoxResult.No;

      var result = Current.Dispatcher.InvokeIfRequired(System.Windows.MessageBox.Show, text, caption, System.Windows.MessageBoxButton.YesNoCancel, System.Windows.MessageBoxImage.Question, defaultButton);

      if (result == System.Windows.MessageBoxResult.Yes)
        return true;
      else if (result == System.Windows.MessageBoxResult.No)
        return false;
      else
        return null;
    }

    private string GetFilterString(OpenFileOptions options)
    {
      var stb = new StringBuilder();
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
      return Current.Dispatcher.InvokeIfRequired(InternalShowOpenFileDialog, options);
    }

    private bool InternalShowOpenFileDialog(OpenFileOptions options)
    {
      var dlg = new Microsoft.Win32.OpenFileDialog
      {
        Filter = GetFilterString(options),
        FilterIndex = options.FilterIndex,
        Multiselect = options.Multiselect
      };
      if (options.Title is not null)
        dlg.Title = options.Title;
      if (options.InitialDirectory is not null && System.IO.Directory.Exists(options.InitialDirectory))
        dlg.InitialDirectory = options.InitialDirectory;
      dlg.RestoreDirectory = options.RestoreDirectory;

      if (true == dlg.ShowDialog(TopmostModalWindow))
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
      return Current.Dispatcher.InvokeIfRequired(InternalShowSaveFileDialog, options);
    }

    private bool InternalShowSaveFileDialog(SaveFileOptions options)
    {
      var dlg = new Microsoft.Win32.SaveFileDialog
      {
        Filter = GetFilterString(options),
        FilterIndex = options.FilterIndex
      };
      //dlg.Multiselect = options.Multiselect;
      if (options.Title is not null)
        dlg.Title = options.Title;
      if (options.InitialDirectory is not null && System.IO.Directory.Exists(options.InitialDirectory))
        dlg.InitialDirectory = options.InitialDirectory;
      dlg.RestoreDirectory = options.RestoreDirectory;
      dlg.OverwritePrompt = options.OverwritePrompt;
      dlg.DefaultExt = options.DefaultExt;
      dlg.AddExtension = options.AddExtension;
      dlg.FileName = options.FileName;

      if (true == dlg.ShowDialog(TopmostModalWindow))
      {
        options.FileName = dlg.FileName;
        options.FileNames = dlg.FileNames;
        return true;
      }
      else
      {
        options.FileName = string.Empty;
        options.FileNames = SaveFileOptions.EmptyStringArray;
        return false;
      }
    }

    public override bool ShowFolderDialog(FolderChoiceOptions options)
    {
      return Current.Dispatcher.InvokeIfRequired(InternalShowFolderDialog, options);
    }

    private bool InternalShowFolderDialog(FolderChoiceOptions options)
    {
      var dlg = new System.Windows.Forms.FolderBrowserDialog()
      {
        ShowNewFolderButton = options.ShowNewFolderButton,
        Description = options.Description,
      };

      if (options.RootFolder.HasValue)
        dlg.RootFolder = options.RootFolder.Value;
      if (!string.IsNullOrEmpty(options.SelectedPath))
        dlg.SelectedPath = options.SelectedPath;

      if(dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
        options.SelectedPath = dlg.SelectedPath;
        return true;
      }
      else
      {
        return false;
      }

    }

    #region Clipboard

      /* old WinForm Clipboard wrappers

  private class ClipDataWrapper : System.Windows.Forms.DataObject, IClipboardSetDataObject
  {
      public void SetCommaSeparatedValues(string text) { this.SetData(System.Windows.Forms.DataFormats.CommaSeparatedValue, text); }
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
  */

    private class WpfClipSetDataWrapper : IClipboardSetDataObject
    {
      private System.Windows.DataObject _dao = new System.Windows.DataObject();

      public System.Windows.IDataObject DataObject { get { return _dao; } }

      public void SetImage(System.Drawing.Image image)
      {
        _dao.SetData(image);
      }

      public void SetFileDropList(System.Collections.Specialized.StringCollection filePaths)
      {
        _dao.SetFileDropList(filePaths);
      }

      public void SetData(string format, object data)
      {
        _dao.SetData(format, data);
      }

      public void SetData(Type format, object data)
      {
        _dao.SetData(format, data);
      }

      public void SetCommaSeparatedValues(string text)
      {
        _dao.SetData("Csv", text);
      }
    }

    private class WpfClipGetDataWrapper : IClipboardGetDataObject
    {
      private System.Windows.DataObject _dao;

      public WpfClipGetDataWrapper(System.Windows.DataObject value)
      {
        _dao = value;
      }

      public string[] GetFormats()
      {
        return _dao.GetFormats();
      }

      public bool GetDataPresent(string format)
      {
        return _dao.GetDataPresent(format);
      }

      public bool GetDataPresent(System.Type type)
      {
        return _dao.GetDataPresent(type);
      }

      public object GetData(string format)
      {
        return _dao.GetData(format);
      }

      public object GetData(System.Type type)
      {
        return _dao.GetData(type);
      }

      public bool ContainsFileDropList()
      {
        return _dao.ContainsFileDropList();
      }

      public System.Collections.Specialized.StringCollection GetFileDropList()
      {
        return _dao.GetFileDropList();
      }

      public bool ContainsImage()
      {
        return _dao.ContainsImage();
      }

      public System.Drawing.Image? GetImage()
      {
        try
        {
          if (_dao.GetDataPresent("EnhancedMetafile"))
            return (System.Drawing.Imaging.Metafile)_dao.GetData("EnhancedMetafile");
          else if (_dao.GetDataPresent("System.Drawing.Imaging.Metafile"))
            return (System.Drawing.Imaging.Metafile)_dao.GetData("System.Drawing.Imaging.Metafile");
          else if (_dao.GetDataPresent("System.Drawing.Bitmap"))
            return (System.Drawing.Bitmap)_dao.GetData("System.Drawing.Bitmap");
        }
        catch (Exception)
        {
        }

        return null;
      }

      public (System.IO.Stream? Stream, string? FileExtension) GetBitmapImageAsOptimizedMemoryStream()
      {
        {
          if (_dao.GetData("PNG", false) is System.IO.MemoryStream stream)
            return (stream, ".png");
        }

        {
          if (_dao.GetData("png", false) is System.IO.MemoryStream stream)
            return (stream, ".png");
        }
        {
          if (_dao.GetData("JPG", false) is System.IO.MemoryStream stream)
            return (stream, ".jpg");
        }
        {
          if (_dao.GetData("JPEG", false) is System.IO.MemoryStream stream)
            return (stream, ".jpg");
        }

        {
          if (_dao.GetData("System.Windows.Media.Imaging.BitmapSource", true) is System.Windows.Media.Imaging.BitmapSource bitmapSource)
            return StreamFromBitmapSource(bitmapSource);
        }

        {
          if (_dao.GetData("System.Drawing.Bitmap", true) is System.Drawing.Bitmap sysDrawBitmap)
            return StreamFromSystemDrawingBitmap(sysDrawBitmap);
        }

        if (_dao.GetImage() is System.Windows.Media.Imaging.BitmapSource bmpSource)
        {
          return StreamFromBitmapSource(bmpSource);
        }

        return (null, null);
      }

      private (System.IO.Stream, string fileExtension) StreamFromBitmapSource(System.Windows.Media.Imaging.BitmapSource bmpSource)
      {
        var pngStream = new System.IO.MemoryStream();
        var pngEncoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
        var pngFrame = System.Windows.Media.Imaging.BitmapFrame.Create(bmpSource);
        pngEncoder.Frames.Add(pngFrame);
        pngEncoder.Save(pngStream);
        pngStream.Seek(0, System.IO.SeekOrigin.Begin);

        var jpgStream = new System.IO.MemoryStream();
        var jpgEncoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
        var jpgFrame = System.Windows.Media.Imaging.BitmapFrame.Create(bmpSource);
        jpgEncoder.Frames.Add(jpgFrame);
        jpgEncoder.Save(jpgStream);
        jpgStream.Seek(0, System.IO.SeekOrigin.Begin);

        var stream = pngStream.Length < jpgStream.Length ? pngStream : jpgStream;
        var strExt = pngStream.Length < jpgStream.Length ? ".png" : ".jpg";
        var altStream = pngStream.Length < jpgStream.Length ? jpgStream : pngStream;
        altStream.Dispose();

        return (stream, strExt);
      }

      private (System.IO.Stream, string fileExtension) StreamFromSystemDrawingBitmap(System.Drawing.Bitmap sysDrawBitmap)
      {
        var pngStream = ImageToStream(sysDrawBitmap, System.Drawing.Imaging.ImageFormat.Png);
        var jpgStream = ImageToStream(sysDrawBitmap, System.Drawing.Imaging.ImageFormat.Jpeg);
        if (pngStream.Length < jpgStream.Length)
        {
          jpgStream.Dispose();
          return (pngStream, ".png");
        }
        else
        {
          pngStream.Dispose();
          return (jpgStream, ".jpg");
        }
      }
    }
    public static System.IO.MemoryStream ImageToStream(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
    {
      var str = new System.IO.MemoryStream();
      image.Save(str, format);
      str.Flush();
      str.Seek(0, System.IO.SeekOrigin.Begin);
      return str;
    }

    public override IClipboardSetDataObject GetNewClipboardDataObject()
    {
      return new WpfClipSetDataWrapper();
    }

    public override IClipboardGetDataObject OpenClipboardDataObject()
    {
      //var dao = System.Windows.Forms.Clipboard.GetDataObject() as System.Windows.Forms.DataObject;
      //return new ClipGetDataWrapper(dao);

      var dao = (System.Windows.DataObject)System.Windows.Clipboard.GetDataObject();
      return new WpfClipGetDataWrapper(dao);
    }

    public override void SetClipboardDataObject(IClipboardSetDataObject dataObject, bool copy)
    {
      //System.Windows.Forms.Clipboard.SetDataObject(dataObject, copy);
      System.Windows.Clipboard.SetDataObject(((WpfClipSetDataWrapper)dataObject).DataObject, copy);
    }

    #endregion Clipboard

    #region Context menu

    /// <summary>
    /// Creates and shows a context menu.
    /// </summary>
    /// <param name="parent">Parent class of this context menu. This determines the Gui technology to be used.</param>
    /// <param name="owner">The object that will be owner of this context menu.</param>
    /// <param name="addInTreePath">Add in tree path used to build the context menu.</param>
    /// <param name="x">The x coordinate of the location where to show the context menu.</param>
    /// <param name="y">The y coordinate of the location where to show the context menu.</param>
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

    public override bool ShowBackgroundCancelDialog(int millisecondsDelay, Thread thread, IExternalDrivenBackgroundMonitor monitor)
    {
      if (Current.Dispatcher.InvokeRequired)
        throw new ApplicationException("Trying to show a BackgroundCancelDialog initiated by a background thread. This nesting is not supported");

      for (int i = 0; i < millisecondsDelay && thread.IsAlive; i += 10)
        System.Threading.Thread.Sleep(10);

      if (thread.IsAlive)
      {
        var dlg = new BackgroundCancelDialogWpf(thread, monitor);
        if (thread.IsAlive)
        {
          dlg.Owner = MainWindowWpf;
          return true == InternalShowModalWindow(dlg);
        }
      }
      return false;
    }

    public override bool ShowTaskCancelDialog(int millisecondsDelay, System.Threading.Tasks.Task task, CancellationTokenSource ctsSoft, CancellationTokenSource ctsHard, IExternalDrivenBackgroundMonitor monitor)
    {
      if (Current.Dispatcher.InvokeRequired)
        throw new ApplicationException("Trying to show a BackgroundCancelDialog initiated by a background thread. This nesting is not supported");

      for (int i = 0; i < millisecondsDelay && !task.IsCompleted; i += 10)
        System.Threading.Thread.Sleep(10);

      if (!task.IsCompleted)
      {
        var dlg = new TaskCancelDialog() { DataContext = new TaskCancelController(task, ctsSoft, ctsHard, monitor, 0) };
        if (!task.IsCompleted)
        {
          dlg.Owner = MainWindowWpf;
          return true == InternalShowModalWindow(dlg);
        }
      }
      return false;
    }

    #endregion Context menu

    #region Commands

    public override void RegisterRequerySuggestedHandler(EventHandler handler)
    {
      CommandManager.RequerySuggested += handler;
    }

    public override void UnregisterRequerySuggestedHandler(EventHandler handler)
    {
      CommandManager.RequerySuggested -= handler;
    }

    #endregion Commands
  }
}
