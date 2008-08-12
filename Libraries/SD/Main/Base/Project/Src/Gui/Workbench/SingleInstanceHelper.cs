// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Kr�ger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2533 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	public static class SingleInstanceHelper
	{
		const int CUSTOM_MESSAGE = NativeMethods.WM_USER + 2;
		const int RESULT_FILES_HANDLED = 2;
		const int RESULT_PROJECT_IS_OPEN = 3;
		
		public static bool OpenFilesInPreviousInstance(string[] fileList)
		{
			LoggingService.Debug("Trying to pass arguments to previous instance...");
			int currentProcessId = Process.GetCurrentProcess().Id;
			string currentFile = Assembly.GetEntryAssembly().Location;
			int number = new Random().Next();
			string fileName = Path.Combine(Path.GetTempPath(), "sd" + number + ".tmp");
			try {
				File.WriteAllLines(fileName, fileList);
				List<IntPtr> alternatives = new List<IntPtr>();
				foreach (Process p in Process.GetProcessesByName("SharpDevelop")) {
					if (p.Id == currentProcessId) continue;
					
					if (FileUtility.IsEqualFileName(currentFile, p.MainModule.FileName)) {
						IntPtr hWnd = p.MainWindowHandle;
						if (hWnd != IntPtr.Zero) {
							long result = NativeMethods.SendMessage(hWnd, CUSTOM_MESSAGE, new IntPtr(number), IntPtr.Zero).ToInt64();
							if (result == RESULT_FILES_HANDLED) {
								return true;
							} else if (result == RESULT_PROJECT_IS_OPEN) {
								alternatives.Add(hWnd);
							}
						}
					}
				}
				foreach (IntPtr hWnd in alternatives) {
					if (NativeMethods.SendMessage(hWnd, CUSTOM_MESSAGE, new IntPtr(number), new IntPtr(1)).ToInt64()== RESULT_FILES_HANDLED) {
						return true;
					}
				}
				return false;
			} finally {
				File.Delete(fileName);
			}
		}
		
		internal static bool PreFilterMessage(ref Message m)
		{
			if (m.Msg != CUSTOM_MESSAGE)
				return false;
			long fileNumber = m.WParam.ToInt64();
			long openEvenIfProjectIsOpened = m.LParam.ToInt64();
			LoggingService.Debug("Receiving custom message...");
			if (openEvenIfProjectIsOpened == 0 && ProjectService.OpenSolution != null) {
				m.Result = new IntPtr(RESULT_PROJECT_IS_OPEN);
			} else {
				m.Result = new IntPtr(RESULT_FILES_HANDLED);
				try {
					WorkbenchSingleton.SafeThreadAsyncCall(
						delegate { NativeMethods.SetForegroundWindow(WorkbenchSingleton.MainForm.Handle) ; }
					);
					string tempFileName = Path.Combine(Path.GetTempPath(), "sd" + fileNumber + ".tmp");
					foreach (string file in File.ReadAllLines(tempFileName)) {
						WorkbenchSingleton.SafeThreadAsyncCall(
							delegate(string openFileName) { FileService.OpenFile(openFileName); }
							, file
						);
					}
				} catch (Exception ex) {
					LoggingService.Warn(ex);
				}
			}
			return true;
		}
	}
}
