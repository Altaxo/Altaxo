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
//	public class LocalVarPad : AbstractPadContent
//	{
//		ListView  localVarList;
//		
//		ColumnHeader name = new ColumnHeader();
//		ColumnHeader val  = new ColumnHeader();
//		ColumnHeader type = new ColumnHeader();
//		
//		public override Control Control {
//			get {
//				return localVarList;
//			}
//		}
//		
//		public LocalVarPad() : base("${res:MainWindow.Windows.Debug.Local}", null)
//		{
//			InitializeComponents();
//			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
//			debuggerService.DebugStarted += new EventHandler(DebuggerServiceDebugStarted);
//			debuggerService.Exception    += new ExceptionEventHandler(DebuggerServiceException);
//			debuggerService.Breaked      += new ThreadEventHandler(DebuggerServiceBreaked);
//			debuggerService.StepComplete += new StepEventHandler(DebuggerStepComplete);
//		}
//		void DebuggerStepComplete(object sender, StepEventArgs e)
//		{
//			Console.WriteLine("GOT STEP COMPLETE!!!");
//			localVarList.Items.Clear();
//			foreach (DebugValue val in e.Thread.LocalVariables) {
//				localVarList.Items.Add(new ListViewItem(new string[] {
//					"<unknown>",
//					val.Value,
//					val.TypeName
//				}));
//			}
//		}
//		void DebuggerServiceBreaked(object sender, ThreadEventArgs e)
//		{
//			Console.WriteLine("GOT BREAK!!!");
//			localVarList.Items.Clear();
//			foreach (DebugValue val in e.Thread.LocalVariables) {
//				localVarList.Items.Add(new ListViewItem(new string[] {
//					"<unknown>",
//					val.Value,
//					val.TypeName
//				}));
//			}
//		}
//		void DebuggerServiceException(object sender, ExceptionEventArgs e)
//		{
//			Console.WriteLine("GOT EXCEPTION!!!");
//			localVarList.Items.Clear();
//			foreach (DebugValue val in e.Thread.LocalVariables) {
//				localVarList.Items.Add(new ListViewItem(new string[] {
//					"<unknown>",
//					val.Value,
//					val.TypeName
//				}));
//			}
//		}
//		
//		void DebuggerServiceDebugStarted(object sender, EventArgs e)
//		{
//			localVarList.Items.Clear();
//		}
//		
//		void InitializeComponents()
//		{
//			localVarList = new ListView();
//			localVarList.FullRowSelect = true;
//			localVarList.AutoArrange = true;
//			localVarList.Alignment   = ListViewAlignment.Left;
//			localVarList.View = View.Details;
//			localVarList.Dock = DockStyle.Fill;
//			localVarList.GridLines  = false;
//			localVarList.Activation = ItemActivation.OneClick;
//			localVarList.Columns.AddRange(new ColumnHeader[] {name, val, type} );
//			name.Width = 250;
//			val.Width = 300;
//			type.Width = 250;
//			RedrawContent();
//		}
//		
//		public override void RedrawContent()
//		{
//			name.Text = "Name";
//			val.Text  = "Value";
//			type.Text = "Type";
//		}
//	}
//}
