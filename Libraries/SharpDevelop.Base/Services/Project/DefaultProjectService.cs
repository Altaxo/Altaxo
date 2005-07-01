// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.CodeDom.Compiler;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Services
{
	
	public enum BeforeCompileAction {
		Nothing,
		SaveAllFiles,
		PromptForSave,
	}
	
	public class DefaultProjectService : AbstractService, IProjectService
	{
		IProject currentProject = null;
		Combine  currentCombine = null;
		Combine  openCombine    = null;
		
		string   openCombineFileName = null;
		
		public IProject CurrentSelectedProject {
			get {
				return currentProject;
			}
			set {
				System.Diagnostics.Debug.Assert(openCombine != null);
				currentProject = value;
				OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
			}
		}
		
		public Combine CurrentSelectedCombine {
			get {
				return currentCombine;
			}
			set {
				System.Diagnostics.Debug.Assert(openCombine != null);
				currentCombine = value;
				OnCurrentSelectedCombineChanged(new CombineEventArgs(currentCombine));
			}
		}
		
		public Combine CurrentOpenCombine {
			get {
				return openCombine;
			}
			set {
				openCombine = value;
			}
		}
		
		bool IsDirtyFileInCombine {
			get {
				ArrayList projects = Combine.GetAllProjects(openCombine);
				
				foreach (ProjectCombineEntry projectEntry in projects) {
					foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
						foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
							if (content.IsDirty && content.FileName == fInfo.Name) {
								return true;
							}
						}
					}
				}
				return false;
			}
		}
		
		public bool NeedsCompiling {
			get {
				if (openCombine == null) {
					return false;
				}
				return openCombine.NeedsBuilding || IsDirtyFileInCombine;
			}
		}
		
		public void SaveCombinePreferences()
		{
			if (CurrentOpenCombine != null) {
				SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
			}
		}
		
		public void CloseCombine()
		{
			CloseCombine(true);
		}

		public void CloseCombine(bool saveCombinePreferencies)
		{
			if (CurrentOpenCombine != null) {
				if (saveCombinePreferencies)
					SaveCombinePreferences(CurrentOpenCombine, openCombineFileName);
				
				Combine closedCombine = CurrentOpenCombine;
				CurrentSelectedProject = null;
				CurrentOpenCombine = CurrentSelectedCombine = null;
				openCombineFileName = null;
				WorkbenchSingleton.Workbench.CloseAllViews();
				OnCombineClosed(new CombineEventArgs(closedCombine));
				closedCombine.Dispose();
			}
		}
		
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		public void OpenCombine(string filename)
		{
			if (openCombine != null) {
				SaveCombine();
				CloseCombine();
			}
				
			if (!fileUtilityService.TestFileExists(filename)) {
				return;
			}
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			statusBarService.SetMessage("${res:MainWindow.StatusBar.OpeningCombineMessage}");
				
			if (Path.GetExtension(filename).ToUpper() == ".PRJX") {
				string validcombine = Path.ChangeExtension(filename, ".cmbx");
				if (File.Exists(validcombine)) {
					LoadCombine(validcombine);
				} else {
					Combine loadingCombine = new Combine();
					IProject project = (IProject)loadingCombine.AddEntry(filename);
					if (project == null) {
						return;
					}
					loadingCombine.Name = project.Name;
					loadingCombine.SaveCombine(validcombine);
					LoadCombine(validcombine);
				}
			} else {
				LoadCombine(filename);
			}
			statusBarService.SetMessage("${res:MainWindow.StatusBar.ReadyMessage}");
		}
		
		void LoadCombine(string filename)
		{
			if (!fileUtilityService.TestFileExists(filename)) {
				return;
			}
			
			Combine loadingCombine = new Combine();
			loadingCombine.LoadCombine(filename);
			openCombine         = loadingCombine;
			openCombineFileName = filename;
			
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			fileService.RecentOpen.AddLastProject(filename);
			
			OnCombineOpened(new CombineEventArgs(openCombine));
			
			RestoreCombinePreferences(CurrentOpenCombine, openCombineFileName);
		}
		
		void Save(string fileName)
		{
			if (openCombine != null) {
				openCombineFileName = fileName;
				openCombine.SaveCombine(fileName);
				openCombine.SaveAllProjects();
			}
		}
		
		public ProjectReference GetReferenceFromProject(IProject prj, string filename)
		{
			foreach (ProjectReference rInfo in prj.ProjectReferences) {
				if (rInfo.Reference == filename) {
					return rInfo;
				}
			}
			ProjectReference newReferenceInformation = new ProjectReference(ReferenceType.Assembly, filename);
			prj.ProjectReferences.Add(newReferenceInformation);
			return newReferenceInformation;
		}
		
		public bool AddReferenceToProject(IProject prj, ProjectReference reference)
		{
			foreach (ProjectReference refproj in currentProject.ProjectReferences) {
				if (reference.Equals(refproj)) {
					return false;
				}
			}
			prj.ProjectReferences.Add(reference);
			ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView pbv = (ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView)WorkbenchSingleton.Workbench.GetPad(typeof(ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser.ProjectBrowserView));
			pbv.UpdateCombineTree();
			SaveCombine();
			return true;
		}
		
		public ProjectFile AddFileToProject(IProject prj, string filename, BuildAction action)
		{
			foreach (ProjectFile fInfo in prj.ProjectFiles) {
				if (fInfo.Name == filename) {
					return fInfo;
				}
			}
			ProjectFile newFileInformation = new ProjectFile(filename, action);
			prj.ProjectFiles.Add(newFileInformation);
			return newFileInformation;
		}
		
		public void AddFileToProject(IProject prj, ProjectFile projectFile) {
			prj.ProjectFiles.Add(projectFile);
			
		}

		
		public void SaveCombine()
		{
			Save(openCombineFileName);
		}
		
		public void MarkFileDirty(string filename)
		{
			if (openCombine != null) {
				ProjectCombineEntry entry = openCombine.GetProjectEntryContaining(filename);
				if (entry != null) {
					entry.IsDirty = true;
				}
			}
		}
		
		public void MarkProjectDirty(IProject project)
		{
			if (openCombine != null) {
				ArrayList projectEntries = Combine.GetAllProjects(openCombine);
				foreach (ProjectCombineEntry entry in projectEntries) {
					if (entry.Project == project) {
						entry.IsDirty = true;
						break;
					}
				}
			}
		}
		
		public void CompileCombine()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				taskService.Tasks.Clear();
				taskService.NotifyTaskChange();
				
				openCombine.Build(false);
			}
		}
		
		public void RecompileAll()
		{
			if (openCombine != null) {
				DoBeforeCompileAction();
				TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
				taskService.Tasks.Clear();
				taskService.NotifyTaskChange();
				
				openCombine.Build(true);
			}
		}
		
		ILanguageBinding BeforeCompile(IProject project)
		{
			DoBeforeCompileAction();
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			taskService.NotifyTaskChange();
			
			// cut&pasted from CombineEntry.cs
			stringParserService.Properties["Project"] = project.Name;
			IProjectService   projectService   = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IStatusBarService statusBarService = (IStatusBarService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IStatusBarService));
			IResourceService resourceService   = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			
			statusBarService.SetMessage("${res:MainWindow.StatusBar.CompilingMessage}");
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			
			// create output directory, if not exists
			string outputDir = ((AbstractProjectConfiguration)project.ActiveConfiguration).OutputDirectory;
			try {
				DirectoryInfo directoryInfo = new DirectoryInfo(outputDir);
				if (!directoryInfo.Exists) {
					directoryInfo.Create();
				}
			} catch (Exception e) {
				throw new ApplicationException("Can't create project output directory " + outputDir + " original exception:\n" + e.ToString());
			}
			
			AbstractProjectConfiguration conf = project.ActiveConfiguration as AbstractProjectConfiguration;
			taskService.CompilerOutput += stringParserService.Parse("${res:MainWindow.CompilerMessages.BuildStartedOutput}", new string[,] { {"PROJECT", project.Name}, {"CONFIG", project.ActiveConfiguration.Name} }) + "\n";
			taskService.CompilerOutput += resourceService.GetString("MainWindow.CompilerMessages.PerformingMainCompilationOutput") + "\n";
			
			if (conf != null && conf.ExecuteBeforeBuild != null && conf.ExecuteBeforeBuild.Length > 0) {
				string command   = conf.ExecuteBeforeBuild;
				string arguments = conf.ExecuteBeforeBuildArguments;
				
				if (File.Exists(command)) {
					taskService.CompilerOutput += stringParserService.Parse("${res:MainWindow.CompilerMessages.ExecuteScript}", new string[,] { {"SCRIPT", conf.ExecuteBeforeBuild} }) + "\n";
					ProcessStartInfo ps = new ProcessStartInfo(command, arguments);
					ps.UseShellExecute = false;
					ps.RedirectStandardOutput = true;
					ps.WorkingDirectory = Path.GetDirectoryName(command);
					Process process = new Process();
					process.StartInfo = ps;
					process.Start();
					taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
				}
			}
			
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			
			// cut&paste END
			return binding;
		}
		
		void AfterCompile(IProject project, ICompilerResult res)
		{
			// cut&pasted from CombineEntry.cs
			TaskService taskService = (TaskService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(TaskService));
			foreach (CompilerError err in res.CompilerResults.Errors) {
				taskService.Tasks.Add(new Task(project, err));
			}
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			AbstractProjectConfiguration conf = project.ActiveConfiguration as AbstractProjectConfiguration;
			
			if (conf != null && taskService.Errors == 0 && conf.ExecuteAfterBuild != null && conf.ExecuteAfterBuild.Length > 0) {
				taskService.CompilerOutput += stringParserService.Parse("${res:MainWindow.CompilerMessages.ExecuteScript}", new string[,] { {"SCRIPT", conf.ExecuteAfterBuild} }) + "\n";
				string command   = conf.ExecuteAfterBuild;
				string arguments = conf.ExecuteAfterBuildArguments;
				
				if (File.Exists(command)) {
					ProcessStartInfo ps = new ProcessStartInfo(command, arguments);
					ps.UseShellExecute = false;
					ps.RedirectStandardOutput = true;
					ps.WorkingDirectory = Path.GetDirectoryName(command);
					Process process = new Process();
					process.StartInfo = ps;
					process.Start();
					taskService.CompilerOutput += process.StandardOutput.ReadToEnd();
				}
			}
			taskService.NotifyTaskChange();
			taskService.CompilerOutput += res.CompilerOutput + stringParserService.Parse("${res:MainWindow.CompilerMessages.ProjectStatsOutput}", new string[,] { {"ERRORS", taskService.Errors.ToString()}, {"WARNINGS", taskService.Warnings.ToString()} }) + "\n\n";
		
		}
		
		public ICompilerResult RecompileProject(IProject project)
		{
			ICompilerResult res = BeforeCompile(project).RecompileProject(project);
			AfterCompile(project, res);
			return res;
		}
		
		public ICompilerResult CompileProject(IProject project)
		{
			ICompilerResult res = BeforeCompile(project).CompileProject(project);
			AfterCompile(project, res);
			return res;
		}
		
		void DoBeforeCompileAction()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			BeforeCompileAction action = (BeforeCompileAction)propertyService.GetProperty("SharpDevelop.Services.DefaultParserService.BeforeCompileAction", BeforeCompileAction.SaveAllFiles);
			
			switch (action) {
				case BeforeCompileAction.Nothing:
					break;
				case BeforeCompileAction.PromptForSave:
					bool save = false;
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.FileName != null && content.IsDirty) {
							if (!save) {
								IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
								if (messageService.AskQuestion("${res:MainWindow.SaveChangesMessage}")) {
									save = true;
								} else {
									break;
								}
							}
							MarkFileDirty(content.FileName);
							content.Save();
						}
					}
					break;
				case BeforeCompileAction.SaveAllFiles:
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						if (content.FileName != null && content.IsDirty) {
							MarkFileDirty(content.FileName);
							content.Save();
						}
					}
					break;
				default:
					System.Diagnostics.Debug.Assert(false);
					break;
			}
		}
		
		public ProjectFile RetrieveFileInformationForFile(string fileName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == fileName) {
						return fInfo;
					}
				}
			}
			return null;
		}
		
		public IProject RetrieveProjectForFile(ProjectFile file)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				if (projectEntry.Project.ProjectFiles.Contains(file)) {
					return projectEntry.Project;
				}
			}
			return null;
		}
		
		void RemoveFileFromAllProjects(string fileName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			restart:
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectReference rInfo in projectEntry.Project.ProjectReferences) {
					if (rInfo.ReferenceType == ReferenceType.Assembly && rInfo.Reference == fileName) {
						projectEntry.Project.ProjectReferences.Remove(rInfo);
						goto restart;
					}
				}
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == fileName) {
						projectEntry.Project.ProjectFiles.Remove(fInfo);
						goto restart;
					}
				}
			}
		}
		
		void RemoveAllInDirectory(string dirName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			restart:
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name.StartsWith(dirName)) {
						projectEntry.Project.ProjectFiles.Remove(fInfo);
						goto restart;
					}
				}
			}
		}
		
		void CheckFileRemove(object sender, FileEventArgs e)
		{
			if (openCombine != null) {
				if (e.IsDirectory) {
					RemoveAllInDirectory(e.FileName);
				} else {
					RemoveFileFromAllProjects(e.FileName);
				}
			}
		}
		
		void RenameFileInAllProjects(string oldName, string newName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name == oldName) {
						fInfo.Name = newName;
					}
				}
			}
		}

		void RenameDirectoryInAllProjects(string oldName, string newName)
		{
			ArrayList projects = Combine.GetAllProjects(openCombine);
			
			foreach (ProjectCombineEntry projectEntry in projects) {
				foreach (ProjectFile fInfo in projectEntry.Project.ProjectFiles) {
					if (fInfo.Name.StartsWith(oldName)) {
						fInfo.Name = newName + fInfo.Name.Substring(oldName.Length);
					}
				}
			}
		}

		void CheckFileRename(object sender, FileEventArgs e)
		{
			System.Diagnostics.Debug.Assert(e.SourceFile != e.TargetFile);
			if (openCombine != null) {
				if (e.IsDirectory) {
					RenameDirectoryInAllProjects(e.SourceFile, e.TargetFile);
				} else {
					RenameFileInAllProjects(e.SourceFile, e.TargetFile);
				}
			}
		}
		
		public override void InitializeService()
		{
			base.InitializeService();
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			
			FileRemovedFromProject += new FileEventHandler(CheckFileRemove);
			fileService.FileRemoved += new FileEventHandler(CheckFileRemove);
			fileService.FileRenamed += new FileEventHandler(CheckFileRename);
		}
		
		void RestoreCombinePreferences(Combine combine, string combinefilename)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "CombinePreferences";
			if (!Directory.Exists(directory)) {
				return;
			}
			
			string[] files = Directory.GetFiles(directory, combine.Name + "*.xml");
			
			if (files.Length > 0) {
				XmlDocument doc = new XmlDocument();
				doc.Load(files[0]);
				XmlElement root = doc.DocumentElement;
				string combinepath = Path.GetDirectoryName(combinefilename);
				if (root["Files"] != null) {
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();		
					foreach (XmlElement el in root["Files"].ChildNodes) 
					{
						if (el.Name == "CustomViewContent") {
							string className = el.Attributes["class"].InnerText;
							string assemblyName = el.Attributes["assembly"].InnerText;
							foreach (Assembly assembly in assemblies) {
								if (assembly.GetName().Name == assemblyName) {
									try {
										IViewContentMemento memento = assembly.CreateInstance(className) as IViewContentMemento;
										if (memento != null && el.ChildNodes.Count > 0) {
											IViewContent content = memento.SetViewContentMemento((IViewContentMemento)memento.FromXmlElement((XmlElement)el.ChildNodes[0]));
											if (content != null) {
												WorkbenchSingleton.Workbench.ShowView(content);
												DisplayBindingService displayBindingService = (DisplayBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(DisplayBindingService));
												displayBindingService.AttachSubWindows(content.WorkbenchWindow);
											}
										}
									}
									catch {}
									break;
								}
							}
						}
						else {
							string fileName = fileUtilityService.RelativeToAbsolutePath(combinepath, el.Attributes["filename"].InnerText);
							if (File.Exists(fileName)) {
								fileService.OpenFile(fileName);
							}
						}
					}
				}
				
				if (root["Views"] != null) {
					foreach (XmlElement el in root["Views"].ChildNodes) {
						foreach (IPadContent view in WorkbenchSingleton.Workbench.PadContentCollection) {
							if (el.Attributes["class"].InnerText == view.GetType().ToString() && view is IMementoCapable && el.ChildNodes.Count > 0) {
								IMementoCapable m = (IMementoCapable)view; 
								m.SetMemento((IXmlConvertable)m.CreateMemento().FromXmlElement((XmlElement)el.ChildNodes[0]));
							}
						}
					}
				}
				
				if (root["Properties"] != null) {
					IProperties properties = (IProperties)new DefaultProperties().FromXmlElement((XmlElement)root["Properties"].ChildNodes[0]);
					string name = properties.GetProperty("ActiveWindow", "");
					foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
						// WINDOWS DEPENDENCY : ToUpper
						if (content.FileName != null) {
							bool select = false;
							try {
								select = Path.GetFullPath(content.FileName).ToUpper() == Path.GetFullPath(name).ToUpper();
							} catch (Exception) {
								select = content.FileName == name;
							}
							if (select) {
								content.WorkbenchWindow.SelectWindow();
							}
							break;
						}
					}
				}
			} 
		}
		
		void SaveCombinePreferences(Combine combine, string combinefilename)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string directory = propertyService.ConfigDirectory + "CombinePreferences";
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			string combinepath = Path.GetDirectoryName(combinefilename);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<?xml version=\"1.0\"?>\n<UserCombinePreferences/>");
			
			XmlAttribute fileNameAttribute = doc.CreateAttribute("filename");
			fileNameAttribute.InnerText = combinefilename;
			doc.DocumentElement.Attributes.Append(fileNameAttribute);
			
			XmlElement filesnode = doc.CreateElement("Files");
			doc.DocumentElement.AppendChild(filesnode);
			
			foreach (IViewContent content in WorkbenchSingleton.Workbench.ViewContentCollection) {
				if (content is IViewContentMementoCreator) {
					XmlElement el = doc.CreateElement("CustomViewContent");
					IViewContentMemento memento = ((IViewContentMementoCreator)content).CreateViewContentMemento();
					XmlAttribute ass = doc.CreateAttribute("assembly");
					ass.InnerText = memento.GetType().Assembly.GetName().Name;
					el.Attributes.Append(ass);
					XmlAttribute attr = doc.CreateAttribute("class");
					attr.InnerText = memento.GetType().ToString();
					el.Attributes.Append(attr);
					el.AppendChild(memento.ToXmlElement(doc));
					filesnode.AppendChild(el);
				}
				else if (content.FileName != null) {
					XmlElement el = doc.CreateElement("File");
					
					XmlAttribute attr = doc.CreateAttribute("filename");
					attr.InnerText = fileUtilityService.AbsoluteToRelativePath(combinepath, content.FileName);
					el.Attributes.Append(attr);
					
					filesnode.AppendChild(el);
				}
			}
			
			XmlElement viewsnode = doc.CreateElement("Views");
			doc.DocumentElement.AppendChild(viewsnode);
			
			foreach (IPadContent view in WorkbenchSingleton.Workbench.PadContentCollection) {
				if (view is IMementoCapable) {
					XmlElement el = doc.CreateElement("ViewMemento");
					
					XmlAttribute attr = doc.CreateAttribute("class");
					attr.InnerText = view.GetType().ToString();
					el.Attributes.Append(attr);
					
					el.AppendChild(((IMementoCapable)view).CreateMemento().ToXmlElement(doc));
					
					viewsnode.AppendChild(el);
				}
			}
			
			IProperties properties = new DefaultProperties();
			string name = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null ? String.Empty : WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
			properties.SetProperty("ActiveWindow", name == null ? String.Empty : name);
			
			XmlElement propertynode = doc.CreateElement("Properties");
			doc.DocumentElement.AppendChild(propertynode);
			
			propertynode.AppendChild(properties.ToXmlElement(doc));
			
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), directory + Path.DirectorySeparatorChar + combine.Name + ".xml", FileErrorPolicy.ProvideAlternative);
		}
		
		public IProject GetProject(string projectName)
		{
			if (CurrentOpenCombine == null) {
				return null;
			}
			ArrayList list = Combine.GetAllProjects(this.CurrentOpenCombine);
			foreach (ProjectCombineEntry projectEntry in list) {
				if (projectEntry.Project.Name == projectName) {
					return projectEntry.Project;
				}
			}
			return null;
		}
		
		//********* own events
		protected virtual void OnCombineOpened(CombineEventArgs e)
		{
			if (CombineOpened != null) {
				CombineOpened(this, e);
			}
		}
		
		protected virtual void OnCombineClosed(CombineEventArgs e)
		{
			if (CombineClosed != null) {
				CombineClosed(this, e);
			}
		}
		
		protected virtual void OnCurrentSelectedCombineChanged(CombineEventArgs e)
		{
			if (CurrentSelectedCombineChanged != null) {
				CurrentSelectedCombineChanged(this, e);
			}
		}
		
		protected virtual void OnCurrentProjectChanged(ProjectEventArgs e)
		{
			if (CurrentSelectedProject != null) {
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				stringParserService.Properties["PROJECTNAME"] = CurrentSelectedProject.Name;
			}
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(this, e);
			}
		}
		
		public virtual void OnConfigurationAdded(EventArgs e)
		{
			if(ConfigurationAdded != null) {
				ConfigurationAdded(this, e);
			}
		}
		
		public virtual void OnConfigurationRemoved(EventArgs e)
		{
			if(ConfigurationRemoved != null) {
				ConfigurationRemoved(this, e);
			}
		}
		
		public virtual void OnActiveConfigurationChanged(ConfigurationEventArgs e)
		{
			if(ActiveConfigurationChanged != null) {
				ActiveConfigurationChanged(this, e);
			}
		}
		
		public virtual void OnRenameProject(ProjectRenameEventArgs e)
		{
			if (ProjectRenamed != null) {
				ProjectRenamed(this, e);
			}
		}
		
		public bool ExistsEntryWithName(string name)
		{
			ArrayList allProjects = Combine.GetAllProjects(openCombine);
			foreach (ProjectCombineEntry projectEntry in allProjects) {
				if (projectEntry.Project.Name == name) {
					return true;
				}
			}
			return false;
		}
		
		public string GetOutputAssemblyName(IProject project)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			ILanguageBinding binding = languageBindingService.GetBindingPerLanguageName(project.ProjectType);
			return binding.GetCompiledOutputName(project);
		}
		
		public string GetOutputAssemblyName(string projectName)
		{
			ArrayList allProjects = Combine.GetAllProjects(CurrentOpenCombine);
			foreach (ProjectCombineEntry projectEntry in allProjects) {
				if (projectEntry.Project.Name == projectName) {
					return GetOutputAssemblyName(projectEntry.Project);
				}
			}
			return null;
		}
		
		public void RemoveFileFromProject(string fileName)
		{
			if (Directory.Exists(fileName)) {
				OnFileRemovedFromProject(new FileEventArgs(fileName, true));
			} else {
				OnFileRemovedFromProject(new FileEventArgs(fileName, false));
			}
		}
			
		public void OnStartBuild()
		{
			if (StartBuild != null) {
				StartBuild(this, null);
			}
		}
		
		public void OnEndBuild()
		{
			if (EndBuild != null) {
				EndBuild(this, null);
			}
		}
		public void OnBeforeStartProject()
		{
			if (BeforeStartProject != null) {
				BeforeStartProject(this, null);
			}
		}
		
		protected virtual void OnFileRemovedFromProject(FileEventArgs e)
		{
			if (FileRemovedFromProject != null) {
				FileRemovedFromProject(this, e);
			}
		}
		
		public string GetFileName(IProject project)
		{
			if (openCombine != null) {
				ArrayList projects = Combine.GetAllProjects(openCombine);
				foreach (ProjectCombineEntry projectCombineEntry in projects) {
					if (projectCombineEntry.Project == project) {
						return projectCombineEntry.Filename;
					}
				}
			}
			return String.Empty;
		}
		
		public string GetFileName(Combine combine)
		{
			if (combine == openCombine) {
				return openCombineFileName;
			}
			Stack combines = new Stack();
			combines.Push(openCombine);
			while (combines.Count > 0) {
				Combine curCombine = (Combine)combines.Pop();
				foreach (CombineEntry entry in curCombine.Entries) {
					CombineCombineEntry combineEntry = (CombineCombineEntry)entry;
					if (combineEntry != null) {
						if (combineEntry.Combine == combine) {
							return entry.Filename;
						}
						combines.Push(combineEntry.Combine);
					}
				}
			}
			
			return String.Empty;
		}
		
		public event FileEventHandler FileRemovedFromProject;
		public event EventHandler     StartBuild;
		public event EventHandler     EndBuild;
		public event EventHandler     BeforeStartProject;
		
		
		public event CombineEventHandler CombineOpened;
		public event CombineEventHandler CombineClosed;
		public event CombineEventHandler CurrentSelectedCombineChanged;
		
		public event ProjectRenameEventHandler ProjectRenamed;
		public event ProjectEventHandler       CurrentProjectChanged;
		public event ConfigurationEventHandler ActiveConfigurationChanged;
		public event EventHandler ConfigurationAdded;
		public event EventHandler ConfigurationRemoved;
	}
}
