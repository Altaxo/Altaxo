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
using Altaxo.Gui.Workbench;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Commands;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Altaxo.Gui.Startup
{
	internal sealed class CallHelper : MarshalByRefObject
	{
		private SharpDevelopHost.CallbackHelper callback;
		private bool useSharpDevelopErrorHandler;

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#region Initialize Core

		public void InitSharpDevelopCore(SharpDevelopHost.CallbackHelper callback, StartupSettings properties)
		{
			// Initialize the most important services:
			var container = new SharpDevelopServiceContainer();
			container.AddFallbackProvider(ServiceSingleton.FallbackServiceProvider);
			container.AddService(typeof(IMessageService), new Altaxo.Main.Services.MessageServiceImpl());
			//			container.AddService(typeof(ILoggingService), new log4netLoggingService());
			ServiceSingleton.ServiceProvider = container;

			LoggingService.Info("InitSharpDevelop...");
			this.callback = callback;
			CoreStartup startup = new CoreStartup(properties.ApplicationName);
			if (properties.UseExceptionBoxForErrorHandler)
			{
				this.useSharpDevelopErrorHandler = true;
				ExceptionBox.RegisterExceptionBoxForUnhandledExceptions();
			}
			string configDirectory = properties.ConfigDirectory;
			string dataDirectory = properties.DataDirectory;
			string propertiesName;
			if (properties.PropertiesName != null)
			{
				propertiesName = properties.PropertiesName;
			}
			else
			{
				propertiesName = properties.ApplicationName + "Properties";
			}

			if (properties.ApplicationRootPath != null)
			{
				FileUtility.ApplicationRootPath = properties.ApplicationRootPath;
			}

			if (configDirectory == null)
				configDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
																			 properties.ApplicationName);

			var propertyService = new Altaxo.Main.Services.PropertyService(
				DirectoryName.Create(configDirectory),
				DirectoryName.Create(dataDirectory ?? Path.Combine(FileUtility.ApplicationRootPath, "data")),
				propertiesName);
			Altaxo.Current.SetPropertyService(propertyService);
			startup.StartCoreServices(propertyService);

			Assembly exe = Assembly.Load(properties.ResourceAssemblyName);
			SD.ResourceService.RegisterNeutralStrings(new ResourceManager("Altaxo.Resources.StringResources", exe));
			SD.ResourceService.RegisterNeutralImages(new ResourceManager("Altaxo.Resources.BitmapResources", exe));
			SD.ResourceService.RegisterNeutralStrings(new ResourceManager("Altaxo.Resources.AltaxoString", exe));
			SD.ResourceService.RegisterNeutralImages(new ResourceManager("Altaxo.Resources.AltaxoBitmap", exe));

			CommandWrapper.LinkCommandCreator = (link => new LinkCommand(link));
			CommandWrapper.WellKnownCommandCreator = ICSharpCode.Core.Presentation.MenuService.GetKnownCommand;
			CommandWrapper.RegisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested += eh);
			CommandWrapper.UnregisterConditionRequerySuggestedHandler = (eh => CommandManager.RequerySuggested -= eh);

			LoggingService.Info("Looking for AddIns...");
			foreach (string file in properties.addInFiles)
			{
				startup.AddAddInFile(file);
			}
			foreach (string dir in properties.addInDirectories)
			{
				startup.AddAddInsFromDirectory(dir);
			}

			if (properties.AllowAddInConfigurationAndExternalAddIns)
			{
				startup.ConfigureExternalAddIns(Path.Combine(configDirectory, "AddIns.xml"));
			}
			if (properties.AllowUserAddIns)
			{
				startup.ConfigureUserAddIns(Path.Combine(configDirectory, "AddInInstallTemp"),
					Path.Combine(configDirectory, "AddIns"));
			}

			LoggingService.Info("Loading AddInTree...");
			startup.RunInitialization();

			// Register events to marshal back
			FileUtility.FileLoaded += delegate (object sender, FileNameEventArgs e) { this.callback.FileLoaded(e.FileName); };
			FileUtility.FileSaved += delegate (object sender, FileNameEventArgs e) { this.callback.FileSaved(e.FileName); };

			LoggingService.Info("InitSharpDevelop finished");
		}

		#endregion Initialize Core

		#region Initialize and run Workbench

		public void RunWorkbench(WorkbenchSettings settings)
		{
			if (settings.RunOnNewThread)
			{
				Thread t = new Thread(RunWorkbenchInternal);
				t.SetApartmentState(ApartmentState.STA);
				t.Name = "SDmain";
				t.Start(settings);
			}
			else
			{
				RunWorkbenchInternal(settings);
			}
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void RunWorkbenchInternal(object settings)
		{
			WorkbenchSettings wbSettings = (WorkbenchSettings)settings;

			WorkbenchStartup wbc = new WorkbenchStartup();
			LoggingService.Info("Initializing workbench...");
			wbc.InitializeWorkbench();

			RunWorkbenchInitializedCommands();

			LoggingService.Info("Starting workbench...");
			Exception exception = null;
			// finally start the workbench.
			try
			{
				callback.BeforeRunWorkbench();
				if (Debugger.IsAttached)
				{
					wbc.Run(wbSettings.InitialFileList);
				}
				else
				{
					try
					{
						wbc.Run(wbSettings.InitialFileList);
					}
					catch (Exception ex)
					{
						exception = ex;
					}
				}
			}
			finally
			{
				LoggingService.Info("Unloading services...");
				try
				{
					// see IShutdownService.Shutdown for a description of the shut down procedure
					WorkbenchSingleton.OnWorkbenchUnloaded();
					var propertyService = SD.PropertyService;
					var shutdownService = (ShutdownService)SD.ShutdownService;
					shutdownService.WaitForBackgroundTasks();
					((IDisposable)SD.Services).Dispose(); // dispose all services
					propertyService.Save();
				}
				catch (Exception ex)
				{
					LoggingService.Warn("Exception during unloading", ex);
					if (exception == null)
					{
						exception = ex;
					}
				}
			}
			LoggingService.Info("Finished running workbench.");
			callback.WorkbenchClosed();
			if (exception != null)
			{
				const string errorText = "Unhandled exception terminated the workbench";
				LoggingService.Fatal(exception);
				if (useSharpDevelopErrorHandler)
				{
					new ExceptionBox(exception, errorText, true).ShowDialog();
				}
				else
				{
					throw new RunWorkbenchException(errorText, exception);
				}
			}
		}

		private void RunWorkbenchInitializedCommands()
		{
			if (Current.ComManager.ApplicationWasStartedWithEmbeddingArg)
			{
				// System.Diagnostics.Debugger.Launch();
				Current.ComManager.StartLocalServer();
			}

			foreach (ICommand command in AddInTree.BuildItems<ICommand>("/SharpDevelop/Workbench/AutostartAfterWorkbenchInitialized", null, false))
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

		#endregion Initialize and run Workbench

		public bool CloseWorkbench(bool force)
		{
			return SD.MainThread.InvokeIfRequired(() => CloseWorkbenchInternal(force));
		}

		private bool CloseWorkbenchInternal(bool force)
		{
			foreach (IWorkbenchWindow window in SD.Workbench.WorkbenchWindowCollection.ToArray())
			{
				if (!window.CloseWindow(force))
					return false;
			}
			SD.Workbench.MainWindow.Close();
			return true;
		}

		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "needs to be run in correct AppDomain")]
		public void KillWorkbench()
		{
			Dispatcher.CurrentDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
		}

		public bool WorkbenchVisible
		{
			get
			{
				return SD.MainThread.InvokeIfRequired<bool>(GetWorkbenchVisibleInternal);
			}
			set
			{
				SD.MainThread.InvokeIfRequired(() => SetWorkbenchVisibleInternal(value));
			}
		}

		private bool GetWorkbenchVisibleInternal()
		{
			return SD.Workbench.MainWindow.Visibility == Visibility.Visible;
		}

		private void SetWorkbenchVisibleInternal(bool value)
		{
			SD.Workbench.MainWindow.Visibility = value ? Visibility.Visible : Visibility.Hidden;
		}
	}
}