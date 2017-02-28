using Altaxo.Gui;
using Altaxo.Gui.Common;
using Altaxo.Gui.Workbench;
using ICSharpCode.Core;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Altaxo.Main.Services
{
	internal class MessageServiceImpl : IDialogMessageService
	{
		public string DefaultMessageBoxTitle { get; set; }

		public string ProductName { get; set; }

		public MessageServiceImpl()
		{
			this.DefaultMessageBoxTitle = this.ProductName = "Altaxo";
		}

		public virtual void ShowException(Exception ex, string message)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			message = StringParser.Parse(message);
			if (ex != null)
			{
				message += "\n\nException occurred: " + ex.ToString();
			}
			DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
		}

		private void DoShowMessage(string message, string caption, MessageBoxImage icon)
		{
			var mainWindow = Application.Current?.MainWindow;

			if (null != mainWindow)
			{
				Current.Gui.BeginExecute(
					() =>
					{
						MessageBox.Show(mainWindow,
														message, caption ?? DefaultMessageBoxTitle,
														MessageBoxButton.OK, MessageBoxImage.Warning,
														MessageBoxResult.OK, GetOptions(message, caption));
					});
			}
			else
			{
				// If we do not have a main window, we probably don't have a Gui thread context either
				MessageBox.Show(
														message, caption ?? DefaultMessageBoxTitle,
														MessageBoxButton.OK, MessageBoxImage.Warning,
														MessageBoxResult.OK, GetOptions(message, caption));
			}
		}

		public void ShowError(string message)
		{
			LoggingService.Error(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
		}

		public void ShowWarning(string message)
		{
			LoggingService.Warn(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
		}

		public void ShowMessage(string message, string caption)
		{
			LoggingService.Info(message);
			DoShowMessage(StringParser.Parse(message), StringParser.Parse(caption), MessageBoxImage.Information);
		}

		public void ShowErrorFormatted(string formatstring, params object[] formatitems)
		{
			LoggingService.Error(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
		}

		public void ShowWarningFormatted(string formatstring, params object[] formatitems)
		{
			LoggingService.Warn(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse("${res:Global.WarningText}"), MessageBoxImage.Warning);
		}

		public void ShowMessageFormatted(string formatstring, string caption, params object[] formatitems)
		{
			LoggingService.Info(formatstring);
			DoShowMessage(StringParser.Format(formatstring, formatitems), StringParser.Parse(caption), MessageBoxImage.Information);
		}

		public bool AskQuestion(string question, string caption)
		{
			var mainWindow = ((GuiFactoryServiceWpfWin)Current.Gui).MainWindowWpf;

			var result = Current.Gui.Evaluate(
				() => MessageBox.Show(mainWindow,
																	 StringParser.Parse(question),
																	 StringParser.Parse(caption ?? "${res:Global.QuestionText}"),
																	 MessageBoxButton.YesNo,
																	 MessageBoxImage.Question,
																	 MessageBoxResult.Yes,
																	 GetOptions(question, caption))
				);
			return result == MessageBoxResult.Yes;
		}

		private static MessageBoxOptions GetOptions(string text, string caption)
		{
			return IsRtlText(text) ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;
		}

		private static bool IsRtlText(string text)
		{
			return false;
		}

		public int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts);
			((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(messageBox);
			return messageBox.Result;
		}

		public void InformSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot)
		{
			SaveErrorInformDialog dlg = new SaveErrorInformDialog(fileName, message, dialogName, exceptionGot);
			((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);
		}

		public ChooseSaveErrorResult ChooseSaveError(FileName fileName, string message, string dialogName, Exception exceptionGot, bool chooseLocationEnabled)
		{
			ChooseSaveErrorResult r = ChooseSaveErrorResult.Ignore;

			restartlabel:
			SaveErrorChooseDialog dlg = new SaveErrorChooseDialog(fileName, message, dialogName, exceptionGot, chooseLocationEnabled);
			((GuiFactoryServiceWpfWin)Current.Gui).ShowDialog(dlg);

			switch (dlg.DetailedDialogResult)
			{
				case SaveErrorChooseDialog.SaveErrorChooseDialogResult.ChooseLocation:
					{
						// choose location:
						SaveFileDialog fdiag = new SaveFileDialog();

						fdiag.OverwritePrompt = true;
						fdiag.AddExtension = true;
						fdiag.CheckFileExists = false;
						fdiag.CheckPathExists = true;
						fdiag.Title = "Choose alternate file name";
						fdiag.FileName = fileName;
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

		public void ShowHandledException(Exception ex, string message = null)
		{
			LoggingService.Error(message, ex);
			LoggingService.Warn("Stack trace of last exception log:\n" + Environment.StackTrace);
			if (message == null)
			{
				message = ex.Message;
			}
			else
			{
				message = StringParser.Parse(message) + "\r\n\r\n" + ex.Message;
			}
			DoShowMessage(message, StringParser.Parse("${res:Global.ErrorText}"), MessageBoxImage.Error);
		}
	}
}