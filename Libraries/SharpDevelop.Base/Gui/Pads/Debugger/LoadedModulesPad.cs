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
//using ICSharpCode.Debugger;
//using ICSharpCode.Core.Properties;
//
//namespace ICSharpCode.SharpDevelop.Gui.Pads
//{
//	public class LoadedModulesPad : AbstractPadContent
//	{
//		ListView  loadedModulesList;
//		
//		ColumnHeader name        = new ColumnHeader();
//		ColumnHeader address     = new ColumnHeader();
//		ColumnHeader path        = new ColumnHeader();
//		ColumnHeader order       = new ColumnHeader();
//		ColumnHeader version     = new ColumnHeader();
//		ColumnHeader program     = new ColumnHeader();
//		ColumnHeader timestamp   = new ColumnHeader();
//		ColumnHeader information = new ColumnHeader();
//			
//		
//		public override Control Control {
//			get {
//				return loadedModulesList;
//			}
//		}
//		
//		public LoadedModulesPad() : base("${res:MainWindow.Windows.Debug.Modules}", null)
//		{
//			InitializeComponents();
//			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
//			debuggerService.DebugStarted += new EventHandler(DebuggerServiceDebugStarted);
//			debuggerService.ModuleLoaded += new ModuleEventHandler(DebuggerServiceModuleLoaded);
//			debuggerService.ModuleUnloaded += new ModuleEventHandler(DebuggerServiceModuleLoaded);
//		}
//		
//		int orderNumber = 0;
//		void DebuggerServiceDebugStarted(object sender, EventArgs e)
//		{
//			loadedModulesList.Items.Clear();
//			orderNumber = 0;
//		}
//		
//		void DebuggerServiceModuleUnloaded(object sender, ModuleEventArgs e)
//		{
//			foreach (ListViewItem item in loadedModulesList.Items) {
//				if (item.Text == e.Module.Name) {
//					loadedModulesList.Items.Remove(item);
//					break;
//				}
//			}
//		}
//		
//		void DebuggerServiceModuleLoaded(object sender, ModuleEventArgs e)
//		{
//			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
//			loadedModulesList.Items.Add(new ListViewItem(new string[] {
//				e.Module.Name,
//				String.Format("{0,-8:X}", e.Module.BaseAddress),
//				e.Module.Path,
//				(++orderNumber).ToString(),
//				"",
//				"",
//				"",
//				stringParserService.Parse(e.Module.HasSymbolsLoaded ? "${res:MainWindow.Windows.Debug.HasSymbols}" : "${res:MainWindow.Windows.Debug.HasNoSymbols}")
//			}));
//		}
//		
//		void InitializeComponents()
//		{
//			loadedModulesList = new ListView();
//			loadedModulesList.FullRowSelect = true;
//			loadedModulesList.AutoArrange = true;
//			loadedModulesList.Alignment   = ListViewAlignment.Left;
//			loadedModulesList.View = View.Details;
//			loadedModulesList.Dock = DockStyle.Fill;
//			loadedModulesList.GridLines  = false;
//			loadedModulesList.Activation = ItemActivation.OneClick;
//			loadedModulesList.Columns.AddRange(new ColumnHeader[] {name, address, path, order, version, program, timestamp, information} );
//			name.Width = 250;
//			address.Width = 100;
//			path.Width = 250;
//			order.Width = 50;
//			version.Width = 50;
//			program.Width = 90;
//			timestamp.Width = 80;
//			information.Width = 70;
//			RedrawContent();
//		}
//		
//		public override void RedrawContent()
//		{
//			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
//			name.Text         = stringParserService.Parse("${res:MainWindow.Windows.Debug.NameColumn}");
//			address.Text      = stringParserService.Parse("${res:MainWindow.Windows.Debug.AddressColumn}");
//			path.Text         = stringParserService.Parse("${res:MainWindow.Windows.Debug.PathColumn}");
//			order.Text        = stringParserService.Parse("${res:MainWindow.Windows.Debug.OrderColumn}");
//			version.Text      = stringParserService.Parse("${res:MainWindow.Windows.Debug.VersionColumn}");
//			program.Text      = stringParserService.Parse("${res:MainWindow.Windows.Debug.ProgramColumn}");
//			timestamp.Text    = stringParserService.Parse("${res:MainWindow.Windows.Debug.TimestampColumn}");
//			information.Text  = stringParserService.Parse("${res:MainWindow.Windows.Debug.InformationColumn}");
//		}
//	}
//}
