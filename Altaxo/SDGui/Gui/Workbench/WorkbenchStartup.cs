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
using Altaxo.Gui.Startup;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

using System;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace Altaxo.Gui.Workbench
{
	/// <summary>
	/// Runs workbench initialization.
	/// Is called by ICSharpCode.SharpDevelop.Sda and should not be called manually!
	/// </summary>
	internal class WorkbenchStartup
	{
		private const string workbenchMemento = "WorkbenchMemento";
		private const string activeContentState = "Workbench.ActiveContent";
		private App app;

		public void InitializeWorkbench()
		{
			app = new App();
			System.Windows.Forms.Integration.WindowsFormsHost.EnableWindowsFormsInterop();
			ComponentDispatcher.ThreadIdle -= ComponentDispatcher_ThreadIdle; // ensure we don't register twice
			ComponentDispatcher.ThreadIdle += ComponentDispatcher_ThreadIdle;
			LayoutConfiguration.LoadLayoutConfiguration();
			SD.Services.AddService(typeof(IMessageLoop), new DispatcherMessageLoop(app.Dispatcher, SynchronizationContext.Current));
			InitializeWorkbench(new AltaxoWorkbench(), new AvalonDockLayout());
		}

		private static void InitializeWorkbench(AltaxoWorkbench workbench, IWorkbenchLayout layout)
		{
			SD.Services.AddService(typeof(IWorkbench), workbench);
			Current.SetWorkbench(workbench);
			new Altaxo.Main.Commands.AutostartCommand().Run();

			//UILanguageService.ValidateLanguage();

			//TaskService.Initialize();
			//Project.CustomToolsService.Initialize();

			workbench.Initialize();
			workbench.RestoreWorkbenchStateFromPropertyService();
			workbench.WorkbenchLayout = layout;

			// HACK: eagerly load output pad because pad services cannnot be instanciated from background threads
			//SD.Services.AddService(typeof(IOutputPad), CompilerMessageView.Instance);

			var dlgMsgService = SD.MessageService as IDialogMessageService;
			if (dlgMsgService != null)
			{
			}

			var applicationStateInfoService = SD.GetService<ApplicationStateInfoService>();
			if (applicationStateInfoService != null)
			{
				applicationStateInfoService.RegisterStateGetter(activeContentState, delegate { return SD.Workbench.ActiveContent; });
			}

			WorkbenchSingleton.OnWorkbenchCreated();

			// initialize workbench-dependent services:
			NavigationService.InitializeService();

			workbench.ActiveContentChanged += delegate
			{
				Debug.WriteLine("ActiveContentChanged to " + workbench.ActiveContent);
				LoggingService.Debug("ActiveContentChanged to " + workbench.ActiveContent);
			};
			workbench.ActiveViewContentChanged += delegate
			{
				Debug.WriteLine("ActiveViewContentChanged to " + workbench.ActiveViewContent);
				LoggingService.Debug("ActiveViewContentChanged to " + workbench.ActiveViewContent);
			};
			workbench.ActiveWorkbenchWindowChanged += delegate
			{
				Debug.WriteLine("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
				LoggingService.Debug("ActiveWorkbenchWindowChanged to " + workbench.ActiveWorkbenchWindow);
			};
		}

		private static void ComponentDispatcher_ThreadIdle(object sender, EventArgs e)
		{
			System.Windows.Forms.Application.RaiseIdle(e);
		}

		public void Run(IList<string> fileList)
		{
			bool didLoadSolutionOrFile = false;

			NavigationService.SuspendLogging();

			foreach (string file in fileList)
			{
				LoggingService.Info("Open file " + file);
				didLoadSolutionOrFile = true;
				try
				{
					var fullFileName = FileName.Create(Path.GetFullPath(file));

					if (Current.ProjectService.IsAltaxoProjectFileExtension(fullFileName.GetExtension()))
					{
						Current.ProjectService.OpenProject((string)fullFileName, false);
					}
					else
					{
						SD.FileService.OpenFile(fullFileName);
					}
				}
				catch (Exception e)
				{
					MessageService.ShowException(e, "unable to open file " + file);
				}
			}

			// load previous solution
			if (!didLoadSolutionOrFile && SD.PropertyService.Get("SharpDevelop.LoadPrevProjectOnStartup", false))
			{
				if (SD.FileService.RecentOpen.RecentProjects.Count > 0)
				{
					try
					{
						Current.ProjectService.OpenProject((string)(SD.FileService.RecentOpen.RecentProjects[0]), false);
						didLoadSolutionOrFile = true;
					}
					catch (Exception ex)
					{
						MessageService.ShowException(ex);
					}
				}
			}

			if (!didLoadSolutionOrFile)
			{
				foreach (ICommand command in AddInTree.BuildItems<ICommand>("/SharpDevelop/Workbench/AutostartNothingLoaded", null, false))
				{
					try
					{
						command.Execute(null);
					}
					catch (Exception ex)
					{
						MessageService.ShowException(ex);
					}
				}
				StartPreloadThread();
			}

			NavigationService.ResumeLogging();

			// finally run the workbench window ...
			app.Run(SD.Workbench.MainWindow);

			// save the workbench memento in the ide properties
			try
			{
				((AltaxoWorkbench)SD.Workbench).StoreWorkbenchStateInPropertyService();
			}
			catch (Exception e)
			{
				MessageService.ShowException(e, "Exception while saving workbench state.");
			}
		}

		#region Preload-Thread

		private void StartPreloadThread()
		{
			// Wait until UI is responsive and pads are initialized before starting the thread.
			// We don't want to slow down SharpDevelop's startup, we just want to make opening
			// a project more responsive.
			// (and parallelism doesn't really help here; we're mostly waiting for the disk to load the code)
			// So we do our work in the background while the user decides which project to open.
			SD.MainThread.InvokeAsyncAndForget(
				() => new Thread(PreloadThread) { IsBackground = true, Priority = ThreadPriority.BelowNormal }.Start(),
				DispatcherPriority.ApplicationIdle
			);
		}

		private void PreloadThread()
		{
			// Pre-load some stuff to make SharpDevelop more responsive once it is started.
			LoggingService.Debug("Preload-Thread started.");
		}

		#endregion Preload-Thread
	}
}