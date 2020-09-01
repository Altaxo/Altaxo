#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ExceptionHandling;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Interaction logic for SaveErrorChooseDialog.xaml
  /// </summary>
  public partial class ExceptionBox : Window
  {
    private Exception _exceptionThrown;
    private string _message;

    public ExceptionBox()
    {
      InitializeComponent();
    }

    /// <summary>
    /// Creates a new ExceptionBox instance.
    /// </summary>
    /// <param name="exception">The exception to display</param>
    /// <param name="message">An additional message to display</param>
    /// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the
    /// continue button is not available.</param>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    public ExceptionBox(Exception exception, string message, bool mustTerminate)
    {
      _exceptionThrown = exception;
      this._message = message;
      InitializeComponent();
      if (mustTerminate)
      {
        continueButton.Visibility = Visibility.Hidden;
      }

      exceptionTextBox.Text = getClipboardString();

      try
      {
        InitializeText();
      }
      catch
      {
      }
    }

    public class UnhandledHandler : IUnhandledExceptionHandler
    {
      public UnhandledHandler()
      {
      }

      public void EhCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
      {
        ExceptionBox.EhCurrentDomain_UnhandledException(sender, e);
      }

      public void EhWindowsFormsApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
      {
        ExceptionBox.EhFormsApplication_ThreadException(sender, e);
      }

      public void EhWpfDispatcher_UnhandledException(object sender, object dispatcher, Exception exception)
      {
        ExceptionBox.EhDispatcher_UnhandledException(sender, dispatcher, exception);
      }
    }

    private static void EhDispatcher_UnhandledException(object sender, object dispatcher, Exception exception)
    {
      Current.Log.Error("Unhandled WPF exception", exception);
      ShowErrorBox(exception, "Unhandled WPF exception", false);
    }

    private static void EhFormsApplication_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
      Current.Log.Error("ThreadException caught", e.Exception);
      ShowErrorBox(e.Exception, null);
    }

    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
    private static void EhCurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
      var ex = e.ExceptionObject as Exception;
      Current.Log.Fatal("UnhandledException caught", ex);
      if (e.IsTerminating)
        Current.Log.Fatal("Runtime is terminating because of unhandled exception.");
      ShowErrorBox(ex, "Unhandled exception", e.IsTerminating);
    }

    /// <summary>
    /// Displays the exception box.
    /// </summary>
    public static void ShowErrorBox(Exception exception, string message)
    {
      ShowErrorBox(exception, message, false);
    }

    /// <summary>
    /// Variable to avoid reentrant calls. Is true if the ExceptionBox is currently displayed.
    /// </summary>
    [ThreadStatic]
    private static bool _isCurrentlyShowingBox;

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void ShowErrorBox(Exception exception, string message, bool mustTerminate)
    {
      // ignore reentrant calls (e.g. when there's an exception in OnRender)
      if (_isCurrentlyShowingBox)
        return;
      _isCurrentlyShowingBox = true;
      try
      {
        if (exception != null)
        {
          try
          {
          }
          catch (Exception ex)
          {
            Current.Log.Warn("Error tracking exception", ex);
          }
        }
        var box = new ExceptionBox(exception, message, mustTerminate);
        {
          box.ShowDialog();
        }
      }
      catch (Exception ex)
      {
        Current.Log.Warn("Error showing ExceptionBox", ex);
        MessageBox.Show(
            exception != null ? exception.ToString() : "Error",
            message,
            MessageBoxButton.OK,
            MessageBoxImage.Error, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
      }
      finally
      {
        _isCurrentlyShowingBox = false;
      }
    }

    public void InitializeText()
    {
      Title = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.Title}");
      closeButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ExitAltaxo}");
      label3.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ThankYouMsg}");
      label2.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.HelpText2}");
      label.Text = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.HelpText1}");
      continueButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.Continue}");
      reportButton.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.ReportError}");
      copyErrorCheckBox.Content = StringParser.Parse("${res:Altaxo.Gui.ExceptionBox.CopyToClipboard}");
      if (PresentationResourceService.InstanceAvailable)
        _guiImage.Source = PresentationResourceService.GetBitmapSource("Altaxo.Gui.ExceptionBox.Image");
    }

    private void EhCloseButton_Click(object sender, RoutedEventArgs e)
    {
      if (MessageBoxResult.Yes == MessageBox.Show(
          StringParser.Parse("${res:ICSharpCode.SharpDevelop.ExceptionBox.QuitWarning}"),
          MessageService.ProductName,
          MessageBoxButton.YesNo,
          MessageBoxImage.Question,
          MessageBoxResult.No,
          MessageBoxOptions.DefaultDesktopOnly))

      {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
      }
    }

    private void EhContinueButton_Click(object sender, RoutedEventArgs e)
    {
      DialogResult = true;
      Close();
    }

    private void EhReportButton_Click(object sender, RoutedEventArgs e)
    {
      CopyInfoToClipboard();

      StartUrl("https://github.com/Altaxo/altaxo/issues");
    }

    private void CopyInfoToClipboard()
    {
      if (true == copyErrorCheckBox.IsChecked)
      {
        string exceptionText = exceptionTextBox.Text;
        if (System.Windows.Forms.Application.OleRequired() == ApartmentState.STA)
        {
          Clipboard.SetText(exceptionText);
        }
        else
        {
          var th = new Thread((ThreadStart)delegate
          {
            Clipboard.SetText(exceptionText);
          })
          {
            Name = "CopyInfoToClipboard"
          };
          th.SetApartmentState(ApartmentState.STA);
          th.Start();
        }
      }
    }

    private string getClipboardString()
    {
      var sb = new StringBuilder();

      Version version = Assembly.GetEntryAssembly().GetName().Version;
      string versionText = string.Format("Altaxo {0}.{1} build {2}.{3}", version.Major, version.Minor, version.Build, version.Revision);

      sb.Append(versionText);

      sb.AppendLine();

      if (_message != null)
      {
        sb.AppendLine(_message);
      }
      if (_exceptionThrown != null)
      {
        sb.AppendLine("Exception thrown:");
        sb.AppendLine(_exceptionThrown.ToString());
      }
      sb.AppendLine();
      sb.AppendLine("---- Recent log messages:");
      try
      {
        //LogMessageRecorder.AppendRecentLogMessages(sb, log4net.LogManager.GetLogger(typeof(log4netLoggingService)));
      }
      catch (Exception ex)
      {
        sb.AppendLine("Failed to append recent log messages.");
        sb.AppendLine(ex.ToString());
      }
      sb.AppendLine();
      sb.AppendLine("---- Post-error application state information:");
      try
      {
        Altaxo.Current.GetRequiredService<ApplicationStateInfoService>().AppendFormatted(sb);
      }
      catch (Exception ex)
      {
        sb.AppendLine("Failed to append application state information.");
        sb.AppendLine(ex.ToString());
      }
      return sb.ToString();
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void StartUrl(string url)
    {
      try
      {
        System.Diagnostics.Process.Start(url);
      }
      catch (Exception e)
      {
        Current.Log.Warn("Cannot start " + url, e);
      }
    }
  }
}
