#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2004 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////
#endregion

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
//using ICSharpCode.SharpDevelop.Gui.ErrorHandlers;

//#if OriginalCode
using SharpDevelop.Internal.Parser;
//#endif

using ICSharpCode.SharpDevelop.Commands;

namespace Altaxo.Main.Commands // ICSharpCode.SharpDevelop.Commands
{
  

  public class InitializeWorkbench1Command : AbstractCommand
  {
    const string workbenchMemento = "SharpDevelop.Workbench.WorkbenchMemento";
    
    public override void Run()
    {

      ICSharpCode.Core.Services.IResourceService resourceService = (ICSharpCode.Core.Services.IResourceService)ServiceManager.Services.GetService(typeof(ICSharpCode.Core.Services.IResourceService));
      Altaxo.Current.SetResourceService(new ResourceServiceWrapper(resourceService));      
      
      Altaxo.Main.ProjectService projectService = (Altaxo.Main.ProjectService)ServiceManager.Services.GetService(typeof(Altaxo.Main.ProjectService));
      Altaxo.Current.SetProjectService( projectService );

      Altaxo.Main.IPrintingService printingService = (Altaxo.Main.IPrintingService)ServiceManager.Services.GetService(typeof(Altaxo.Main.IPrintingService));
      Altaxo.Current.SetPrintingService( printingService );

      Altaxo.Current.SetGUIFactoryService( new Altaxo.Main.Services.GUIFactoryService() );

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
			w.UpdatePadContents(null, null);
      
#if !ModifiedForAltaxo
      WorkbenchSingleton.CreateWorkspace();
#else
      w.WorkbenchLayout = new SdiWorkbenchLayout();
      w.RedrawAllComponents();
#endif  
    }

    private class ResourceServiceWrapper : Altaxo.Main.Services.IResourceService
    {
      ICSharpCode.Core.Services.IResourceService _service;
      public ResourceServiceWrapper(ICSharpCode.Core.Services.IResourceService service)
      {
        _service = service;
      }

      public string GetString(string name)
      {
        return _service.GetString(name);
      }
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
			
      if (!File.Exists(codeCompletionProxyFile)) 
      {
        RunWizard();
        DefaultParserService parserService = (DefaultParserService)ServiceManager.Services.GetService(typeof(IParserService));
        parserService.LoadProxyDataFile();
      }
    }
		
    void RunWizard()
    {
      IProperties customizer = new DefaultProperties();
			
      if (SplashScreenForm.SplashScreen.Visible) 
      {
        SplashScreenForm.SplashScreen.Close();
      }
      PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			
      customizer.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
        propertyService.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
			
      using (WizardDialog wizard = new WizardDialog("Initialize Code Completion Database", customizer, "/SharpDevelop/CompletionDatabaseWizard")) 
      {
        wizard.ControlBox = false;
        wizard.ShowInTaskbar = true;
        if (wizard.ShowDialog() == DialogResult.OK && !customizer.GetProperty("SkipDb", false)) 
        {
          propertyService.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
            customizer.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
          // restart  & exit 
          ServiceManager.Services.UnloadAllServices();
          ((Form)WorkbenchSingleton.Workbench).Dispose();
          System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "AltaxoStartup.exe"));
          System.Environment.Exit(0);
        }
      }
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
    class FormKeyHandler : IMessageFilter
    {
      const int keyPressedMessage          = 0x100;
			
      void HideAllPads()
      {
        foreach (IPadContent pad in WorkbenchSingleton.Workbench.PadContentCollection) 
        {
          WorkbenchSingleton.Workbench.WorkbenchLayout.HidePad(pad);
        }
      }
      void SelectActiveWorkbenchWindow()
      {
        if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) 
        {
          if (!WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.ContainsFocus) 
          {
            if (Form.ActiveForm == (Form)WorkbenchSingleton.Workbench) 
            {
              WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.Control.Focus();
            }
          }
        }
      }
			
      public bool PreFilterMessage(ref Message m)
      {
        if (m.Msg != keyPressedMessage) 
        {
          return false;
        }
        Keys keyPressed = (Keys)m.WParam.ToInt32() | Control.ModifierKeys;
				
        if (keyPressed == Keys.Escape) 
        {
          SelectActiveWorkbenchWindow();
          return false;
        }
				
        if (keyPressed == (Keys.Escape | Keys.Shift)) 
        {
          HideAllPads();
          SelectActiveWorkbenchWindow();
          return true;
        }
        return false;
      }
    }


    public override void Run()
    {
      //#if OriginalSharpDevelopCode
      ReflectionClass reflectionClass = new ReflectionClass(typeof(object), null);
      //#endif      
      // register string tag provider (TODO: move to add-in tree :)
      StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
      stringParserService.RegisterStringTagProvider(new ICSharpCode.SharpDevelop.Commands.SharpDevelopStringTagProvider());
      
      PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
      Form f = (Form)WorkbenchSingleton.Workbench;
      f.Show();
      
      idleEventHandler = new EventHandler(ShowTipOfTheDay);
      Application.Idle += idleEventHandler;
      
#if !ModifiedForAltaxo

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
#else      
			
      bool didLoadCombineOrFile = false;
      foreach (string file in SplashScreenForm.GetRequestedFileList()) {
				didLoadCombineOrFile = true;
				switch (System.IO.Path.GetExtension(file).ToUpper()) {
          case ".AXOPRJ":
          case ".AXOPRZ":
            try
            {
              Current.ProjectService.OpenProject(file);
            }
            catch(Exception ex)
            {
              Current.Console.WriteLine("Unable to open project <<{0}>>, the following exception was thrown:",file);
              Current.Console.WriteLine(ex.ToString());
            }
            break;
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

#endif
      
      f.Focus(); // windows.forms focus workaround	
			
      // finally run the workbench window ...
      Application.AddMessageFilter(new FormKeyHandler());
      Application.Run(f);
			
      // save the workbench memento in the ide properties
      try 
      {
        propertyService.SetProperty(workbenchMemento, WorkbenchSingleton.Workbench.CreateMemento());
      } 
      catch (Exception e) 
      {
        Console.WriteLine("Exception while saving workbench state: " + e.ToString());
      }
    }
  }



  
  
}
