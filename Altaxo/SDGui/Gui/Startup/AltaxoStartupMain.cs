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

using System;

using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Altaxo.Gui.Startup
{
	/// <summary>
	/// This Class is the Core main class, it starts the program.
	/// </summary>
	public static class AltaxoStartupMain
	{
		private static string[] commandLineArgs = null;

		public static string[] CommandLineArgs
		{
			get
			{
				return commandLineArgs;
			}
		}

		private static bool UseExceptionBox
		{
			get
			{
#if DEBUG
				if (Debugger.IsAttached) return false;
#endif
				foreach (string arg in commandLineArgs)
				{
					if (arg.Contains("noExceptionBox")) return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Starts the core of SharpDevelop.
		/// </summary>
		[STAThread()]
		public static void Main(string[] args)
		{
			commandLineArgs = args; // Needed by UseExceptionBox

			// Do not use LoggingService here (see comment in Run(string[]))
			if (UseExceptionBox)
			{
				try
				{
					Run();
				}
				catch (Exception ex)
				{
					try
					{
						HandleMainException(ex);
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
				Run();
			}
		}

		private static void HandleMainException(Exception ex)
		{
			LoggingService.Fatal(ex);
			try
			{
				new ExceptionBox(ex, "Unhandled exception terminated Altaxo", true).ShowDialog();
			}
			catch
			{
				MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
			}
		}

		private static void Run()
		{
			// DO NOT USE LoggingService HERE!
			// LoggingService requires ICSharpCode.Core.dll and log4net.dll
			// When a method containing a call to LoggingService is JITted, the
			// libraries are loaded.
			// We want to show the SplashScreen while those libraries are loading, so
			// don't call LoggingService.

#if DEBUG
			Control.CheckForIllegalCrossThreadCalls = true;
#endif
			bool noLogo = false;

			Application.SetCompatibleTextRenderingDefault(false);
			SplashScreenForm.SetCommandLineArgs(commandLineArgs);

			foreach (string parameter in SplashScreenForm.GetParameterList())
			{
				if ("nologo".Equals(parameter, StringComparison.OrdinalIgnoreCase))
					noLogo = true;

				// do not show logo if Altaxo is started as COM server
				if ("embedding".Equals(parameter, StringComparison.OrdinalIgnoreCase))
					noLogo = true;
			}

			if (!CheckEnvironment())
				return;

			if (!noLogo)
			{
				SplashScreenForm.ShowSplashScreen();
			}
			try
			{
				RunApplication();
			}
			finally
			{
				if (SplashScreenForm.SplashScreen != null)
				{
					SplashScreenForm.SplashScreen.Dispose();
				}
			}
		}

		private static bool CheckEnvironment()
		{
			// Safety check: our setup already checks that .NET 4 is installed, but we manually check the .NET version in case SharpDevelop is
			// used on another machine than it was installed on (e.g. "SharpDevelop on USB stick")
			if (!DotnetDetection.IsDotnet46Installed())
			{
				MessageBox.Show("This version of SharpDevelop requires .NET 4.6. You are using: " + Environment.Version, "SharpDevelop");
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

		private static void RunApplication()
		{
#if ModifiedForAltaxo
			var originalUICulture = System.Globalization.CultureInfo.CurrentUICulture;
			var originalCulture = System.Globalization.CultureInfo.CurrentCulture;
#endif

			// The output encoding differs based on whether SharpDevelop is a console app (debug mode)
			// or Windows app (release mode). Because this flag also affects the default encoding
			// when reading from other processes' standard output, we explicitly set the encoding to get
			// consistent behaviour in debug and release builds of SharpDevelop.

#if DEBUG
			// Console apps use the system's OEM codepage, windows apps the ANSI codepage.
			// We'll always use the Windows (ANSI) codepage.
			try
			{
				Console.OutputEncoding = System.Text.Encoding.Default;
			}
			catch (IOException)
			{
				// can happen if SharpDevelop doesn't have a console
			}
#endif

			LoggingService.Info("Starting SharpDevelop...");
			try
			{
				StartupSettings startup = new StartupSettings();
#if DEBUG
				startup.UseExceptionBoxForErrorHandler = UseExceptionBox;
#endif

				Assembly exe = typeof(AltaxoStartupMain).Assembly;
				startup.ApplicationRootPath = Path.Combine(Path.GetDirectoryName(exe.Location), "..");
				startup.AllowUserAddIns = true;

				string configDirectory = System.Configuration.ConfigurationManager.AppSettings["settingsPath"];
				if (String.IsNullOrEmpty(configDirectory))
				{
#if ModifiedForAltaxo
					startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
																								 "Altaxo\\Altaxo4");
					startup.ResourceAssemblyName = "AltaxoSDGui";
					startup.UseExceptionBoxForErrorHandler = true;
					//SD.ResourceService.RegisterNeutralStrings(new System.Resources.ResourceManager("Resources.AltaxoString", exe));
					//SD.ResourceService.RegisterNeutralImages(new System.Resources.ResourceManager("Resources.AltaxoBitmap", exe));
#else

					startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
																								 "ICSharpCode/SharpDevelop" + RevisionClass.Major);
#endif
				}
				else
				{
					startup.ConfigDirectory = Path.Combine(Path.GetDirectoryName(exe.Location), configDirectory);
				}

				startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));

				// allows testing addins without having to install them
				foreach (string parameter in SplashScreenForm.GetParameterList())
				{
					if (parameter.StartsWith("addindir:", StringComparison.OrdinalIgnoreCase))
					{
						startup.AddAddInsFromDirectory(parameter.Substring(9));
					}
				}

				SharpDevelopHost host = new SharpDevelopHost(AppDomain.CurrentDomain, startup);

				string[] fileList = SplashScreenForm.GetRequestedFileList();
				if (fileList.Length > 0)
				{
					if (LoadFilesInPreviousInstance(fileList))
					{
						LoggingService.Info("Aborting startup, arguments will be handled by previous instance");
						return;
					}
				}

				host.BeforeRunWorkbench += delegate
				{
					if (SplashScreenForm.SplashScreen != null)
					{
						SplashScreenForm.SplashScreen.BeginInvoke(new MethodInvoker(SplashScreenForm.SplashScreen.Dispose));
						SplashScreenForm.SplashScreen = null;
					}
				};
#if ModifiedForAltaxo
				Altaxo.Settings.CultureSettingsAtStartup.StartupUICultureInfo = originalUICulture;
				Altaxo.Settings.CultureSettingsAtStartup.StartupDocumentCultureInfo = originalCulture;

				Altaxo.Main.Commands.AutostartCommand.EarlyRun(CommandLineArgs);
				if (Altaxo.Serialization.AutoUpdates.UpdateInstallerStarter.Run(true, AltaxoStartupMain.CommandLineArgs))
					return;

				var comManager = new Altaxo.Com.ComManager(new Altaxo.Com.AltaxoComApplicationAdapter());
				Altaxo.Current.SetComManager(comManager);
				comManager.ProcessArguments(CommandLineArgs);
				if (comManager.ApplicationShouldExitAfterProcessingArgs)
					return;

				//var altaxoWb = new Altaxo.Gui.SharpDevelop.AltaxoSDWorkbench();
				//Altaxo.Current.SetWorkbench(altaxoWb);
				// TODO SD.Workbench.InitializeWorkbench(altaxoWb, new ICSharpCode.SharpDevelop.Gui.AvalonDockLayout());
				//new Altaxo.Main.Commands.AutostartCommand().Run();
				/*
				if (comManager.ApplicationWasStartedWithEmbeddingArg)
				{
					//System.Diagnostics.Debugger.Launch();
					comManager.StartLocalServer();
				}
				*/
#endif

				WorkbenchSettings workbenchSettings = new WorkbenchSettings();
				workbenchSettings.RunOnNewThread = false;
				for (int i = 0; i < fileList.Length; i++)
				{
					workbenchSettings.InitialFileList.Add(fileList[i]);
				}
				host.RunWorkbench(workbenchSettings);
			}
			finally
			{
				LoggingService.Info("Leaving RunApplication()");
			}

#if ModifiedForAltaxo
			Altaxo.Serialization.AutoUpdates.UpdateInstallerStarter.Run(false, null);
#endif
		}

		private static bool LoadFilesInPreviousInstance(string[] fileList)
		{
			return false;
		}
	}
}