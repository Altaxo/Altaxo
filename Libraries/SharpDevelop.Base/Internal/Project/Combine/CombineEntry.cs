// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public abstract class CombineEntry : IDisposable
	{
		public static int BuildProjects = 0;
		public static int BuildErrors   = 0;
		
		object    entry;
		
		ArrayList dependencies = new ArrayList();
		
		string    filename;
		public string Filename {
			get {
				return filename;
			}
			set {
				filename = value;
			}
		}
		
		public abstract string Name {
			get;
		}
		
		public object Entry {
			get {
				return entry;
			}
		}
		
		public CombineEntry(object entry, string filename)
		{
			this.entry = entry;
			this.filename = filename;
		}
		
		public void Dispose()
		{
			if (entry is IDisposable) {
				((IDisposable)entry).Dispose();
			}
		}
		
		public abstract void Build(bool doBuildAll);
		public abstract void Execute(bool debug);
		public abstract void Save();
	}
	
	public class ProjectCombineEntry : CombineEntry
	{
		IProject project;
		
		public bool IsDirty {
			get {
				return project.IsDirty;
			}
			set {
				project.IsDirty = value;
			}
		}
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public override string Name {
			get {
				return project.Name;
			}
		}
		
		public ProjectCombineEntry(IProject project, string filename) : base(project, filename)
		{
			this.project = project;
		}
		
				
		public override void Build(bool doBuildAll)
		{ // if you change something here look at the DefaultProjectService BeforeCompile method
			if (doBuildAll || IsDirty) {
				IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
				ICompilerResult res = projectService.CompileProject(project);
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				
				if (taskService.Errors > 0) {
					++BuildErrors;
					IsDirty = true;
				} else {
					++BuildProjects;
					IsDirty = false;
				}
				
// This code is now in the project service:
//				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
//				stringParserService.Properties["Project"] = Name;
//				IProjectService   projectService   = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
//				IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
//				TaskService       taskService      = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
//				IResourceService resourceService   = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
//				
//				statusBarService.SetMessage("${res:MainWindow.StatusBar.CompilingMessage}");
//				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
//				
//				// create output directory, if not exists
//				string outputDir = ((AbstractProjectConfiguration)project.ActiveConfiguration).OutputDirectory;
//				try {
//					DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
//					if (!directoryInfo.Exists) {
//						directoryInfo.Create();
//					}
//				} catch (Exception e) {
//					throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
//				}
//				
//				ILanguageBinding csc = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
//				
//				AbstractProjectConfiguration conf = project.ActiveConfiguration as AbstractProjectConfiguration;
//				taskService.CompilerOutput += stringParserService.Parse("${res:MainWindow.CompilerMessages.BuildStartedOutput}", new string[,] { {"PROJECT", Project.Name}, {"CONFIG", Project.ActiveConfiguration.Name} }) + "\n";
//				taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.PerformingMainCompilationOutput") + "\n";
//				
//				if (conf != null && conf.ExecuteBeforeBuild != null && conf.ExecuteBeforeBuild.Length > 0) {
//					string command   = conf.ExecuteBeforeBuild;
//					string arguments = conf.ExecuteBeforeBuildArguments;
//					
//					if (File.Exists(command)) {
//						taskService.CompilerOutput += "Execute : " + conf.ExecuteBeforeBuild;
//						ProcessStartInfo ps = new ProcessStartInfo(command, arguments);
//						ps.UseShellExecute = false;
//						ps.RedirectStandardOutput = true;
//						ps.WorkingDirectory = Path.GetDirectoryName(command);
//						Process process = new Process();
//						process.StartInfo = ps;
//						process.Start();
//						taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
//					}
//				}
//				
//				ICompilerResult res = csc.CompileProject(project);
//				
//				IsDirty = false;
//				foreach (CompilerError err in res.CompilerResults.Errors) {
//					IsDirty = true;
//					taskService.Tasks.Add(new Task(project, err));
//				}
//				
//				if (conf != null && !IsDirty && conf.ExecuteAfterBuild != null && conf.ExecuteAfterBuild.Length > 0) {
//					taskService.CompilerOutput += "Execute : " + conf.ExecuteAfterBuild;
//					string command   = conf.ExecuteAfterBuild;
//					string arguments = conf.ExecuteAfterBuildArguments;
//					
//					if (File.Exists(command)) {
//						ProcessStartInfo ps = new ProcessStartInfo(command, arguments);
//						ps.UseShellExecute = false;
//						ps.RedirectStandardOutput = true;
//						ps.WorkingDirectory = Path.GetDirectoryName(command);
//						Process process = new Process();
//						process.StartInfo = ps;
//						process.Start();
//						taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
//					}
//				}
//				
//				taskService.NotifyTaskChange();
//				
//				if (taskService.Errors > 0) {
//					++BuildErrors;
//				} else {
//					++BuildProjects;
//				}
//				
//				taskService.CompilerOutput += res.CompilerOutput + stringParserService.Parse("${res:MainWindow.CompilerMessages.ProjectStatsOutput}", new string[,] { {"ERRORS", taskService.Errors.ToString()}, {"WARNINGS", taskService.Warnings.ToString()} }) + "\n\n";
			}
		}
		
		public override void Execute(bool debug)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			if (binding == null) {
				throw new ApplicationException("can't find language binding for project ");
			}
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			
			if (taskService.Errors == 0) {
				
				bool treatWarningsAsErrors = false;
				if(project.ActiveConfiguration is AbstractProjectConfiguration) {
					treatWarningsAsErrors = ((AbstractProjectConfiguration)project.ActiveConfiguration).TreatWarningsAsErrors;
				}
				if(! taskService.HasCriticalErrors(treatWarningsAsErrors)) {
					binding.Execute(project, debug);
				}
			}
		}
		
		public override void Save()
		{
			project.SaveProject(Filename);
		}
	}
	
	public class CombineCombineEntry : CombineEntry
	{
		Combine combine;
		
		public Combine Combine {
			get {
				return combine;
			}
		}
		public override string Name {
			get {
				return combine.Name;
			}
		}
		
		public CombineCombineEntry(Combine combine, string filename) : base(combine, filename)
		{
			this.combine = combine;
		}
		
		public override void Build(bool doBuildAll)
		{
			combine.Build(doBuildAll);
		}
		
		public override void Execute(bool debug)
		{
			combine.Execute(debug);
		}
		
		public override void Save()
		{
			combine.SaveCombine(Filename);
			combine.SaveAllProjects();
		}
	}
}
