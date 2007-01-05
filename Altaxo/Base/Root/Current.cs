#region Copyright
/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2007 Dr. Dirk Lellinger
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;

using Altaxo.Serialization;
using Altaxo.Gui.Common;


namespace Altaxo
{
  /// <summary>
  /// This class provides access to application wide unique objects.
  /// </summary>
  public class Current
  {
    //private static object sm_theApplication;

    private static IWorkbench sm_theWorkbench;

    private static Altaxo.Main.IProjectService sm_theProjectService;

    private static Altaxo.Main.IPrintingService sm_thePrintingService;

    private static Altaxo.Main.Services.IOutputService sm_theOutputService;

    private static Altaxo.Main.Services.IDataDisplayService sm_theDataDisplayService;

    private static Altaxo.Main.Services.GUIFactoryService sm_theGUIFactoryService;

    private static Altaxo.Main.Services.IResourceService sm_theResourceService;

    private static Altaxo.Main.Services.IFitFunctionService sm_theFitFunctionService;

    private static bool sm_theApplicationIsClosing;

    /// <summary>
    /// Gets the main workbench.
    /// </summary>
    public static IWorkbench Workbench
    {
      get { return sm_theWorkbench; }
    }

    /// <summary>
    /// Returns the project service, which provides methods to add worksheet and graphs, or open and close the document.
    /// </summary>
    public static Altaxo.Main.IProjectService ProjectService
    {
      get { return sm_theProjectService; }
    }

    /// <summary>
    /// Returns the current altaxo project (the current document).
    /// </summary>
    public static Altaxo.AltaxoDocument Project
    {
      get { return sm_theProjectService.CurrentOpenProject; }
    }

    /// <summary>
    /// Returns the printing service, which provides methods for page setup and printing.
    /// </summary>
    public static Altaxo.Main.IPrintingService PrintingService
    {
      get { return sm_thePrintingService; }
    }

    /// <summary>
    /// Returns the console window, which can be used by your scripts for textual output.
    /// </summary>
    public static Altaxo.Main.Services.IOutputService Console
    {
      get { return sm_theOutputService; }
    }

    /// <summary>
    /// Returns the console window, which can be used by your scripts for textual output.
    /// </summary>
    public static Altaxo.Main.Services.GUIFactoryService Gui
    {
      get { return sm_theGUIFactoryService; }
    }

    /// <summary>
    /// Returns the resource service, which is used to obtain resource strings.
    /// </summary>
    public static Altaxo.Main.Services.IResourceService ResourceService
    {
      get { return sm_theResourceService; }
    }

    /// <summary>
    /// Returns the fit function service, which is used to obtain the file based user defined fit functions.
    /// </summary>
    public static Altaxo.Main.Services.IFitFunctionService FitFunctionService
    {
      get { return sm_theFitFunctionService; }
    }

    /// <summary>
    /// Sets the current output service.
    /// </summary>
    /// <param name="service">The instance of the output service to use in this application.</param>
    public static void SetGUIFactoryService(Altaxo.Main.Services.GUIFactoryService service)
    {
      if(null==sm_theGUIFactoryService)
        sm_theGUIFactoryService = service; 
      else
        throw new ApplicationException("The  service can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Returns a flag which is true if the application is about to be closed.
    /// </summary>
    public static bool ApplicationIsClosing
    {
      get { return sm_theApplicationIsClosing; }
      set { sm_theApplicationIsClosing = value; }
    }

    /// <summary>
    /// Returns the main windows form.
    /// </summary>
    public static Form MainWindow
    {
      get 
      {
        if(Current.Workbench is Form)
          return (Form)Current.Workbench;
        else
          return (Form)Current.Workbench.ViewObject;        
      }
    }
  

    /// <summary>
    /// Sets the main workbench.
    /// </summary>
    /// <param name="workbench">The main workbench to use in this application.</param>
    public static void SetWorkbench(IWorkbench workbench)
    {
      if(null==sm_theWorkbench)
        sm_theWorkbench = workbench; 
      else
        throw new ApplicationException("The workbench can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Sets the main project service.
    /// </summary>
    /// <param name="projectservice">The project service instance to use in this application.</param>
    public static void SetProjectService(Altaxo.Main.IProjectService projectservice)
    {
      if(null==sm_theProjectService)
        sm_theProjectService = projectservice; 
      else
        throw new ApplicationException("The project service can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Sets the current printing service.
    /// </summary>
    /// <param name="printingservice">The instance of printing service to use in this application.</param>
    public static void SetPrintingService(Altaxo.Main.IPrintingService printingservice)
    {
      if(null==sm_thePrintingService)
        sm_thePrintingService = printingservice; 
      else
        throw new ApplicationException("The printing service can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Sets the resource service.
    /// </summary>
    /// <param name="resourceservice">The instance of resource service to use in this application.</param>
    public static void SetResourceService(Altaxo.Main.Services.IResourceService resourceservice)
    {
      if(null==sm_theResourceService)
        sm_theResourceService = resourceservice; 
      else
        throw new ApplicationException("The resource service can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Sets the fit function service.
    /// </summary>
    /// <param name="fitFunctionService">The instance of fit function service to use in this application.</param>
    public static void SetFitFunctionService(Altaxo.Main.Services.IFitFunctionService fitFunctionService)
    {
      if (null == sm_theFitFunctionService)
        sm_theFitFunctionService = fitFunctionService;
      else
        throw new ApplicationException("The fit function service can not be re-set to another value, only initialized for the first time!");

    }

    /// <summary>
    /// Sets the current output service.
    /// </summary>
    /// <param name="outputservice">The instance of the output service to use in this application.</param>
    public static void SetOutputService(Altaxo.Main.Services.IOutputService outputservice)
    {
      if(null==sm_theOutputService)
        sm_theOutputService = outputservice; 
      else
        throw new ApplicationException("The output service can not be re-set to another value, only initialized for the first time!");

    }


    /// <summary>
    /// Returns the data display window, which is used to show the data obtained from the data reader tool.
    /// </summary>
    public static Altaxo.Main.Services.IDataDisplayService DataDisplay
    {
      get { return sm_theDataDisplayService; }
    }


    /// <summary>
    /// Sets the current data display service.
    /// </summary>
    /// <param name="service">The instance of the data display service to use in this application.</param>
    public static void SetDataDisplayService(Altaxo.Main.Services.IDataDisplayService service)
    {
      if(null==sm_theDataDisplayService)
        sm_theDataDisplayService = service; 
      else
        throw new ApplicationException("The data display service can not be re-set to another value, only initialized for the first time!");

    }

#if FormerGuiState
    /// <summary>
    /// The main entry point for the application. This function has to be called to
    /// run the application.
    /// </summary>
    [STAThread]
    public static void Main() 
    {
      if(null==sm_theProjectService)
      {
        sm_theProjectService  = new Altaxo.Main.ProjectService();

        sm_thePrintingService = new Altaxo.Main.PrintingService();

        // we construct the main document
        sm_theProjectService.CurrentOpenProject = new AltaxoDocument();

        MainController ctrl = new MainController();

        sm_theWorkbench = new AltaxoWorkbench(new MainView());
        
        ctrl.SetMenuToMainWindow();

        // InitializeMainController(ctrl);

      }
      try
      {
        System.Windows.Forms.Application.Run(Current.MainWindow);
      }
      catch(Exception e)
      {
        System.Windows.Forms.MessageBox.Show(e.ToString());
      }
    }
#endif
  }
}
