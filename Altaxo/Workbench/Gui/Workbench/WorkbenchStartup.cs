using Altaxo.Gui.Startup;
using Altaxo.Main.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Altaxo.Gui.Workbench
{
  /// <summary>
  /// Helper class to start-up the workbench window.
  /// </summary>
  public class WorkbenchStartup
  {
    private App _application;

    private AltaxoWorkbench _workbench;

    public void InitializeWorkbench()
    {
      _application = new App();

      var synchronizationContext = SynchronizationContext.Current;
      if (null == synchronizationContext)
      {
        using (var ctrl = new System.Windows.Forms.Control()) // trick: create a windows forms control to make sure we have a synchronization context
        {
          synchronizationContext = SynchronizationContext.Current;
        }
      }

      Current.AddService<IDispatcherMessageLoop, IDispatcherMessageLoopWpf>(new DispatcherMessageLoop(_application.Dispatcher, synchronizationContext));

      _workbench = new AltaxoWorkbench(); // workbench view model

      Altaxo.Current.AddService<IWorkbench, IWorkbenchEx, AltaxoWorkbench>(_workbench);

      _workbench.RestoreWorkbenchStateFromPropertyService();
      _workbench.RestoreWorkbenchDockingLayoutFromPropertyService();
      _workbench.RestoreWorkbenchDockingThemeFromPropertyService();

      //UILanguageService.ValidateLanguage();

      //TaskService.Initialize();
      //Project.CustomToolsService.Initialize();
    }

    public void Run(StartupSettings startupArguments)
    {
      var mainForm = new MainWorkbenchWindow();
      _workbench.Initialize(mainForm);
      mainForm.DataContext = Current.Workbench;
      var propertyService = Current.PropertyService; // save as local variable because if the app is closed, the services will be shutdown by the shutdown service
      Current.IProjectService.ExecuteActionsImmediatelyBeforeRunningApplication(startupArguments.StartupArgs, startupArguments.ParameterList, startupArguments.RequestedFileList);
      _application.Run(mainForm);
    }
  }
}
