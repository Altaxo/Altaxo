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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Workbench;
using Microsoft.Win32;

namespace Altaxo.Main.Services
{
  internal class MessageServiceImpl : IDialogMessageService
  {
    public string DefaultMessageBoxTitle { get; set; }

    public string ProductName { get; set; }

    public MessageServiceImpl()
    {
      DefaultMessageBoxTitle = ProductName = "Altaxo";
    }

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

    public void ShowError(string message)
    {
      Current.Log.Error(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    public void ShowWarning(string message)
    {
      Current.Log.Warn(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
    }

    public void ShowMessage(string message, string? caption)
    {
      caption ??= string.Empty;
      Current.Log.Info(message);
      DoShowMessage(StringParser.Parse(message), StringParser.Parse(caption), MessageBoxImage.Information);
    }

    public void ShowErrorFormatted(string formatstring, params object[] formatitems)
    {
      Current.Log.Error(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
    }

    public void ShowWarningFormatted(string formatstring, params object[] formatitems)
    {
      Current.Log.Warn(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
    }

    public void ShowMessageFormatted(string formatstring, string? caption, params object[] formatitems)
    {
      caption ??= string.Empty;
      Current.Log.Info(formatstring);
      DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse(caption), MessageBoxImage.Information);
    }

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

    private static MessageBoxOptions GetOptions(string text)
    {
      return IsRtlText(text) ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;
    }

    private static bool IsRtlText(string text)
    {
      return false;
    }

    public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
    {
      var messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(messageBox);
      return messageBox.Result;
    }

    public void InformSaveError(PathName fileName, string message, string dialogName, Exception exceptionGot)
    {
      var dlg = new SaveErrorInformDialog(fileName, message, dialogName, exceptionGot);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);
    }

    public ChooseSaveErrorResult ChooseSaveError(PathName fileOrFolderName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
    {
      ChooseSaveErrorResult r = ChooseSaveErrorResult.Ignore;

restartlabel:
      var dlg = new SaveErrorChooseDialog(fileOrFolderName, message, dialogName, exceptionGot, chooseLocationEnabled);
      ((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);

      switch (dlg.DetailedDialogResult)
      {
        case SaveErrorChooseDialog.SaveErrorChooseDialogResult.ChooseLocation:
          {
            if (fileOrFolderName is FileName fileName)
            {
              // choose location:
              var fdiag = new SaveFileDialog
              {
                OverwritePrompt = true,
                AddExtension = true,
                CheckFileExists = false,
                CheckPathExists = true,
                Title = "Choose alternate file name",
                FileName = fileOrFolderName
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
            else if (fileOrFolderName is DirectoryName folderName)
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
              throw new NotImplementedException($"Unhandled type of PathName: {fileOrFolderName?.GetType()}");
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
    /// <param name="caption"></param>
    /// <param name="dialogText"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public string ShowInputBox(string caption, string dialogText, string defaultValue)
    {
      throw new NotImplementedException();
    }

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

    public void WriteLine(MessageLevel level, string source, string message)
    {
      throw new NotImplementedException();
    }

    public void WriteLine(MessageLevel messageLevel, string source, string format, params object[] args)
    {
      throw new NotImplementedException();
    }

    public void WriteLine(MessageLevel messageLevel, string source, IFormatProvider provider, string format, params object[] args)
    {
      throw new NotImplementedException();
    }
  }
}
