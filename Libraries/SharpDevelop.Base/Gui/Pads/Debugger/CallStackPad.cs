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
//
//namespace ICSharpCode.SharpDevelop.Gui.Pads
//{
//	public class CallStackPad : AbstractPadContent
//	{
//		ListView  callStackList;
//		
//		ColumnHeader name     = new ColumnHeader();
//		ColumnHeader language = new ColumnHeader();
//		
//		public override Control Control {
//			get {
//				return callStackList;
//			}
//		}
//		
//		public CallStackPad() : base("${res:MainWindow.Windows.Debug.CallStack}", null)
//		{
//			InitializeComponents();
////			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
////			debuggerService.DebugStarted += new EventHandler(DebuggerServiceDebugStarted);
////			debuggerService.Break        += new BreakEventHandler(DebuggerServiceBreak);
//			
//		}
//		
////		void DebuggerServiceDebugStarted(object sender, EventArgs e)
////		{
////			callStackList.Items.Clear();
////		}
////		
////		void DebuggerServiceBreak(object sender, BreakEventArgs e)
////		{
////			callStackList.BeginUpdate();
////			callStackList.Items.Clear();
////			foreach (MethodCall methodCall in e.CallStack) {
////				callStackList.Items.Add(new ListViewItem(new string[] {
////					methodCall.Name,
////					methodCall.Language
////				}));
////			}
////			callStackList.EndUpdate();
////		}
//		
//				
//		void InitializeComponents()
//		{
//			callStackList = new ListView();
//			callStackList.FullRowSelect = true;
//			callStackList.AutoArrange = true;
//			callStackList.Alignment   = ListViewAlignment.Left;
//			callStackList.View = View.Details;
//			callStackList.Dock = DockStyle.Fill;
//			callStackList.GridLines  = false;
//			callStackList.Activation = ItemActivation.OneClick;
//			callStackList.Columns.AddRange(new ColumnHeader[] {name, language} );
//			name.Width = 300;
//			language.Width = 400;
//			
//			RedrawContent();
//		}
//		
//		public override void RedrawContent()
//		{
//			name.Text        = "Name";
//			language.Text    = "Language";
//		}
//	}
//}
