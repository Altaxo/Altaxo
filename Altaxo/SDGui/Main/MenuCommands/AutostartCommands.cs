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
      
#if !ModifiedForAltaxo
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
        if (wizard.ShowDialog() == DialogResult.OK) 
        {
          propertyService.SetProperty("SharpDevelop.CodeCompletion.DataDirectory",
            customizer.GetProperty("SharpDevelop.CodeCompletion.DataDirectory", String.Empty));
          // restart  & exit 
          ServiceManager.Services.UnloadAllServices();
          System.Diagnostics.Process.Start(Path.Combine(Application.StartupPath, "AltaxoStartup.exe"));
          System.Environment.Exit(0);
        }
      }
    }
  }
  
}
