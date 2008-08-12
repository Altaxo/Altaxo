﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2059 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop
{
	public static class StatusBarService
	{
		static SdStatusBar statusBar = null;
		
		internal static void Initialize()
		{
			statusBar = new SdStatusBar();
		}
		
		public static bool Visible {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar.Visible;
			}
			set {
				System.Diagnostics.Debug.Assert(statusBar != null);
				statusBar.Visible = value;
			}
		}
		
		public static Control Control {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public static IProgressMonitor ProgressMonitor {
			get { 
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public static void SetCaretPosition(int x, int y, int charOffset)
		{
			statusBar.CursorStatusBarPanel.Text = StringParser.Parse("${res:StatusBarService.CursorStatusBarPanelText}", 
			                                                                new string[,] { {"Line", String.Format("{0,-10}", y + 1)}, 
			                                                                                {"Column", String.Format("{0,-5}", x + 1)}, 
			                                                                                {"Character", String.Format("{0,-5}", charOffset + 1)}});
		}
		
		public static void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Text = insertMode ? StringParser.Parse("${res:StatusBarService.CaretModes.Insert}") : StringParser.Parse("${res:StatusBarService.CaretModes.Overwrite}");
		}
		
		public static void ShowErrorMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(StringParser.Parse(message));
		}
		
		public static void SetMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(StringParser.Parse(message));
		}
		
		public static void SetMessage(Image image, string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.SetMessage(image, StringParser.Parse(message));
		}
		
		public static void SetMessage(string message, bool highlighted)
		{
			statusBar.SetMessage(message, highlighted);
		}
		
		static bool   wasError    = false;
		static string lastMessage = "";
		
		public static void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
			
			Visible = PropertyService.Get("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
		}
		
		public static void Update()
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
	/*		statusBar.Panels.Clear();
			statusBar.Controls.Clear();
			
			foreach (StatusBarContributionItem item in Items) {
				if (item.Control != null) {
					statusBar.Controls.Add(item.Control);
				} else if (item.Panel != null) {
					statusBar.Panels.Add(item.Panel);
				} else {
					throw new ApplicationException("StatusBarContributionItem " + item.ItemID + " has no Control or Panel defined.");
				}
			}*/
		}
	}
}
