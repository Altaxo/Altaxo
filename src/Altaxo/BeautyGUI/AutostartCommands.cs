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
using System.Runtime.Remoting;
using System.Security.Policy;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Gui.ErrorHandlers;

#if OriginalCode
using SharpDevelop.Internal.Parser;
#endif

using ICSharpCode.SharpDevelop.Commands;

namespace Altaxo.Main.Commands // ICSharpCode.SharpDevelop.Commands
{
	public class InitializeWorkbenchCommand : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		public override void Run()
		{
			Altaxo.Current.SetProjectService( new Altaxo.Main.ProjectService() );

			Altaxo.Current.SetPrintingService( new Altaxo.Main.PrintingService() );

			// we construct the main document
			Altaxo.Current.ProjectService.CurrentOpenProject = new AltaxoDocument();

			Altaxo.MainController ctrl = new Altaxo.MainController();
			// HACK the new WorkbenchWindow object is by this time completely ignored by the 
			// workbench constructor
			BeautyWorkbench w = new ICSharpCode.SharpDevelop.Gui.BeautyWorkbench(new ICSharpCode.SharpDevelop.Gui.BeautyWorkbenchWindow(), new Altaxo.AltaxoDocument());
			Altaxo.Current.SetWorkbench ( w );
			// Altaxo.Current.InitializeMainController(ctrl);
			WorkbenchSingleton.Workbench = w;
			
			ctrl.SetMenuToMainWindow();

			w.InitializeWorkspace();
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			w.SetMemento((IXmlConvertable)propertyService.GetProperty(workbenchMemento, new WorkbenchMemento()));
			w.UpdateViews(null, null);
			//WorkbenchSingleton.CreateWorkspace();
			//SetWbLayout();
			w.WorkbenchLayout = new MdiWorkbenchLayout();

			w.RedrawAllComponents();
			
		}

	
	}


	public class InitializeWorkbench1Command : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		public override void Run()
		{

			Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
			Altaxo.Current.SetProjectService( projectService );

			Altaxo.Main.IPrintingService printingService = (Altaxo.Main.IPrintingService)ServiceManager.Services.GetService(typeof(Altaxo.Main.IPrintingService));
			Altaxo.Current.SetPrintingService( printingService );



			//Altaxo.MainController ctrl = new Altaxo.MainController(new Altaxo.AltaxoDocument());

			Workbench1 w = new Workbench1();
			Altaxo.Current.SetWorkbench ( w );
			//Altaxo.Current.InitializeMainController(ctrl);
			WorkbenchSingleton.Workbench = w;
			
			w.InitializeWorkspace();

			projectService.ProjectChanged += new ProjectEventHandler(w.EhProjectChanged);
			
			// we construct the main document (for now)
			Altaxo.Current.ProjectService.CurrentOpenProject = new AltaxoDocument();

	
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			w.SetMemento((IXmlConvertable)propertyService.GetProperty(workbenchMemento, new WorkbenchMemento()));
			w.UpdateViews(null, null);
			
#if OriginalSharpDevelopCode
			WorkbenchSingleton.CreateWorkspace();
#else
			w.WorkbenchLayout = new SdiWorkbenchLayout();
			w.RedrawAllComponents();
#endif	
		}
	}


	public class StartWorkbench1Command : AbstractCommand
	{
		const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
		
		EventHandler idleEventHandler;
		bool isCalled = false;
		
		/// <remarks>
		/// The worst workaround in the whole project
		/// </remarks>
		void ShowTipOfTheDay(object sender, EventArgs e)
		{
			if (isCalled) 
			{
				Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
			// show tip of the day
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true)) 
			{
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}
		}
		
		public override void Run()
		{
#if OriginalSharpDevelopCode
			ReflectionClass reflectionClass = new ReflectionClass(typeof(object), null);
#endif			
			// register string tag provider (TODO: move to add-in tree :)
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			stringParserService.RegisterStringTagProvider(new ICSharpCode.SharpDevelop.Commands.SharpDevelopStringTagProvider());
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			Form f = (Form)WorkbenchSingleton.Workbench;
			f.Show();
			
			idleEventHandler = new EventHandler(ShowTipOfTheDay);
			Application.Idle += idleEventHandler;
			
#if OriginalSharpDevelopCode

			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			
			// load previous combine
			if ((bool)propertyService.GetProperty("SharpDevelop.LoadPrevProjectOnStartup", false)) 
			{
				object recentOpenObj = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.MainWindow.RecentOpen");
				if (recentOpenObj is ICSharpCode.SharpDevelop.Services.RecentOpen) 
				{
					ICSharpCode.SharpDevelop.Services.RecentOpen recOpen = (ICSharpCode.SharpDevelop.Services.RecentOpen)recentOpenObj;
					if (recOpen.RecentProject.Count > 0) 
					{ 
						projectService.OpenCombine(recOpen.RecentProject[0].ToString());
					}
				}
			}
			
#endif
			
			foreach (string file in SplashScreenForm.GetRequestedFileList()) 
			{
				switch (System.IO.Path.GetExtension(file).ToUpper()) 
				{
#if OriginalSharpDevelopCode
					case ".PRJX":
						try 
						{
							projectService.OpenCombine(file);
						} 
						catch (Exception e) 
						{
							CombineLoadError.HandleError(e, file);
						}
						
						break;
#endif
					default:
						try 
						{
							IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
							fileService.OpenFile(file);
						} 
						catch (Exception e) 
						{
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
			if (isCalled) 
			{
				Application.Idle -= idleEventHandler;
				return;
			}
			isCalled = true;
			// show tip of the day
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialog.TipOfTheDayView.ShowTipsAtStartup", true)) 
			{
				ViewTipOfTheDay dview = new ViewTipOfTheDay();
				dview.Run();
			}
		}
		
		public override void Run()
		{
#if OriginalCode
			ReflectionClass reflectionClass = new ReflectionClass(typeof(object), null);
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
			Form f = (Form)WorkbenchSingleton.Workbench;
#else
			Form f = (Form)((IExtendedWorkbench)WorkbenchSingleton.Workbench).ViewObject;
#endif
			
			f.Show();
			
			idleEventHandler = new EventHandler(ShowTipOfTheDay);
			Application.Idle += idleEventHandler;
			
#if OriginalCode
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
#endif
			
			f.Focus(); // windows.forms focus workaround	
			
			// finally run the workbench window ...
			Application.Run(f);
			
#if OriginalCode
			// save the workbench memento in the ide properties
			propertyService.SetProperty(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
#endif

		}
	}
}
