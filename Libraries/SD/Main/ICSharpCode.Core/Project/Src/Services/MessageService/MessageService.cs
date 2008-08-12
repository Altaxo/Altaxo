﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2079 $</version>
// </file>

using System;
using System.Text;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	/// <summary>
	/// Class with static methods to show message boxes.
	/// </summary>
	public static class MessageService
	{
		static Form mainForm;
		
		public static Form MainForm {
			get {
				return mainForm;
			}
			set {
				mainForm = value;
			}
		}
		
		public static void ShowError(Exception ex)
		{
			ShowError(ex, null);
		}
		
		public static void ShowError(string message)
		{
			ShowError(null, message);
		}
		
		public static void ShowErrorFormatted(string formatstring, params string[] formatitems)
		{
			ShowError(null, Format(formatstring, formatitems));
		}
		
		
		public delegate void ShowErrorDelegate(Exception ex, string message);
		
		static ShowErrorDelegate customErrorReporter;
		
		/// <summary>
		/// Gets/Sets the custom error reporter. If this property is null, the default
		/// messagebox is used.
		/// </summary>
		public static ShowErrorDelegate CustomErrorReporter {
			get {
				return customErrorReporter;
			}
			set {
				customErrorReporter = value;
			}
		}
		
		public static void ShowError(Exception ex, string message)
		{
			if (message == null) message = string.Empty;
			
			if (ex != null) {
				LoggingService.Error(message, ex);
				if (customErrorReporter != null) {
					customErrorReporter(ex, message);
					return;
				}
			} else {
				LoggingService.Error(message);
			}
			
			string msg = message + "\n\n";
			
			if (ex != null) {
				msg += "Exception occurred: " + ex.ToString();
			}
			
			if (MessageService.MainForm == null) {
				MessageBox.Show(StringParser.Parse(msg), StringParser.Parse("SharpDevelop: ${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
			} else {
				
				MethodInvoker showError = delegate {
					MessageBox.Show(MessageService.MainForm, StringParser.Parse(msg), StringParser.Parse("${res:Global.ErrorText}"), MessageBoxButtons.OK, MessageBoxIcon.Error);
				};
				if (MessageService.MainForm.InvokeRequired) {
					MessageService.MainForm.BeginInvoke(showError);
				} else {
					showError();
				}
			}
		}
		
		public static void ShowWarning(string message)
		{
			message = StringParser.Parse(message);
			LoggingService.Warn(message);
			
			string caption = StringParser.Parse("${res:Global.WarningText}");
			if (MessageService.MainForm == null) {
				MessageBox.Show(message, caption,
				                MessageBoxButtons.OK, MessageBoxIcon.Warning,
				                MessageBoxDefaultButton.Button1, GetOptions(message, caption));
			} else {
				MethodInvoker showWarning = delegate {
					MessageBox.Show(MessageService.MainForm,
					                message, caption,
					                MessageBoxButtons.OK, MessageBoxIcon.Warning,
					                MessageBoxDefaultButton.Button1, GetOptions(message, caption));
				};
				if (MessageService.MainForm.InvokeRequired) {
					MessageService.MainForm.BeginInvoke(showWarning);
				} else {
					showWarning();
				}
			}
		}
		
		public static void ShowWarningFormatted(string formatstring, params string[] formatitems)
		{
			ShowWarning(Format(formatstring, formatitems));
		}
		
		public static bool AskQuestion(string question, string caption)
		{
			return MessageBox.Show(MessageService.MainForm,
			                       StringParser.Parse(question),
			                       StringParser.Parse(caption),
			                       MessageBoxButtons.YesNo,
			                       MessageBoxIcon.Question,
			                       MessageBoxDefaultButton.Button1,
			                       GetOptions(question, caption))
				== DialogResult.Yes;
		}
		
		static MessageBoxOptions GetOptions(string text, string caption)
		{
			return IsRtlText(text) ? MessageBoxOptions.RtlReading | MessageBoxOptions.RightAlign : 0;
		}
		
		static bool IsRtlText(string text)
		{
			if (!RightToLeftConverter.IsRightToLeft)
				return false;
			foreach (char c in StringParser.Parse(text)) {
				if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
					return true;
			}
			return false;
		}
		
		public static bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems), caption);
		}
		
		public static bool AskQuestionFormatted(string formatstring, params string[] formatitems)
		{
			return AskQuestion(Format(formatstring, formatitems));
		}
		
		public static bool AskQuestion(string question)
		{
			return AskQuestion(StringParser.Parse(question), StringParser.Parse("${res:Global.QuestionText}"));
		}
		
		public static int ShowCustomDialog(string caption, string dialogText, int acceptButtonIndex, int cancelButtonIndex, params string[] buttontexts)
		{
			using (CustomDialog messageBox = new CustomDialog(caption, dialogText, acceptButtonIndex, cancelButtonIndex, buttontexts)) {
				messageBox.ShowDialog(MessageService.MainForm);
				return messageBox.Result;
			}
		}
		
		public static int ShowCustomDialog(string caption, string dialogText, params string[] buttontexts)
		{
			return ShowCustomDialog(caption, dialogText, -1, -1, buttontexts);
		}
		
		public static string ShowInputBox(string caption, string dialogText, string defaultValue)
		{
			using (InputBox inputBox = new InputBox(dialogText, caption, defaultValue)) {
				inputBox.ShowDialog(MessageService.MainForm);
				return inputBox.Result;
			}
		}
		
		static string defaultMessageBoxTitle = "MessageBox";
		static string productName = "Application Name";
		
		public static string ProductName {
			get {
				return productName;
			}
			set {
				productName = value;
			}
		}
		
		public static string DefaultMessageBoxTitle {
			get {
				return defaultMessageBoxTitle;
			}
			set {
				defaultMessageBoxTitle = value;
			}
		}
		
		public static void ShowMessage(string message)
		{
			ShowMessage(message, DefaultMessageBoxTitle);
		}
		
		public static void ShowMessageFormatted(string formatstring, params string[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems));
		}
		
		public static void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems)
		{
			ShowMessage(Format(formatstring, formatitems), caption);
		}
		
		public static void ShowMessage(string message, string caption)
		{
			message = StringParser.Parse(message);
			LoggingService.Info(message);
			MessageBox.Show(mainForm,
			                message,
			                StringParser.Parse(caption),
			                MessageBoxButtons.OK,
			                MessageBoxIcon.Information,
			                MessageBoxDefaultButton.Button1,
			                GetOptions(message, caption));
		}
		
		static string Format(string formatstring, string[] formatitems)
		{
			try {
				return String.Format(StringParser.Parse(formatstring), formatitems);
			} catch (FormatException) {
				StringBuilder b = new StringBuilder(StringParser.Parse(formatstring));
				foreach(string formatitem in formatitems) {
					b.Append("\nItem: ");
					b.Append(formatitem);
				}
				return b.ToString();
			}
		}
	}
}
