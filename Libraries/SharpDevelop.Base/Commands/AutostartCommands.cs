// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.CodeDom.Compiler;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;

using SA = ICSharpCode.SharpAssembly.Assembly;

using SharpDevelop.Internal.Parser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class InitializeWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		public override void Run()
		{
			DefaultWorkbench w = new DefaultWorkbench();
			WorkbenchSingleton.Workbench = w;
			
			w.InitializeWorkspace();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			w.SetMemento((IXmlConvertable)propertyService.GetProperty(workbenchMemento, new WorkbenchMemento()));
			w.UpdatePadContents(null, null);
			WorkbenchSingleton.CreateWorkspace();
			
		}
	}
	
	public class StartCodeCompletionWizard : AbstractCommand
	{
		public override void Run()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string path = propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty).ToString();
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string codeCompletionTemp = fileUtilityService.GetDirectoryNameWithSeparator(path);
			string codeCompletionProxyFile = codeCompletionTemp + "CodeCompletionProxyDataV02.bin";
			
			if (!File.Exists(codeCompletionProxyFile)) {
				RunWizard();
				DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(IParserService));
				parserService.LoadProxyDataFile();
			}
		}
		
		void RunWizard()
		{
			IProperties customizer = new DefaultProperties();
			
			if (SplashScreenForm.SplashScreen.Visible) {
				SplashScreenForm.SplashScreen.Close();
			}
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			customizer.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
			                       propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
			
			using (WizardDialog wizard = new WizardDialog("Initialize Code Completion Database", customizer, "/SharpDevelop/CompletionDatabaseWizard")) {
				wizard.ControlBox = false;
				wizard.ShowInTaskbar = true;
				if (wizard.ShowDialog() == DialogResult.OK && !customizer.GetProperty("SkipDb", false)) {
					propertyService.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
					                            customizer.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
					// restart  & exit 
					ServiceManager.Services.UnloadAllServices();
					((Form)WorkbenchSingleton.Workbench).Dispose();
					System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "SharpDevelop.exe"));
					System.Environment.Exit(0);
				}
			}
		}
	}
	
	public class StartParserServiceThread : AbstractCommand
	{
		public override void Run()
		{
			DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(DefaultParserService));
			parserService.StartParserThread();
		}
	}
	
	public class StartSharpAssemblyPreloadThread : AbstractCommand
	{
		public override void Run()
		{
			// Sorry this takes WAY TOO MUCH memory :(
//			Thread preloadThread = new Thread(new ThreadStart(PreloadThreadStart));
//			preloadThread.IsBackground = true;
//			preloadThread.Priority = ThreadPriority.Lowest;
//			preloadThread.Start();
		}
		
		void PreloadThreadStart()
		{
			foreach (string assembly in assemblyList) {
				SA.SharpAssembly.Load(assembly);
				Console.WriteLine(" ...done");
			}
		}
		
		readonly static string[] assemblyList = {
			"Microsoft.VisualBasic",
			"Microsoft.JScript",
			"mscorlib",
			"System.Data",
			"System.Design",
			"System.DirectoryServices",
			"System.Drawing.Design",
			"System.Drawing",
			"System.EnterpriseServices",
			"System.Management",
			"System.Messaging",
			"System.Runtime.Remoting",
			"System.Runtime.Serialization.Formatters.Soap",

			"System.Security",
			"System.ServiceProcess",
			"System.Web.Services",
			"System.Web",
			"System.Windows.Forms",
			"System",
			"System.XML"
		};
	}
	
	public class StartWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		EventHandler idleEventHandler;
		bool isCalled = false;
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			if (isCalled) {
				Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
			// show tip of the day
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true)) {
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}
		}
		
		class FormKeyHandler : IMessageFilter
		{
			const int keyPressedMessage          = 0x100;
			
			void HideAllPads()
			{
				foreach (IPadContent pad in WorkbenchSingleton.Workbench.PadContentCollection) {
					WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(pad);
				}
			}
			void SelectActiveWorkbenchWindow()
			{
				if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
					if (!WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.ContainsFocus) {
						if (Form.ActiveForm == (Form)WorkbenchSingleton.Workbench) {
							WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.Focus();
						}
					}
				}
			}
			
			public bool PreFilterMessage(ref Message m)
			{
				if (m.Msg != keyPressedMessage) {
					return false;
				}
				Keys keyPressed = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				
				if (keyPressed == Keys.Escape) {
					SelectActiveWorkbenchWindow();
					return false;
				}
				
				if (keyPressed == (Keys.Escape | Keys.Shift)) {
					HideAllPads();
					SelectActiveWorkbenchWindow();
					return true;
				}
				return false;
			}
		}

		
		public override void Run()
		{
			ReflectionClass reflectionClass = new ReflectionClass(typeof(object), null);
			
			// register string tag provider (TODO: move to add-in tree :)
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			stringParserService.RegisterStringTagProvider(new ICSharpCode.SharpDevelop.Commands.SharpDevelopStringTagProvider());
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			
			idleEventHandler = new EventHandler(ShowTipOfTheDay);
			Application.Idle += idleEventHandler;
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			bool didLoadCombineOrFile = false;
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				didLoadCombineOrFile = true;
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
						fileUtilityService.ObservedLoad(new NamedFileOperationDelegate(projectService.OpenCombine), file);
						break;
					default:
						try {
							IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
							fileService.OpenFile(file);
						} catch (Exception e) {
							Console.WriteLine("unable to open file {0} exception was :\n{1}", file, e.ToString());
						}
						break;
				}
			}
			
			// load previous combine
			if (!didLoadCombineOrFile && (bool)propertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				object recentOpenObj = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) {
					ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject.Count > 0) { 
						projectService.OpenCombine(recOpen.RecentProject[0].ToString());
					}
				}
			}
			
			
			f.Focus(); // windows.forms focus workaround	
			
			// finally run the workbench window ...
			Application.AddMessageFilter(new FormKeyHandler());
			Application.Run(f);
			
			// save the workbench memento in the ide properties
			try {
				propertyService.SetProperty(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
			} catch (Exception e) {
				Console.WriteLine("Exception while saving workbench state: " + e.ToString());
			}
		}
	}
}
