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
using ICSharpCode.SharpDevelop.Gui.ErrorHandlers;

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
			w.UpdateViews(null, null);
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
				if (wizard.ShowDialog() == DialogResult.OK) {
					propertyService.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
					                            customizer.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
					// restart  & exit 
					ServiceManager.Services.UnloadAllServices();
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
			Thread preloadThread = new Thread(new ThreadStart(PreloadThreadStart));
			preloadThread.IsBackground = true;
			preloadThread.Priority = ThreadPriority.Lowest;
			preloadThread.Start();
		}
		
		public void PreloadThreadStart()
		{
			Console.WriteLine("#Assembly: starting preloading thread");
			SA.SharpAssembly.Load("System");
			Console.WriteLine("#Assembly: preloaded system");
			SA.SharpAssembly.Load("System.Xml");
			Console.WriteLine("#Assembly: preloaded system.xml");
			SA.SharpAssembly.Load("System.Windows.Forms");
			Console.WriteLine("#Assembly: preloaded system.windows.forms");
			SA.SharpAssembly.Load("System.Drawing");
			Console.WriteLine("#Assembly: preloaded system.drawing");
			SA.SharpAssembly.Load("System.Data");
			Console.WriteLine("#Assembly: preloaded system.data");
			SA.SharpAssembly.Load("System.Design");			
			Console.WriteLine("#Assembly: preloaded system.design");
			SA.SharpAssembly.Load("System.Web");			
			Console.WriteLine("#Assembly: preloaded system.web");
		}
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
			
			// load previous combine
			if ((bool)propertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) {
				object recentOpenObj = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) {
					ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject.Count > 0) { 
						projectService.OpenCombine(recOpen.RecentProject[0].ToString());
					}
				}
			}
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
					case ".CMBX":
					case ".PRJX":
						try {
							projectService.OpenCombine(file);
						} catch (Exception e) {
							CombineLoadError.HandleError(e, file);
						}
						
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
			
			f.Focus(); // windows.forms focus workaround	
			
			// finally run the workbench window ...
			Application.Run(f);
			
			// save the workbench memento in the ide properties
			propertyService.SetProperty(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
		}
	}
}
