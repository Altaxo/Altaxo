//// <file>
////     <copyright see="prj:///doc/copyright.txt"/>
////     <license see="prj:///doc/license.txt"/>
////     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
////     <version value="$version"/>
//// </file>
//
//using System;
//using System.Windows.Forms;
//using System.Drawing;
//using System.CodeDom.Compiler;
//using System.Collections;
//using System.IO;
//using System.Diagnostics;
//using ICSharpCode.Core.Services;
//using ICSharpCode.SharpDevelop.Services;
//
//using ICSharpCode.Core.Properties;
//using ICSharpCode.Debugger;
//
//namespace ICSharpCode.SharpDevelop.Gui.Pads
//{
//	public class RunningThreadsPad : AbstractPadContent
//	{
//		ListView  runningThreadsList;
//		
//		ColumnHeader id          = new ColumnHeader();
//		ColumnHeader name        = new ColumnHeader();
//		ColumnHeader location    = new ColumnHeader();
//		ColumnHeader priority    = new ColumnHeader();
//		ColumnHeader breaked     = new ColumnHeader();
//		
//		public override Control Control {
//			get {
//				return runningThreadsList;
//			}
//		}
//		
//		public RunningThreadsPad() : base("${res:MainWindow.Windows.Debug.Threads}", null)
//		{
//			InitializeComponents();
//			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
//			debuggerService.DebugStarted += new EventHandler(DebuggerServiceDebugStarted);
//			debuggerService.DebugStopped += new EventHandler(DebuggerServiceDebugStarted);
//			debuggerService.ThreadCreated += new ThreadEventHandler(DebuggerServiceThreadCreated);
//			debuggerService.ThreadExited  += new ThreadEventHandler(DebuggerServiceThreadExited);
//			
//		}
//		
//		void DebuggerServiceDebugStarted(object sender, EventArgs e)
//		{
//			runningThreadsList.Items.Clear();
//		}
//		void DebuggerServiceThreadExited(object sender, ThreadEventArgs e)
//		{
//			foreach (ListViewItem item in runningThreadsList.Items) {
//				if (item.Text == e.Thread.ThreadID.ToString()) {
//					runningThreadsList.Items.Remove(item);
//					break;
//				}
//			}
//		}
//		void DebuggerServiceThreadCreated(object sender, ThreadEventArgs e)
//		{
//			runningThreadsList.Items.Add(new ListViewItem(new string[] {
//				e.Thread.ThreadID.ToString(),
//				e.Thread.Name,
//				e.Thread.Location,
//				e.Thread.Priority.ToString(),
//				""
//			}));
//		}
//		
//		void InitializeComponents()
//		{
//			runningThreadsList = new ListView();
//			runningThreadsList.FullRowSelect = true;
//			runningThreadsList.AutoArrange = true;
//			runningThreadsList.Alignment   = ListViewAlignment.Left;
//			runningThreadsList.View = View.Details;
//			runningThreadsList.Dock = DockStyle.Fill;
//			runningThreadsList.GridLines  = false;
//			runningThreadsList.Activation = ItemActivation.OneClick;
//			runningThreadsList.Columns.AddRange(new ColumnHeader[] {id, name, location, priority, breaked} );
//			id.Width = 100;
//			name.Width = 300;
//			location.Width = 250;
//			priority.Width = 50;
//			breaked.Width = 70;
//						
//			RedrawContent();
//		}
//		
//		public override void RedrawContent()
//		{
//			id.Text          = "ID";
//			name.Text        = "Name";
//			location.Text    = "Location";
//			priority.Text    = "Priority";
//			breaked.Text     = "Breaked";
//		}
//	}
//}
