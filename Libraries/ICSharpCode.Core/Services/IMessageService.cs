// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Resources;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Xml;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Properties;

namespace ICSharpCode.Core.Services
{
	/// <summary>
	/// This interface must be implemented by all services.
	/// </summary>
	public interface IMessageService
	{
		void ShowError(Exception ex);
		void ShowError(string message);
		void ShowError(Exception ex, string message);
		void ShowErrorFormatted(string formatstring, params string[] formatitems);
		
		void ShowWarning(string message);
		void ShowWarningFormatted(string formatstring, params string[] formatitems);
		
		void ShowMessage(string message);
		void ShowMessage(string message, string caption);
		void ShowMessageFormatted(string formatstring, params string[] formatitems);
		void ShowMessageFormatted(string caption, string formatstring, params string[] formatitems);
		
		/// <summary>
		/// returns the number of the chosen button
		/// </summary>
		int  ShowCustomDialog(string caption, 
		                      string dialogText,
		                      params string[] buttontexts);
		
		bool AskQuestion(string question);
		bool AskQuestionFormatted(string formatstring, params string[] formatitems);
		bool AskQuestion(string question, string caption);
		bool AskQuestionFormatted(string caption, string formatstring, params string[] formatitems);
	}
}
