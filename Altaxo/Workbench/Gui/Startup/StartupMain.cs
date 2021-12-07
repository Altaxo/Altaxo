// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Windows.Input;
using Altaxo.AddInItems;
using Altaxo.Gui.AddInItems;
using Altaxo.Gui.Workbench;
using Altaxo.Gui.Workbench.Commands;
using Altaxo.Main.Services;
using Altaxo.Main.Services.ExceptionHandling;

namespace Altaxo.Gui.Startup
{
  /// <summary>
  /// Main entry to start the application. Call <see cref="Main"/> to start the application.
  /// </summary>
  public static class StartupMain
  {
    private static readonly string[] _possibleStartupAssemblyNameEnds = { "startup", "startup32", "startup64" };

    private static bool AttachDebugger(params string[] startupArgs)
    {
      foreach (string arg in startupArgs ?? Enumerable.Empty<string>())
      {
        if (arg == "--attachDebugger")
          return true;
      }
      return false;
    }

    private static bool UseExceptionBox(params string[] startupArgs)
    {
#if DEBUG
      if (Debugger.IsAttached)
        return false;
#endif
      foreach (string arg in startupArgs ?? Enumerable.Empty<string>())
      {
        if (arg.Contains("noExceptionBox"))
          return false;
      }
      return true;
    }

    /// <summary>
    /// Finds the name of the application.
    /// If the name of the startup assembly ends with "Startup", "Startup64", or "Startup32", e.g. AltaxoStartup, the part of the name before "Startup" is used as the application name, i.e. "Altaxo"
    /// Otherwise, the name of the startup assembly is used directly as application name.
    /// </summary>
    /// <returns>Application name.</returns>
    private static string FindApplicationName()
    {
      var assName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException("Unable to get name of entry assembly");
      var assNameInvar = assName.ToLowerInvariant();
      string appName;
      foreach (var end in _possibleStartupAssemblyNameEnds)
      {
        if (assNameInvar.EndsWith(end) && (appName = assName.Substring(0, assName.Length - end.Length)).Length > 0)
        {
          return appName;
        }
      }
      return assName;
    }

    /// <summary>
    /// Starts the application.
    /// </summary>
    [STAThread()]
    public static void Main(string[] args)
    {
      // Make sure to use nothing except from this Assembly (event not derived from another assembly)
      // until the splash screen is shown
      // otherwise other assemblies will be loaded before the splash screen is visible

      if (AttachDebugger(args))
      {
        System.Diagnostics.Debugger.Launch();
        if (System.Diagnostics.Debugger.IsAttached)
        {
          System.Diagnostics.Debugger.Break();
        }
        else
        {
          Console.Write("Wait for a debugger to be attached...");
          for (int i = 0; i < 30; ++i)
          {
            Console.Write(".");
            System.Threading.Thread.Sleep(1000);
          }
          System.Diagnostics.Debugger.Break();
        }
      }

      var startupArguments = new StartupArguments(FindApplicationName(), args);

      // Do not use LoggingService here (see comment in Run(string[]))
      if (UseExceptionBox(args))
      {
        try
        {
          Run(startupArguments);
        }
        catch (Exception ex)
        {
          try
          {
            HandleMainException(startupArguments, ex);
          }
          catch (Exception loadError)
          {
            // HandleMainException can throw error when log4net is not found
            MessageBox.Show(loadError.ToString(), "Critical error (Logging service defect?)");
          }
        }
      }
      else
      {
        Run(startupArguments);
      }
    }

    private static void HandleMainException(StartupArguments args, Exception ex)
    {
      Current.Log.Fatal(ex);
      try
      {
        new ExceptionBox(ex, "Unhandled exception terminated " + args.ApplicationName, true).ShowDialog();
      }
      catch
      {
        MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
      }
    }

