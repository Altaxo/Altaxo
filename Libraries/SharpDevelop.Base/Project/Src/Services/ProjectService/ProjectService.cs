﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2140 $</version>
// </file>

using System;
using System.IO;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public static class ProjectService
	{
		static Solution openSolution;
		static IProject currentProject;
		
		public static Solution OpenSolution {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return openSolution;
			}
		}
		
		public static IProject CurrentProject {
			[System.Diagnostics.DebuggerStepThrough]
			get {
				return currentProject;
			}
			set {
				if (currentProject != value) {
					LoggingService.Info("CurrentProject changed to " + (value == null ? "null" : value.Name));
					currentProject = value;
					OnCurrentProjectChanged(new ProjectEventArgs(currentProject));
				}
			}
		}
		
		/// <summary>
		/// Gets an open project by the name of the project file.
		/// </summary>
		public static IProject GetProject(string projectFilename)
		{
			if (openSolution == null) return null;
			foreach (IProject project in openSolution.Projects) {
				if (FileUtility.IsEqualFileName(project.FileName, projectFilename)) {
					return project;
				}
			}
			return null;
		}
		
		static bool initialized;
		
		public static void InitializeService()
		{
			if (!initialized) {
				initialized = true;
				WorkbenchSingleton.Workbench.ActiveWorkbenchWindowChanged += ActiveWindowChanged;
				FileService.FileRenamed += FileServiceFileRenamed;
				FileService.FileRemoved += FileServiceFileRemoved;
			}
		}

		/// <summary>
		/// Returns if a project loader exists for the given file. This method works even in early
		/// startup (before service initialization)
		/// </summary>
		public static bool HasProjectLoader(string fileName)
		{
			AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			foreach (Codon codon in addinTreeNode.Codons) {
				string pattern = codon.Properties.Get("extensions", "");
				if (FileUtility.MatchesPattern(fileName, pattern) && codon.Properties.Contains("class")) {
					return true;
				}
			}
			return false;
		}
		
		public static IProjectLoader GetProjectLoader(string fileName)
		{
			AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			foreach (Codon codon in addinTreeNode.Codons) {
				string pattern = codon.Properties.Get("extensions", "");
				if (FileUtility.MatchesPattern(fileName, pattern) && codon.Properties.Contains("class")) {
					object binding = codon.AddIn.CreateObject(codon.Properties["class"]);
					return binding as IProjectLoader;
				}
			}
			return null;
		}
		
		public static void LoadSolutionOrProject(string fileName)
		{
			IProjectLoader loader = GetProjectLoader(fileName);
			if (loader != null)	{
				loader.Load(fileName);
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
			}
		}
		
		static void FileServiceFileRenamed(object sender, FileRenameEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string oldName = e.SourceFile;
			string newName = e.TargetFile;
			long x = 0;
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				foreach (SolutionItem item in container.SolutionItems.Items) {
					string oldFullName  = Path.Combine(OpenSolution.Directory, item.Name);
					++x;
					if (FileUtility.IsBaseDirectory(oldName, oldFullName)) {
						string newFullName = FileUtility.RenameBaseDirectory(oldFullName, oldName, newName);
						item.Name = item.Location = FileUtility.GetRelativePath(OpenSolution.Directory, newFullName);
					}
				}
			}
			
			long y = 0;
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, oldName)) {
					foreach (ProjectItem item in project.Items) {
						++y;
						if (FileUtility.IsBaseDirectory(oldName, item.FileName)) {
							OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							item.FileName = FileUtility.RenameBaseDirectory(item.FileName, oldName, newName);
							OnProjectItemAdded(new ProjectItemEventArgs(project, item));
						}
					}
				}
			}
		}
		
		static void FileServiceFileRemoved(object sender, FileEventArgs e)
		{
			if (OpenSolution == null) {
				return;
			}
			string fileName = e.FileName;
			
			foreach (ISolutionFolderContainer container in OpenSolution.SolutionFolderContainers) {
				for (int i = 0; i < container.SolutionItems.Items.Count;) {
					SolutionItem item = container.SolutionItems.Items[i];
					if (FileUtility.IsBaseDirectory(fileName, Path.Combine(OpenSolution.Directory, item.Name))) {
						container.SolutionItems.Items.RemoveAt(i);
					} else {
						++i;
					}
				}
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				if (FileUtility.IsBaseDirectory(project.Directory, fileName)) {
					IProjectItemListProvider provider = project as IProjectItemListProvider;
					if (provider != null) {
						foreach (ProjectItem item in Linq.ToArray(provider.Items)) {
							if (FileUtility.IsBaseDirectory(fileName, item.FileName)) {
								provider.RemoveProjectItem(item);
								OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
							}
						}
					}
				}
			}
		}
		
		static void ActiveWindowChanged(object sender, EventArgs e)
		{
			object activeContent = WorkbenchSingleton.Workbench.ActiveContent;
			IViewContent viewContent = activeContent as IViewContent;
			if (viewContent == null && activeContent is ISecondaryViewContent) {
				// required if one creates a new winforms app and then immediately switches to design mode
				// without focussing the text editor
				IWorkbenchWindow window = ((ISecondaryViewContent)activeContent).WorkbenchWindow;
				if (window == null) // workbench window is being disposed
					return;
				viewContent = window.ViewContent;
			}
			if (OpenSolution == null || viewContent == null) {
				return;
			}
			string fileName = viewContent.FileName;
			if (fileName == null) {
				return;
			}
			CurrentProject = OpenSolution.FindProjectContainingFile(fileName) ?? CurrentProject;
		}
		
		public static void AddProject(ISolutionFolderNode solutionFolderNode, IProject newProject)
		{
			solutionFolderNode.Container.AddFolder(newProject);
			ParserService.CreateProjectContentForAddedProject(newProject);
			solutionFolderNode.Solution.FixSolutionConfiguration(new IProject[] { newProject });
			OnProjectAdded(new ProjectEventArgs(newProject));
		}
		
		/// <summary>
		/// Adds a project item to the project, raising the ProjectItemAdded event.
		/// Make sure you call project.Save() after adding new items!
		/// </summary>
		public static void AddProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				provider.AddProjectItem(item);
				OnProjectItemAdded(new ProjectItemEventArgs(project, item));
			}
		}
		
		/// <summary>
		/// Removes a project item from the project, raising the ProjectItemRemoved event.
		/// Make sure you call project.Save() after removing items!
		/// No action (not even raising the event) is taken when the item was already removed form the project.
		/// </summary>
		public static void RemoveProjectItem(IProject project, ProjectItem item)
		{
			if (project == null) throw new ArgumentNullException("project");
			if (item == null)    throw new ArgumentNullException("item");
			IProjectItemListProvider provider = project as IProjectItemListProvider;
			if (provider != null) {
				if (provider.RemoveProjectItem(item)) {
					OnProjectItemRemoved(new ProjectItemEventArgs(project, item));
				}
			}
		}
		
		static void BeforeLoadSolution()
		{
			if (openSolution != null) {
				SaveSolutionPreferences();
				WorkbenchSingleton.Workbench.CloseAllViews();
				CloseSolution();
			}
		}
		
		public static void LoadSolution(string fileName)
		{
			BeforeLoadSolution();
			try {
				openSolution = Solution.Load(fileName);
				if (openSolution == null)
					return;
			} catch (UnauthorizedAccessException ex) {
				MessageService.ShowError(ex.Message);
				return;
			}
			AbstractProject.filesToOpenAfterSolutionLoad.Clear();
			try {
				string file = GetPreferenceFileName(openSolution.FileName);
				if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
					(openSolution.Preferences as IMementoCapable).SetMemento(Properties.Load(file));
				} else {
					(openSolution.Preferences as IMementoCapable).SetMemento(new Properties());
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			try {
				ApplyConfigurationAndReadPreferences();
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
			// Create project contents for solution
			ParserService.OnSolutionLoaded();
			
			// preferences must be read before OnSolutionLoad is called to enable
			// the event listeners to read e.Solution.Preferences.Properties
			OnSolutionLoaded(new SolutionEventArgs(openSolution));
		}
		
		internal static void ParserServiceCreatedProjectContents()
		{
			foreach (string file in AbstractProject.filesToOpenAfterSolutionLoad) {
				if (File.Exists(file)) {
					FileService.OpenFile(file);
				}
			}
			AbstractProject.filesToOpenAfterSolutionLoad.Clear();
		}
		
		static void ApplyConfigurationAndReadPreferences()
		{
			openSolution.ApplySolutionConfigurationAndPlatformToProjects();
			foreach (IProject project in openSolution.Projects) {
				string file = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidFileName(file) && File.Exists(file)) {
					project.SetMemento(Properties.Load(file));
				}
			}
		}
		
		/// <summary>
		/// Load a single project as solution.
		/// </summary>
		public static void LoadProject(string fileName)
		{
			string solutionFile = Path.ChangeExtension(fileName, ".sln");
			if (File.Exists(solutionFile)) {
				LoadSolution(solutionFile);
				
				if (openSolution != null) {
					bool found = false;
					foreach (IProject p in openSolution.Projects) {
						if (FileUtility.IsEqualFileName(fileName, p.FileName)) {
							found = true;
							break;
						}
					}
					if (found == false) {
						string[,] parseArgs = {{"SolutionName", Path.GetFileName(solutionFile)}, {"ProjectName", Path.GetFileName(fileName)}};
						int res = MessageService.ShowCustomDialog(MessageService.ProductName,
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject}", parseArgs),
						                                          0, 2,
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.AddProjectToSolution}", parseArgs),
						                                          StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.SolutionDoesNotContainProject.CreateNewSolution}", parseArgs),
						                                          "${res:Global.IgnoreButtonText}");
						if (res == 0) {
							// Add project to solution
							Commands.AddExitingProjectToSolution.AddProject((ISolutionFolderNode)ProjectBrowserPad.Instance.SolutionNode, fileName);
							SaveSolution();
							return;
						} else if (res == 1) {
							CloseSolution();
							try {
								File.Copy(solutionFile, Path.ChangeExtension(solutionFile, ".old.sln"), true);
							} catch (IOException){}
						} else {
							// ignore, just open the solution
							return;
						}
					} else {
						// opened solution instead and correctly found the project
						return;
					}
				} else {
					// some problem during opening, abort
					return;
				}
			}
			Solution solution = new Solution();
			solution.Name = Path.GetFileNameWithoutExtension(fileName);
			ILanguageBinding binding = LanguageBindingService.GetBindingPerProjectFile(fileName);
			IProject project;
			if (binding != null) {
				project = LanguageBindingService.LoadProject(solution, fileName, solution.Name);
				if (project is UnknownProject) {
					if (((UnknownProject)project).WarningDisplayedToUser == false) {
						((UnknownProject)project).ShowWarningMessageBox();
					}
					return;
				}
			} else {
				MessageService.ShowError(StringParser.Parse("${res:ICSharpCode.SharpDevelop.Commands.OpenCombine.InvalidProjectOrCombine}", new string[,] {{"FileName", fileName}}));
				return;
			}
			solution.AddFolder(project);
			ProjectSection configSection = solution.GetSolutionConfigurationsSection();
			foreach (string configuration in project.ConfigurationNames) {
				foreach (string platform in project.PlatformNames) {
					string key;
					if (platform == "AnyCPU") { // Fix for SD2-786
						key = configuration + "|Any CPU";
					} else {
						key = configuration + "|" + platform;
					}
					configSection.Items.Add(new SolutionItem(key, key));
				}
			}
			solution.FixSolutionConfiguration(new IProject[] { project });
			
			if (FileUtility.ObservedSave((NamedFileOperationDelegate)solution.Save, solutionFile) == FileOperationResult.OK) {
				// only load when saved succesfully
				LoadSolution(solutionFile);
			}
		}
		
		public static void SaveSolution()
		{
			if (openSolution != null) {
				openSolution.Save();
				foreach (IProject project in openSolution.Projects) {
					project.Save();
				}
				OnSolutionSaved(new SolutionEventArgs(openSolution));
			}
		}
		
		/// <summary>
		/// Returns a File Dialog filter that can be used to filter on all registered project formats
		/// </summary>
		public static string GetAllProjectsFilter(object caller)
		{
			AddInTreeNode addinTreeNode = AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter");
			StringBuilder b = new StringBuilder(StringParser.Parse("${res:SharpDevelop.Solution.AllKnownProjectFormats}|"));
			bool first = true;
			foreach (Codon c in addinTreeNode.Codons) {
				string ext = c.Properties.Get("extensions", "");
				if (ext != "*.*" && ext.Length > 0) {
					if (!first) {
						b.Append(';');
					} else {
						first = false;
					}
					b.Append(ext);
				}
			}
			foreach (string entry in addinTreeNode.BuildChildItems(caller)) {
				b.Append('|');
				b.Append(entry);
			}
			return b.ToString();
		}
		
		static string GetPreferenceFileName(string projectFileName)
		{
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			return Path.Combine(directory,
			                    Path.GetFileName(projectFileName)
			                    + "." + projectFileName.ToLowerInvariant().GetHashCode().ToString("x")
			                    + ".xml");
		}
		
		public static void SaveSolutionPreferences()
		{
			if (openSolution == null)
				return;
			string directory = Path.Combine(PropertyService.ConfigDirectory, "preferences");
			if (!Directory.Exists(directory)) {
				Directory.CreateDirectory(directory);
			}
			
			if (SolutionPreferencesSaving != null)
				SolutionPreferencesSaving(null, new SolutionEventArgs(openSolution));
			Properties memento = (openSolution.Preferences as IMementoCapable).CreateMemento();
			
			string fullFileName = GetPreferenceFileName(openSolution.FileName);
			if (FileUtility.IsValidFileName(fullFileName)) {
				FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
			}
			
			foreach (IProject project in OpenSolution.Projects) {
				memento = project.CreateMemento();
				if (memento == null) continue;
				
				fullFileName = GetPreferenceFileName(project.FileName);
				if (FileUtility.IsValidFileName(fullFileName)) {
					FileUtility.ObservedSave(new NamedFileOperationDelegate(memento.Save), fullFileName, FileErrorPolicy.Inform);
				}
			}
		}
		
		public static void CloseSolution()
		{
			if (openSolution != null) {
				CurrentProject = null;
				OnSolutionClosing(new SolutionEventArgs(openSolution));
				
				openSolution.Dispose();
				openSolution = null;
				
				OnSolutionClosed(EventArgs.Empty);
			}
		}
		
		public static void MarkFileDirty(string fileName)
		{
			if (OpenSolution != null) {
				foreach (IProject project in OpenSolution.Projects) {
					if (project.IsFileInProject(fileName)) {
						MarkProjectDirty(project);
					}
				}
			}
		}
		
		public static void MarkProjectDirty(IProject project)
		{
			project.IsDirty = true;
		}
		
		static void OnCurrentProjectChanged(ProjectEventArgs e)
		{
			if (CurrentProjectChanged != null) {
				CurrentProjectChanged(null, e);
			}
		}
		
		static void OnSolutionClosed(EventArgs e)
		{
			if (SolutionClosed != null) {
				SolutionClosed(null, e);
			}
		}
		
		static void OnSolutionClosing(SolutionEventArgs e)
		{
			if (SolutionClosing != null) {
				SolutionClosing(null, e);
			}
		}
		
		static void OnSolutionLoaded(SolutionEventArgs e)
		{
			if (SolutionLoaded != null) {
				SolutionLoaded(null, e);
			}
		}
		
		static void OnSolutionSaved(SolutionEventArgs e)
		{
			if (SolutionSaved != null) {
				SolutionSaved(null, e);
			}
		}
		
		static void OnProjectConfigurationChanged(ProjectConfigurationEventArgs e)
		{
			if (ProjectConfigurationChanged != null) {
				ProjectConfigurationChanged(null, e);
			}
		}
		
		static void OnSolutionConfigurationChanged(SolutionConfigurationEventArgs e)
		{
			if (SolutionConfigurationChanged != null) {
				SolutionConfigurationChanged(null, e);
			}
		}
		
		public static void RaiseEventStartBuild()
		{
			if (StartBuild != null) {
				StartBuild(null, EventArgs.Empty);
			}
		}
		
		public static void RaiseEventEndBuild()
		{
			if (EndBuild != null) {
				EndBuild(null, EventArgs.Empty);
			}
		}
		
		public static void RemoveSolutionFolder(string guid)
		{
			if (OpenSolution == null) {
				return;
			}
			foreach (ISolutionFolder folder in OpenSolution.SolutionFolders) {
				if (folder.IdGuid == guid) {
					folder.Parent.RemoveFolder(folder);
					OnSolutionFolderRemoved(new SolutionFolderEventArgs(folder));
					break;
				}
			}
		}
		
		static void OnSolutionFolderRemoved(SolutionFolderEventArgs e)
		{
			if (SolutionFolderRemoved != null) {
				SolutionFolderRemoved(null, e);
			}
		}
		
		static void OnProjectItemAdded(ProjectItemEventArgs e)
		{
			if (ProjectItemAdded != null) {
				ProjectItemAdded(null, e);
			}
		}
		static void OnProjectItemRemoved(ProjectItemEventArgs e)
		{
			if (ProjectItemRemoved != null) {
				ProjectItemRemoved(null, e);
			}
		}
		static void OnProjectAdded(ProjectEventArgs e)
		{
			if (ProjectAdded != null) {
				ProjectAdded(null, e);
			}
		}
		
		public static event ProjectEventHandler ProjectAdded;
		public static event SolutionFolderEventHandler SolutionFolderRemoved;
		
		public static event EventHandler StartBuild;
		public static event EventHandler EndBuild;
		
		public static event ProjectConfigurationEventHandler ProjectConfigurationChanged;
		public static event SolutionConfigurationEventHandler SolutionConfigurationChanged;
		
		public static event EventHandler<SolutionEventArgs> SolutionLoaded;
		public static event EventHandler<SolutionEventArgs> SolutionSaved;
		
		public static event EventHandler<SolutionEventArgs> SolutionClosing;
		public static event EventHandler                    SolutionClosed;
		
		/// <summary>
		/// Raised before the solution preferences are being saved. Allows you to save
		/// your additional properties in the solution preferences.
		/// </summary>
		public static event EventHandler<SolutionEventArgs> SolutionPreferencesSaving;
		
		public static event ProjectEventHandler CurrentProjectChanged;
		
		public static event EventHandler<ProjectItemEventArgs> ProjectItemAdded;
		public static event EventHandler<ProjectItemEventArgs> ProjectItemRemoved;
	}
}
