// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;

using ICSharpCode.Core.Services;
using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class Compile : AbstractMenuCommand
	{
		public static object CompileLockObject = new Compile();
		
		public static void ShowAfterCompileStatus()
		{
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			if (!taskService.SomethingWentWrong) {
				statusBarService.SetMessage("${res:MainWindow.StatusBar.SuccessfulMessage}");
			} else {
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				stringParserService.Properties["Errors"]   = taskService.Errors.ToString();
				stringParserService.Properties["Warnings"] = taskService.Warnings.ToString();
				statusBarService.SetMessage("${res:MainWindow.StatusBar.ErrorWarningsMessage}");
			}
		}
		
		void CompileThread()
		{
			lock (Compile.CompileLockObject) {
				CombineEntry.BuildProjects = 0;
				CombineEntry.BuildErrors   = 0;
				
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				try {
					if (projectService.CurrentOpenCombine != null) {
						projectService.CompileCombine();
						ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
							ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName)) {
									IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
									stringParserService.Properties["LanguageBinding"] = binding == null ? "null" : binding.Language;
									stringParserService.Properties["FileName"] = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
									messageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.RunCompile.LanguageBindingCantCompileFileError}");
								} else {
									new SaveFile().Run();
									ICompilerResult res = binding.CompileFile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName);
									taskService.Tasks.Clear();
									foreach (CompilerError err in res.CompilerResults.Errors) {
										taskService.Tasks.Add(new Task(null, err));
									}
									taskService.CompilerOutput = res.CompilerOutput;
									taskService.NotifyTaskChange();
									ShowAfterCompileStatus();
								}
							} else {
								// should never happen anymore
								IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
								messageService.ShowError("No source file for compilation found. Please save unsaved files");
							}
						}
					}
					taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.DoneLabel") + "\n\n" +
					stringParserService.Parse("${res:MainWindow.CompilerMessages.StatsOutput}", new string[,] { {"SUCCEEDED", CombineEntry.BuildProjects.ToString()}, {"FAILED", CombineEntry.BuildErrors.ToString()} }) + "\n";
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Error while compiling");
				}
				projectService.OnEndBuild();
			}
		}
		
		public void RunWithWait()
		{
			CompileThread();
		}
		
		public override void Run()
		{
			lock (CompileLockObject) {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				
//				if (projectService.CurrentOpenCombine != null) {
					taskService.CompilerOutput = String.Empty;
					projectService.OnStartBuild();
					Thread t = new Thread(new ThreadStart(CompileThread));
					t.IsBackground  = true;
					t.Start();
//				}
				
			}
		}
	}
	public class CompileAll : AbstractMenuCommand
	{
		void CompileThread()
		{ 
			lock (Compile.CompileLockObject) {
				CombineEntry.BuildProjects = 0;
				CombineEntry.BuildErrors   = 0;
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				try {
					
					if (projectService.CurrentOpenCombine != null) {
						projectService.RecompileAll();
						Compile.ShowAfterCompileStatus();
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
							ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName);
							
							if (binding != null) {
								if (binding == null || !binding.CanCompile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName)) {
									IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
									stringParserService.Properties["LanguageBinding"] = binding == null ? "null" : binding.Language;
									stringParserService.Properties["FileName"] = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
									messageService.ShowError("${res:ICSharpCode.SharpDevelop.Commands.RunCompile.LanguageBindingCantCompileFileError}");
								} else {
									new SaveFile().Run();
									ICompilerResult res = binding.CompileFile(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName);
									taskService.Tasks.Clear();
									foreach (CompilerError err in res.CompilerResults.Errors) {
										taskService.Tasks.Add(new Task(null, err));
									}
									taskService.CompilerOutput = res.CompilerOutput;
									taskService.NotifyTaskChange();
									Compile.ShowAfterCompileStatus();
								}
							} else {
								// should never happen anymore
								IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
								messageService.ShowError("No source file for compilation found. Please save unsaved files");
							}
						}
					}
					taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.DoneLabel") + "\n\n" +
					stringParserService.Parse("${res:MainWindow.CompilerMessages.StatsOutput}", new string[,] { {"SUCCEEDED", CombineEntry.BuildProjects.ToString()}, {"FAILED", CombineEntry.BuildErrors.ToString()} }) + "\n";
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Error while compiling");
				}
				projectService.OnEndBuild();
			}
		}
		
		public override void Run()
		{
//			if (Monitor.TryEnter(Compile.CompileLockObject)) {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
//				if (projectService.CurrentOpenCombine != null) {
	
					taskService.CompilerOutput = String.Empty;
					projectService.OnStartBuild();
					Thread t = new Thread(new ThreadStart(CompileThread));
					t.IsBackground  = true;
					t.Start();
//				}
//				Monitor.Exit(Compile.CompileLockObject);
//			}
		}
	}
	
	public class RunHelper
	{
		bool debug;
		public RunHelper(bool debug)
		{
			this.debug = debug;
		}
		void RunThread()
		{
			lock (Compile.CompileLockObject) {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
				try {
					statusBarService.SetMessage("${res:MainWindow.StatusBar.ExecutingMessage}");
					if (projectService.CurrentOpenCombine != null) {
						try {
							if (projectService.NeedsCompiling) {
								projectService.CompileCombine();
							}
							projectService.OnBeforeStartProject();
							projectService.CurrentOpenCombine.Execute(debug);
							
						} catch (NoStartupCombineDefinedException) {
							IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
							messageService.ShowError("Cannot execute Run command, cannot find startup project.\nPlease define a startup project for the combine in the combine properties.");
						}
					} else {
						if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null) {
							new Compile().RunWithWait();
							if (!taskService.SomethingWentWrong) {
								LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
								ILanguageBinding binding = languageBindingService.GetBindingPerFileName(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName);
								if (binding != null) {
									projectService.OnBeforeStartProject();
									binding.Execute(WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName, debug);
								} else {
									IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
									messageService.ShowError("No runnable executable found.");
								}
							}
						}
					}
				} catch (Exception e) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(e, "Error while running");
				}
				statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
			}
		}
		public void StartThread()
		{
			Thread t = new Thread(new ThreadStart(RunThread));
			t.IsBackground  = true;
			t.Start();
		}
	}
	
	public class RunCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			new RunHelper(true).StartThread();
		}
	}
	
	public class RunWithoutDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			new RunHelper(false).StartThread();
		}
	}
	
	public class ContinueDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.Continue();
		}
	}
	
	public class BreakDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.Break();
		}
	}
	
	public class StopDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.Stop();
		}
	}
	
	public class StepDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.Step(false);
		}
	}
	
	public class StepIntoDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.Step(true);
		}
	}
	
	public class StepOutDebuggingCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			debuggerService.StepOut();
		}
	}
	
	
	public class BuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			lock (Compile.CompileLockObject) {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
						
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.Tasks.Clear();
						taskService.NotifyTaskChange();
			
						projectService.OnStartBuild();
						projectService.CompileProject(projectService.CurrentSelectedProject);
						// taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.DoneLabel") + "\n\n" +
						//     stringParserService.Parse("${res:MainWindow.CompilerMessages.StatsOutput}", new string[,] { {"SUCCEEDED", CombineEntry.BuildProjects.ToString()}, {"FAILED", CombineEntry.BuildErrors.ToString()} }) + "\n";
					} catch (Exception e) {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, "Error while compiling project " + projectService.CurrentSelectedProject.Name);
					}
					projectService.OnEndBuild();
				}
				Compile.ShowAfterCompileStatus();
			}
		}
	}
	
	public class RebuildCurrentProject : AbstractMenuCommand
	{
		public override void Run()
		{
			lock (Compile.CompileLockObject) {
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
				
				if (projectService.CurrentSelectedProject != null) {
					try {
						StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
						
						CombineEntry.BuildProjects = 0;
						CombineEntry.BuildErrors   = 0;
						taskService.CompilerOutput = String.Empty;
						taskService.Tasks.Clear();
						taskService.NotifyTaskChange();
				
						projectService.OnStartBuild();
						projectService.RecompileProject(projectService.CurrentSelectedProject);
						// taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.DoneLabel") + "\n\n" +
						//     stringParserService.Parse("${res:MainWindow.CompilerMessages.StatsOutput}", new string[,] { {"SUCCEEDED", CombineEntry.BuildProjects.ToString()}, {"FAILED", CombineEntry.BuildErrors.ToString()} }) + "\n";
					} catch (Exception e) {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						messageService.ShowError(e, "Error while compiling project " + projectService.CurrentSelectedProject.Name);
					}
					projectService.OnEndBuild();
				}					
				Compile.ShowAfterCompileStatus();
			}
		}
	}
}