    private static void Run(StartupArguments startupArguments)
    {
      // DO NOT USE LoggingService HERE!
      // LoggingService requires ICSharpCode.Core.dll and log4net.dll
      // When a method containing a call to LoggingService is JITted, the
      // libraries are loaded.
      // We want to show the SplashScreen while those libraries are loading, so
      // don't call Current.Log.

#if DEBUG
      Control.CheckForIllegalCrossThreadCalls = true;
#endif
      bool noLogo = false;

      Application.SetCompatibleTextRenderingDefault(false);

      foreach (string parameter in startupArguments.ParameterList)
      {
        if ("nologo".Equals(parameter, StringComparison.OrdinalIgnoreCase))
          noLogo = true;

        // do not show logo if Altaxo is started as COM server
        if ("embedding".Equals(parameter, StringComparison.OrdinalIgnoreCase))
          noLogo = true;
      }

      if (!CheckEnvironment(startupArguments)) // check the framework version
        return;

      if (!noLogo)
      {
        SplashScreenForm.ShowSplashScreen(startupArguments.ApplicationName); // show splash screen
      }

      try
      {
        RunApplication(startupArguments); // now run the application
      }
      finally
      {
        if (SplashScreenForm.SplashScreen is not null)
        {
          SplashScreenForm.SplashScreen.Dispose();
        }
      }
    }

    /// <summary>
    /// Checks if we do have a compatible dotnet framework version for running this app.
    /// </summary>
    /// <returns></returns>
    private static bool CheckEnvironment(StartupArguments args)
    {
      // Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case the app is
      // used on another machine than it was installed on (e.g. "on USB stick")
      if (!Altaxo.Main.Services.NetFrameworkVersionDetermination.IsVersion48Installed())
      {
        MessageBox.Show(string.Format("This version of {0} requires .NET 4.8 You are using: {1}", args.ApplicationName, Environment.Version));
        return false;
      }
      // Work around a WPF issue when %WINDIR% is set to an incorrect path
      string windir = Environment.GetFolderPath(Environment.SpecialFolder.Windows, Environment.SpecialFolderOption.DoNotVerify);
      if (Environment.GetEnvironmentVariable("WINDIR") != windir)
      {
        Environment.SetEnvironmentVariable("WINDIR", windir);
      }
      return true;
    }

