// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;
using System.Drawing;
using System.CodeDom.Compiler;
using System.Collections;
using System.IO;
using System.Diagnostics;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;

using ICSharpCode.Core.Properties;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public class BreakPointsPad : AbstractPadContent
	{
		ListView  breakpointsList;
		
		ColumnHeader name     = new ColumnHeader();
		ColumnHeader path     = new ColumnHeader();
		
		public override Control Control {
			get {
				return breakpointsList;
			}
		}
		
		public BreakPointsPad() : base("${res:MainWindow.Windows.Debug.Breakpoints}", null)
		{
			InitializeComponents();
			DebuggerService debuggerService    = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.BreakPointChanged += new EventHandler(DebuggerBreakPointChanged);
		}
		
		void DebuggerBreakPointChanged(object sender, EventArgs e)
		{
			DebuggerService debuggerService    = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			lock (debuggerService.Breakpoints) {
				ListViewItem[] items = new ListViewItem[debuggerService.Breakpoints.Count];
				for (int i = 0; i < items.Length; ++i) {
					Breakpoint breakpoint = debuggerService.Breakpoints[i] as Breakpoint;
					items[i] = new ListViewItem(new string[] {
						Path.GetFileName(breakpoint.FileName) + ", Line = " + (breakpoint.LineNumber + 1),
						Path.GetDirectoryName(breakpoint.FileName)
					});
					items[i].Checked = breakpoint.IsEnabled;
					items[i].Tag     = breakpoint;
				}
				breakpointsList.ItemCheck -= new ItemCheckEventHandler(BreakpointsListItemCheck);
				breakpointsList.BeginUpdate();
				breakpointsList.Items.Clear();
				breakpointsList.Items.AddRange(items);
				breakpointsList.EndUpdate();
				breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
			}
		}
		void InitializeComponents()
		{
			breakpointsList = new ListView();
			breakpointsList.FullRowSelect = true;
			breakpointsList.AutoArrange = true;
			breakpointsList.Alignment   = ListViewAlignment.Left;
			breakpointsList.View = View.Details;
			breakpointsList.Dock = DockStyle.Fill;
			breakpointsList.GridLines  = false;
			breakpointsList.Activation = ItemActivation.OneClick;
			breakpointsList.CheckBoxes = true;
			breakpointsList.Columns.AddRange(new ColumnHeader[] {name, path} );
			breakpointsList.ItemCheck += new ItemCheckEventHandler(BreakpointsListItemCheck);
			
			name.Width = 300;
			path.Width = 400;
			
			RedrawContent();
		}
		
		void BreakpointsListItemCheck(object sender, ItemCheckEventArgs e)
		{
			Breakpoint breakpoint = breakpointsList.Items[e.Index].Tag as Breakpoint;
			if (breakpoint != null) {
				breakpoint.IsEnabled = e.NewValue == CheckState.Checked;
			}
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.RedrawContent();
			}
		}
		
		public override void RedrawContent()
		{
			name.Text        = "Name";
			path.Text        = "Path";
		}
	}
}
