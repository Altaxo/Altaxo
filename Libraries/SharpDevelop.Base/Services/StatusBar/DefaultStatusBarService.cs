// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DefaultStatusBarService : AbstractService, IStatusBarService
	{
		SdStatusBar statusBar = null;
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		
		public DefaultStatusBarService()
		{
			statusBar = new SdStatusBar(this);
		}
		
		public void Dispose()
		{
			if (statusBar != null) {
				statusBar.Dispose();
				statusBar = null;
			}
		}
		
		public bool Visible {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar.Visible;
			}
			set {
				System.Diagnostics.Debug.Assert(statusBar != null);
				statusBar.Visible = value;
			}
		}
		
		public Control Control {
			get {
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public IProgressMonitor ProgressMonitor {
			get { 
				System.Diagnostics.Debug.Assert(statusBar != null);
				return statusBar;
			}
		}
		
		public bool CancelEnabled {
			get {
				return statusBar != null && statusBar.CancelEnabled;
			}
			set {
				System.Diagnostics.Debug.Assert(statusBar != null);
				statusBar.CancelEnabled = value;
			}
		}
		
		public void SetCaretPosition(int x, int y, int charOffset)
		{
			statusBar.CursorStatusBarPanel.Text = stringParserService.Parse("${res:StatusBarService.CursorStatusBarPanelText}", 
			                                                                new string[,] { {"Line", String.Format("{0,-10}", y + 1)}, 
			                                                                                {"Column", String.Format("{0,-5}", x + 1)}, 
			                                                                                {"Character", String.Format("{0,-5}", charOffset + 1)}});
		}
		
		public void SetInsertMode(bool insertMode)
		{
			statusBar.ModeStatusBarPanel.Text = insertMode ? stringParserService.Parse("${res:StatusBarService.CaretModes.Insert}") : stringParserService.Parse("${res:StatusBarService.CaretModes.Overwrite}");
		}
		
		public void ShowErrorMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.ShowErrorMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage(string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			lastMessage = message;
			statusBar.SetMessage(stringParserService.Parse(message));
		}
		
		public void SetMessage(Image image, string message)
		{
			System.Diagnostics.Debug.Assert(statusBar != null);
			statusBar.SetMessage(image, stringParserService.Parse(message));
		}
		
		bool   wasError    = false;
		string lastMessage = "";
		public void RedrawStatusbar()
		{
			if (wasError) {
				ShowErrorMessage(lastMessage);
			} else {
				SetMessage(lastMessage);
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			Visible = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.StatusBarVisible", true);
		}
		
		public void Update()
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