    private static void RunApplication(StartupArguments startupArguments)
    {
#if DEBUG
      // The output encoding differs based on whether the app is a console app (debug mode)
      // or Windows app (release mode). Because this flag also affects the default encoding
      // when reading from other processes' standard output, we explicitly set the encoding to get
      // consistent behaviour in debug and release builds.

      // Console apps use the system's OEM codepage, windows apps the ANSI codepage.
      // We'll always use the Windows (ANSI) codepage.
      try
      {
        Console.OutputEncoding = System.Text.Encoding.Default;
      }
      catch (IOException)
      {
        // can happen if the application doesn't have a console appended
      }
#endif

      Current.Log.Info(string.Format("Starting {0}...", startupArguments.ApplicationName));
      Altaxo.Main.Services.IAutoUpdateInstallationService? updateInstaller = null;
      try
      {
        var startupSettings = new StartupSettings(startupArguments.ApplicationName, startupArguments.StartupArgs, startupArguments.RequestedFileList, startupArguments.ParameterList);

#if DEBUG
        startupSettings.UseExceptionBoxForErrorHandler = UseExceptionBox(startupArguments.StartupArgs);
#endif

        var thisAssemblyLocation = typeof(StartupMain).Assembly.Location ?? throw new InvalidOperationException("Unable to get the location of this assembly");
        var thisAssemblyPath = Path.GetDirectoryName(thisAssemblyLocation) ?? throw new InvalidOperationException("Unable to get path of this assembly");
        startupSettings.ApplicationRootPath = Path.Combine(thisAssemblyPath, "..");
        startupSettings.AllowUserAddIns = true;

        var configDirectory = System.Configuration.ConfigurationManager.AppSettings["settingsPath"];
        if (string.IsNullOrEmpty(configDirectory))
        {
          var relativeConfigDirectory = System.Configuration.ConfigurationManager.AppSettings["relativeSettingsPath"];
          if (string.IsNullOrEmpty(relativeConfigDirectory))
            startupSettings.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), startupArguments.ApplicationName);
          else
            startupSettings.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), startupArguments.ApplicationName, relativeConfigDirectory);
        }
        else
        {
          startupSettings.ConfigDirectory = Path.Combine(thisAssemblyPath, configDirectory);
        }

        startupSettings.AddAddInsFromDirectory(Path.Combine(startupSettings.ApplicationRootPath, "AddIns"));

        // allows testing addins without having to install them, by providing them in a command line
        const string addinCommandStart = "addindir:";
        foreach (string parameter in startupSettings.ParameterList)
        {
          if (parameter.StartsWith(addinCommandStart, StringComparison.OrdinalIgnoreCase))
          {
            startupSettings.AddAddInsFromDirectory(parameter.Substring(addinCommandStart.Length));
          }
        }

        // Start ServiceSystem, PropertyService, ResourceService, Load Addins
        InitializeApplication(startupSettings); // initialize core, load all addins

        if (startupSettings.RequestedFileList.Length > 0)
        {
          if (LoadFilesInPreviousInstance(startupSettings.RequestedFileList))
          {
            Current.Log.Info("Aborting startup, arguments will be handled by previous instance");
            return;
          }
        }

        updateInstaller = Altaxo.Current.GetService<Altaxo.Main.Services.IAutoUpdateInstallationService>();
        if (updateInstaller is not null)
        {
          if (updateInstaller.Run(true, startupArguments.StartupArgs))
            return;
        }

        // Start Com
        var comManager = Altaxo.Current.GetService<Altaxo.Main.IComManager>();
        if (comManager is not null)
        {
          if (!comManager.ProcessStartupArguments(startupArguments.StartupArgs))
            return;
        }

        RunWorkbench(
                    startupSettings,
                    () =>
                    {
                      var splashScreen = SplashScreenForm.SplashScreen;
                      if (splashScreen is not null)
                      {
                        splashScreen.BeginInvoke(new MethodInvoker(splashScreen.Dispose));
                        SplashScreenForm.SplashScreen = null;
                      }
                    }, null);
      }
      finally
      {
        Current.Log.Info("Leaving RunApplication()");
      }

      updateInstaller?.Run(false, null);
    }

    private static bool LoadFilesInPreviousInstance(string[] fileList)
    {
      return false;
    }

    /// <summary>
    /// Initializes the application.
    /// </summary>
    /// <param name="startupSettings">The settings used for startup of the application.</param>
    public static void InitializeApplication(StartupSettings startupSettings)
    {
      Current.IsInDesignMode = false; // we are not in design mode

      // Initialize the most important services:
      var container = new AltaxoServiceContainer();
      container.AddFallbackProvider(Current.FallbackServiceProvider);
      container.AddService(typeof(IMessageService), new Altaxo.Main.Services.MessageServiceImpl());
      //			container.AddService(typeof(ILoggingService), new log4netLoggingService());
      Current.Services = container;

      var unhandledExceptionHandlerService = new UnhandledExceptionHandlerService();
      Current.AddService<IUnhandledExceptionHandlerService>(unhandledExceptionHandlerService);

      Current.Log.Info("Initialize application...");
      var startup = new CoreStartup(startupSettings.ApplicationName);
      if (startupSettings.UseExceptionBoxForErrorHandler)
      {
        unhandledExceptionHandlerService.AddHandler(new ExceptionBox.UnhandledHandler(), false);
      }
      var configDirectory = startupSettings.ConfigDirectory;
      var dataDirectory = startupSettings.DataDirectory;
      string propertiesName;
      if (startupSettings.PropertiesName is not null)
      {
        propertiesName = startupSettings.PropertiesName;
      }
      else
      {
        propertiesName = startupSettings.ApplicationName + "Properties";
      }

      if (startupSettings.ApplicationRootPath is not null)
      {
        FileUtility.ApplicationRootPath = startupSettings.ApplicationRootPath;
      }

      if (configDirectory is null)
        configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                                     startupSettings.ApplicationName);

      var propertyService = new Altaxo.Main.Services.PropertyService(
          DirectoryName.Create(configDirectory),
          DirectoryName.Create(dataDirectory ?? Path.Combine(FileUtility.ApplicationRootPath, "data")),
          DirectoryName.Create(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), startupSettings.ApplicationName)),
          propertiesName);
      startup.StartCoreServices(propertyService, startupSettings);

      var exe = Assembly.Load(startupSettings.ResourceAssemblyName);
      Current.ResourceService.RegisterNeutralStrings(new ResourceManager("Altaxo.Resources.UnprefixedStrings", exe), $"{exe.FullName} /  Altaxo.Resources.UnprefixedStrings");
      Current.ResourceService.RegisterNeutralImages(new ResourceManager("Altaxo.Resources.UnprefixedImages", exe), $"{exe.FullName} /  Altaxo.Resources.UnprefixedImages");

      CommandWrapper.LinkCommandCreator = (link => new LinkCommand(link)); // creation of command for opening web sites
      CommandWrapper.WellKnownCommandCreator = MenuService.GetKnownCommand; // creation of all other commands
      CommandWrapper.RegisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested += eh); // CommandWrapper has to know how to subscribe to the RequerySuggested event of the command manager
      CommandWrapper.UnregisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested -= eh); // CommandWrapper must know how to unsubscribe to the RequerySuggested event of the command manager

      Current.Log.Info("Looking for AddIns...");
      foreach (string file in startupSettings._addInFiles)
      {
        startup.AddAddInFile(file);
      }
      foreach (string dir in startupSettings._addInDirectories)
      {
        startup.AddAddInsFromDirectory(dir);
      }

      if (startupSettings.AllowAddInConfigurationAndExternalAddIns)
      {
        startup.ConfigureExternalAddIns(Path.Combine(configDirectory, "AddIns.xml"));
      }
      if (startupSettings.AllowUserAddIns)
      {
        startup.ConfigureUserAddIns(Path.Combine(configDirectory, "AddInInstallTemp"),
            Path.Combine(configDirectory, "AddIns"));
      }

      Current.Log.Info("Loading AddInTree...");
      startup.RunInitialization();

      Current.Log.Info("Init application finished");
    }

    /// <summary>
    /// Runs the workbench.
    /// </summary>
    /// <param name="wbSettings">The workbench settings.</param>
    /// <param name="BeforeRunWorkbench">Action(s) that are executed immediatly before the workbench runs. May be null.</param>
    /// <param name="WorkbenchClosed">Action(s) that are executed immediatly after the workbench has closed. May be null.</param>
    /// <exception cref="RunWorkbenchException"></exception>
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
    private static void RunWorkbench(StartupSettings wbSettings, Action BeforeRunWorkbench, Action? WorkbenchClosed)
    {
      Current.Log.Info("Initializing workbench...");
      var wbc = new WorkbenchStartup();

      RunWorkbenchInitializedCommands();

      Current.Log.Info("Starting workbench...");
      Exception? exception = null;
      // finally start the workbench.
      try
      {
        BeforeRunWorkbench?.Invoke();
        if (Debugger.IsAttached)
        {
          wbc.Run(wbSettings);
        }
        else
        {
          try
          {
            wbc.Run(wbSettings);
          }
          catch (Exception ex)
          {
            exception = ex;
          }
        }
      }
      finally
      {
      }
      Current.Log.Info("Finished running workbench.");
      WorkbenchClosed?.Invoke();
      if (exception is not null)
      {
        const string errorText = "Unhandled exception terminated the workbench";
        Current.Log.Fatal(exception);
        if (wbSettings.UseExceptionBoxForErrorHandler)
        {
          new ExceptionBox(exception, errorText, true).ShowDialog();
        }
        else
        {
          throw new RunWorkbenchException(errorText, exception);
        }
      }
    }

    private static void RunWorkbenchInitializedCommands()
    {
      if (Current.ComManager is not null && Current.ComManager.ApplicationWasStartedWithEmbeddingArg)
      {
        Current.ComManager.StartLocalServer();
      }

      foreach (ICommand command in AddInTree.BuildItems<ICommand>(StringParser.Parse("/${AppName}/Workbench/AutostartAfterWorkbenchInitialized"), null, false))
      {
        try
        {
          command.Execute(null);
        }
        catch (Exception ex)
        {
          // allow startup to continue if some commands fail
          MessageService.ShowException(ex);
        }
      }
    }
  }
}
