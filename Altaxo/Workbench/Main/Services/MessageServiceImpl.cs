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
using System.Windows;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Workbench;
using Microsoft.Win32;

namespace Altaxo.Main.Services
{
  /// <summary>
  /// Implements dialog-based message handling for the workbench.
  /// </summary>
  internal class MessageServiceImpl : IDialogMessageService
  {
    /// <summary>
    /// Gets or sets the default message box title.
    /// </summary>
    public string DefaultMessageBoxTitle { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageServiceImpl"/> class.
    /// </summary>
    public MessageServiceImpl()
    {
      DefaultMessageBoxTitle = ProductName = "Altaxo";
    }

    /// <summary>
    /// Shows an exception message.
    /// </summary>
    /// <param name="ex">The exception to show.</param>
    /// <param name="message">The optional message text.</param>
    public virtual void ShowException(Exception ex, string? message)
    {
      message ??= string.Empty;
      Current.Log.Error(message, ex);
      Current.Log.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
      message = StringParser.Parse(message);
      if (ex is not null)
      {
        message += "\n\nException occurred: " + ex.ToString();
      }
      DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    /// <summary>
    /// Shows a message box on the UI thread if necessary.
    /// </summary>
    /// <param name="message">The message text.</param>
    /// <param name="caption">The message caption.</param>
    /// <param name="icon">The message icon.</param>
    private void DoShowMessage(string message, string caption, MessageBoxImage icon)
    {
      if (Current.GetService<IDispatcherMessageLoop>() is { } dispatcher && dispatcher.InvokeRequired)
      {
        dispatcher.InvokeAndForget(
            () =>
            {
              var mainWindow = Application.Current?.MainWindow;
              if (mainWindow is not null)
                MessageBox.Show(mainWindow,
                                                        message, caption ?? DefaultMessageBoxTitle,
                                                        MessageBoxButton.OK, MessageBoxImage.Warning,
                                                        MessageBoxResult.OK, GetOptions(message));
              else
                MessageBox.Show(
                                                       message, caption ?? DefaultMessageBoxTitle,
                                                       MessageBoxButton.OK, MessageBoxImage.Warning,
                                                       MessageBoxResult.OK, GetOptions(message));

            });
      }
      else
      {
        // If we do not have a main window, we probably don't have a Gui thread context either
        MessageBox.Show(
                                                message, caption ?? DefaultMessageBoxTitle,
                                                MessageBoxButton.OK, MessageBoxImage.Warning,
                                                MessageBoxResult.OK, GetOptions(message));
      }
    }

    /// <summary>
    /// Shows an error message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    public void ShowError(string message)
    {
      Current.Log.Error(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    /// <summary>
    /// Shows a warning message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    public void ShowWarning(string message)
    {
      Current.Log.Warn(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
    }

    /// <summary>
    /// Shows an information message.
    /// </summary>
    /// <param name="message">The message to show.</param>
    /// <param name="caption">The message caption.</param>
    public void ShowMessage(string message, string? caption)
    {
      caption ??= string.Empty;
      Current.Log.Info(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse(caption), MessageBoxImage.Information);
    }

    /// <summary>
    /// Shows a formatted error message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowErrorFormatted(string formatstring, params object[] formatitems)
    {
      Current.Log.Error(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    /// <summary>
    /// Shows a formatted warning message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowWarningFormatted(string formatstring, params object[] formatitems)
    {
      Current.Log.Warn(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
    }

    /// <summary>
    /// Shows a formatted information message.
    /// </summary>
    /// <param name="formatstring">The composite format string.</param>
    /// <param name="caption">The message caption.</param>
    /// <param name="formatitems">The format arguments.</param>
    public void ShowMessageFormatted(string formatstring, string? caption, params object[] formatitems)
    {
      caption ??= string.Empty;
      Current.Log.Info(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse(caption), MessageBoxImage.Information);
    }

    /// <summary>
    /// Asks the user a yes or no question.
    /// </summary>
    /// <param name="question">The question text.</param>
    /// <param name="caption">The message caption.</param>
    /// <returns><see langword="true"/> if the answer is yes; otherwise, <see langword="false"/>.</returns>
    public bool AskQuestion(string question, string? caption)
    {
      caption ??= string.Empty;
      var mainWindow = ((GuiFactoryServiceWpfWin)Current.Gui).MainWindowWpf;

      var result = Current.Dispatcher.InvokeIfRequired(
          () => MessageBox.Show(mainWindow,
                                                               StringParser.Parse(question),
                                                               StringParser.Parse(caption ?? "${res:Global.QuestionText}"),
                                                               MessageBoxButton.YesNo,
                                                               MessageBoxImage.Question,
                                                               MessageBoxResult.Yes,
                                                               GetOptions(question))
          );
      return result == MessageBoxResult.Yes;
    }

    /// <summary>
    /// Gets message box options for the specified text.
    /// </summary>
    /// <param name="text">The text to inspect.</param>
    /// <returns>The matching message box options.</returns>
    private static MessageBoxOptions GetOptions(string text)
    {
      return IsRtlText(text) ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;
    }

    /// <summary>
    /// Determines whether the text is right-to-left.
    /// </summary>
    /// <param name="text">The text to inspect.</param>
    /// <returns><see langword="true"/> if the text is right-to-left; otherwise, <see langword="false"/>.</returns>
    private static bool IsRtlText(string text)
    {
      return false;
    }

    /// <summary>
    /// Shows a custom dialog.
    /// </summary>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="dialogText">The dialog text.</param>
    /// <param name="acceptButtonIndex">The accept button index.</param>
    /// <param name="cancelButtonIndex">The cancel button index.</param>
    /// <param name="buttontexts">The dialog button texts.</param>
    /// <returns>The dialog result.</returns>
    public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
    {
      var messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(messageBox);
      return messageBox.Result;
    }

    /// <summary>
    /// Shows a dialog for a save error.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="message">The message text.</param>
    /// <param name="dialogName">The dialog name.</param>
    /// <param name="exceptionGot">The exception.</param>
    public void InformSaveError(PathName fileName, string message, string dialogName, Exception exceptionGot)
    {
      var dlg = new SaveErrorInformDialog(fileName, message, dialogName, exceptionGot);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);
    }

    /// <summary>
    /// Shows a dialog for choosing how to handle a save error.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="message">The message text.</param>
    /// <param name="dialogName">The dialog name.</param>
    /// <param name="exceptionGot">The exception.</param>
    /// <param name="chooseLocationEnabled">Whether choosing a different location is enabled.</param>
    /// <returns>The chosen action.</returns>
    public ChooseSaveErrorResult ChooseSaveError(PathName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      ChooseSaveErrorResult r = ChooseSaveErrorResult.Ignore;

restartlabel:
      var dlg = new SaveErrorChooseDialog(fileName, message, dialogName, exceptionGot, chooseLocationEnabled);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);

      switch (dlg.DetailedDialogResult)
      {
        case SaveErrorChooseDialog.SaveErrorChooseDialogResult.ChooseLocation:
          {
            if (fileName is FileName fileNameX)
            {
              // choose location:
              var fdiag = new SaveFileDialog
              {
                OverwritePrompt = true,
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                Title = "Choose alternate file name",
                FileName = fileName
              };
              if (fdiag.ShowDialog() == true)
              {
                r = ChooseSaveErrorResult.SaveAlternative(FileName.Create(fdiag.FileName));
                break;
              }
              else
              {
                goto restartlabel;
              }
            }
            else if (fileName is DirectoryName folderNameX)
            {
              var fdiag = new System.Windows.Forms.FolderBrowserDialog();
              if (System.Windows.Forms.DialogResult.OK == fdiag.ShowDialog())
              {
                r = ChooseSaveErrorResult.SaveAlternative(DirectoryName.Create(fdiag.SelectedPath));
                break;
              }
              else
              {
                goto restartlabel;
              }
            }
            else
            {
              throw new NotImplementedException($"Unhandled type of PathName: {fileName?.GetType()}");
            }
          }
        case SaveErrorChooseDialog.SaveErrorChooseDialogResult.Retry:
          r = ChooseSaveErrorResult.Retry;
          break;

        default:
          r = ChooseSaveErrorResult.Ignore;
          break;
      }

      return r;
    }

    /// <summary>
    /// Shows an input box.
    /// </summary>
    /// <param name="caption">The dialog caption.</param>
    /// <param name="dialogText">The dialog text.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The entered text.</returns>
    public string ShowInputBox(string caption, string dialogText, string defaultValue)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Shows a handled exception.
    /// </summary>
    /// <param name="ex">The exception to show.</param>
    /// <param name="message">The optional message text.</param>
    public void ShowHandledException(Exception ex, string? message = null)
    {
      message ??= string.Empty;
      Current.Log.Error(message, ex);
      Current.Log.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
      if (message is null)
      {
        message = ex.Message;
      }
      else
      {
        message = StringParser.Parse(message) + "\r\n\r\n" + ex.Message;
      }
      DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    /// <summary>
    /// Writes a message line.
    /// </summary>
    /// <param name="level">The message level.</param>
    /// <param name="source">The source.</param>
    /// <param name="message">The message text.</param>
    public void WriteLine(MessageLevel level, string source, string message)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Writes a formatted message line.
    /// </summary>
    /// <param name="messageLevel">The message level.</param>
    /// <param name="source">The source.</param>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    public void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Writes a formatted message line using the specified format provider.
    /// </summary>
    /// <param name="messageLevel">The message level.</param>
    /// <param name="source">The source.</param>
    /// <param name="provider">The format provider.</param>
    /// <param name="format">The format string.</param>
    /// <param name="args">The format arguments.</param>
    public void WriteLine(MessageLevel messageLevel, string source, IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }
  }
}
