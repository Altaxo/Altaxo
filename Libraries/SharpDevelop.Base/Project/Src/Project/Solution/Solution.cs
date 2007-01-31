﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2146 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using ICSharpCode.Core;
using MSBuild = Microsoft.Build.BuildEngine;

namespace ICSharpCode.SharpDevelop.Project
{
	public interface IMSBuildEngineProvider
	{
		MSBuild.Engine BuildEngine {
			get;
		}
	}
	
	public class Solution : SolutionFolder, IDisposable, IMSBuildEngineProvider
	{
		// contains <guid>, (IProject/ISolutionFolder) pairs.
		Dictionary<string, ISolutionFolder> guidDictionary = new Dictionary<string, ISolutionFolder>();
		
		string fileName = String.Empty;
		
		MSBuild.Engine buildEngine = MSBuildInternals.CreateEngine();
		
		public Solution()
		{
			preferences = new SolutionPreferences(this);
		}
		
		public MSBuild.Engine BuildEngine {
			get { return buildEngine; }
		}
		
		#region Enumerate projects/folders
		public IProject FindProjectContainingFile(string fileName)
		{
			IProject currentProject = ProjectService.CurrentProject;
			if (currentProject != null && currentProject.IsFileInProject(fileName))
				return currentProject;
			
			// Try all project's in the solution.
			foreach (IProject project in Projects) {
				if (project.IsFileInProject(fileName)) {
					return project;
				}
			}
			return null;
		}
		
		[Browsable(false)]
		public IEnumerable<IProject> Projects {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is IProject) {
						yield return ((IProject)currentFolder);
					}
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		
		[Browsable(false)]
		public IEnumerable<ISolutionFolderContainer> SolutionFolderContainers {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						yield return currentContainer;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}

		[Browsable(false)]
		public IEnumerable<ISolutionFolder> SolutionFolders {
			get {
				Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>();
				
				foreach (ISolutionFolder solutionFolder in Folders) {
					stack.Push(solutionFolder);
				}
				
				while (stack.Count > 0) {
					ISolutionFolder currentFolder = stack.Pop();
					
					yield return currentFolder;
					
					if (currentFolder is ISolutionFolderContainer) {
						ISolutionFolderContainer currentContainer = (ISolutionFolderContainer)currentFolder;
						foreach (ISolutionFolder subFolder in currentContainer.Folders) {
							stack.Push(subFolder);
						}
					}
				}
			}
		}
		
		/// <summary>
		/// Returns the startup project. If no startup project is set in the solution preferences,
		/// returns any project that is startable.
		/// </summary>
		[Browsable(false)]
		public IProject StartupProject {
			get {
				if (!HasProjects) {
					return null;
				}
				IProject startupProject = preferences.StartupProject;
				if (startupProject != null)
					return startupProject;
				foreach (IProject project in Projects) {
					if (project.IsStartable) {
						return project;
					}
				}
				return null;
			}
		}
		
		public ISolutionFolder GetSolutionFolder(string guid)
		{
			foreach (ISolutionFolder solutionFolder in SolutionFolders) {
				if (solutionFolder.IdGuid == guid) {
					return solutionFolder;
				}
			}
			return null;
		}
		
		public SolutionFolder CreateFolder(string folderName)
		{
			return new SolutionFolder(folderName, folderName, "{" + Guid.NewGuid().ToString().ToUpperInvariant() + "}");
		}
		#endregion
		
		#region Properties
		[Browsable(false)]
		public bool HasProjects {
			get {
				return Projects.GetEnumerator().MoveNext();
			}
		}
		
		[Browsable(false)]
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		[Browsable(false)]
		public string Directory {
			get {
				return Path.GetDirectoryName(fileName);
			}
		}
		
		[Browsable(false)]
		public bool IsDirty {
			get {
				foreach (IProject project in Projects) {
					if (project.IsDirty) {
						return true;
					}
				}
				return false;
			}
		}
		
		SolutionPreferences preferences;
		
		[Browsable(false)]
		public SolutionPreferences Preferences {
			get {
				return preferences;
			}
		}
		#endregion
		
		#region ISolutionFolderContainer implementations
		[Browsable(false)]
		public override Solution ParentSolution {
			get { return this; }
		}
		
		public override ProjectSection SolutionItems {
			get {
				foreach (SolutionFolder folder in Folders) {
					if (folder.Name == "Solution Items") {
						return folder.SolutionItems;
					}
				}
				
				SolutionFolder newFolder = CreateFolder("Solution Items");
				return newFolder.SolutionItems;
			}
		}
		
		public override void AddFolder(ISolutionFolder folder)
		{
			base.AddFolder(folder);
			guidDictionary[folder.IdGuid] = folder;
		}
		
		#endregion
		
		#region Save
		public void Save()
		{
			try {
				Save(fileName);
			} catch (IOException ex) {
				MessageService.ShowError("Could not save " + fileName + ":\n" + ex.Message);
			} catch (UnauthorizedAccessException ex) {
				MessageService.ShowError("Could not save " + fileName + ":\n" + ex.Message + "\n\nEnsure the file is writable.");
			}
		}
		
		public void Save(string fileName)
		{
			this.fileName = fileName;
			string outputDirectory = Path.GetDirectoryName(fileName);
			if (!System.IO.Directory.Exists(outputDirectory)) {
				System.IO.Directory.CreateDirectory(outputDirectory);
			}
			
			StringBuilder projectSection        = new StringBuilder();
			StringBuilder nestedProjectsSection = new StringBuilder();
			
			List<ISolutionFolder> folderList = Folders;
			Stack<ISolutionFolder> stack = new Stack<ISolutionFolder>(folderList.Count);
			// push folders in reverse order because it's a stack
			for (int i = folderList.Count - 1; i >= 0; i--) {
				stack.Push(folderList[i]);
			}
			
			while (stack.Count > 0) {
				ISolutionFolder currentFolder = stack.Pop();
				
				projectSection.Append("Project(\"");
				projectSection.Append(currentFolder.TypeGuid);
				projectSection.Append("\")");
				projectSection.Append(" = ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.Name);
				projectSection.Append("\", ");
				string relativeLocation;
				if (currentFolder is IProject) {
					currentFolder.Location = ((IProject)currentFolder).FileName;
				}
				if (Path.IsPathRooted(currentFolder.Location)) {
					relativeLocation = FileUtility.GetRelativePath(Path.GetDirectoryName(FileName), currentFolder.Location);
				} else {
					relativeLocation = currentFolder.Location;
				}
				projectSection.Append('"');
				projectSection.Append(relativeLocation);
				projectSection.Append("\", ");
				projectSection.Append('"');
				projectSection.Append(currentFolder.IdGuid);
				projectSection.Append("\"");
				projectSection.AppendLine();
				
				if (currentFolder is IProject) {
					IProject project = (IProject)currentFolder;
					// Web projects can have sections
					SaveProjectSections(project.ProjectSections, projectSection);
					
				} else if (currentFolder is SolutionFolder) {
					SolutionFolder folder = (SolutionFolder)currentFolder;
					
					SaveProjectSections(folder.Sections, projectSection);
					
					foreach (ISolutionFolder subFolder in folder.Folders) {
						stack.Push(subFolder);
						nestedProjectsSection.Append("\t\t");
						nestedProjectsSection.Append(subFolder.IdGuid);
						nestedProjectsSection.Append(" = ");
						nestedProjectsSection.Append(folder.IdGuid);
						nestedProjectsSection.Append(Environment.NewLine);
					}
				} else {
					LoggingService.Warn("Solution.Load(): unknown folder : " + currentFolder);
				}
				projectSection.Append("EndProject");
				projectSection.Append(Environment.NewLine);
			}
			
			StringBuilder globalSection = new StringBuilder();
			globalSection.Append("Global");
			globalSection.Append(Environment.NewLine);
			foreach (ProjectSection section in Sections) {
				globalSection.Append("\tGlobalSection(");
				globalSection.Append(section.Name);
				globalSection.Append(") = ");
				globalSection.Append(section.SectionType);
				globalSection.Append(Environment.NewLine);
				
				section.AppendSection(globalSection, "\t\t");
				
				globalSection.Append("\tEndGlobalSection");
				globalSection.Append(Environment.NewLine);
			}
			
			// we need to specify UTF8 because MSBuild needs the BOM
			using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8)) {
				sw.WriteLine();
				sw.WriteLine("Microsoft Visual Studio Solution File, Format Version 9.00");
				sw.WriteLine("# Visual Studio 2005");
				sw.WriteLine("# SharpDevelop " + RevisionClass.FullVersion);
				sw.Write(projectSection.ToString());
				
				sw.Write(globalSection.ToString());
				
				if (nestedProjectsSection.Length > 0) {
					sw.WriteLine("\tGlobalSection(NestedProjects) = preSolution");
					sw.Write(nestedProjectsSection.ToString());
					sw.WriteLine("\tEndGlobalSection");
				}
				
				sw.WriteLine("EndGlobal");
			}
		}
		
		static void SaveProjectSections(IEnumerable<ProjectSection> sections, StringBuilder projectSection)
		{
			foreach (ProjectSection section in sections) {
				projectSection.Append("\tProjectSection(");
				projectSection.Append(section.Name);
				projectSection.Append(") = ");
				projectSection.Append(section.SectionType);
				projectSection.Append(Environment.NewLine);
				
				section.AppendSection(projectSection, "\t\t");
				
				projectSection.Append("\tEndProjectSection");
				projectSection.Append(Environment.NewLine);
			}
		}
		#endregion
		
		#region Read/SetupSolution
		static Regex versionPattern       = new Regex("Microsoft Visual Studio Solution File, Format Version\\s+(?<Version>.*)", RegexOptions.Compiled);
		
		static Regex projectLinePattern   = new Regex("Project\\(\"(?<ProjectGuid>.*)\"\\)\\s+=\\s+\"(?<Title>.*)\",\\s*\"(?<Location>.*)\",\\s*\"(?<Guid>.*)\"", RegexOptions.Compiled);
		static Regex globalSectionPattern = new Regex("\\s*GlobalSection\\((?<Name>.*)\\)\\s*=\\s*(?<Type>.*)", RegexOptions.Compiled);
		
		static string GetFirstNonCommentLine(TextReader sr)
		{
			string line = "";
			while ((line = sr.ReadLine()) != null) {
				line = line.Trim();
				if (line.Length > 0 && line[0] != '#')
					return line;
			}
			return "";
		}
		
		/// <summary>
		/// Reads the specified solution file. The project-location-guid information is written into the conversion class.
		/// </summary>
		/// <returns>The version number of the solution.</returns>
		public static string ReadSolutionInformation(string solutionFileName, Converter.PrjxToSolutionProject.Conversion conversion)
		{
			LoggingService.Debug("ReadSolutionInformation: " + solutionFileName);
			string solutionDirectory = Path.GetDirectoryName(solutionFileName);
			using (StreamReader sr = File.OpenText(solutionFileName)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					return null;
				}
				string version = match.Result("${Version}");
				while ((line = sr.ReadLine()) != null) {
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = Path.Combine(solutionDirectory, match.Result("${Location}"));
						string guid         = match.Result("${Guid}");
						LoggingService.Debug(guid + ": " + title);
						conversion.NameToGuid[title] = new Guid(guid);
						conversion.NameToPath[title] = location;
						conversion.GuidToPath[new Guid(guid)] = location;
					}
				}
				return version;
			}
		}
		
		static bool SetupSolution(Solution newSolution, string fileName)
		{
			string         solutionDirectory     = Path.GetDirectoryName(fileName);
			ProjectSection nestedProjectsSection = null;
			
			bool needsConversion = false;
			
			// read solution files using system encoding, but detect UTF8 if BOM is present
			using (StreamReader sr = new StreamReader(fileName, Encoding.Default, true)) {
				string line = GetFirstNonCommentLine(sr);
				Match match = versionPattern.Match(line);
				if (!match.Success) {
					MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.InvalidSolutionFile}", fileName);
					return false;
				}
				
				switch (match.Result("${Version}")) {
					case "7.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion7}")) {
							return false;
						}
						break;
					case "8.00":
						needsConversion = true;
						if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ConvertSolutionVersion8}")) {
							return false;
						}
						break;
					case "9.00":
						break;
					default:
						MessageService.ShowErrorFormatted("${res:SharpDevelop.Solution.UnknownSolutionVersion}", match.Result("${Version}"));
						return false;
				}
				
				while (true) {
					line = sr.ReadLine();
					
					if (line == null) {
						break;
					}
					match = projectLinePattern.Match(line);
					if (match.Success) {
						string projectGuid  = match.Result("${ProjectGuid}");
						string title        = match.Result("${Title}");
						string location     = match.Result("${Location}");
						string guid         = match.Result("${Guid}");
						
						if (!FileUtility.IsUrl(location)) {
							location = Path.GetFullPath(Path.Combine(solutionDirectory, location));
						}
						
						if (projectGuid == FolderGuid) {
							SolutionFolder newFolder = SolutionFolder.ReadFolder(sr, title, location, guid);
							newSolution.AddFolder(newFolder);
						} else {
							IProject newProject = LanguageBindingService.LoadProject(newSolution, location, title, projectGuid);
							ReadProjectSections(sr, newProject.ProjectSections);
							newProject.IdGuid = guid;
							newSolution.AddFolder(newProject);
						}
						match = match.NextMatch();
					} else {
						match = globalSectionPattern.Match(line);
						if (match.Success) {
							ProjectSection newSection = ProjectSection.ReadGlobalSection(sr, match.Result("${Name}"), match.Result("${Type}"));
							// Don't put the NestedProjects section into the global sections list
							// because it's transformed to a tree representation and the tree representation
							// is transformed back to the NestedProjects section during save.
							if (newSection.Name == "NestedProjects") {
								nestedProjectsSection = newSection;
							} else {
								newSolution.Sections.Add(newSection);
							}
						}
					}
				}
			}
			// Create solution folder 'tree'.
			if (nestedProjectsSection != null) {
				foreach (SolutionItem item in nestedProjectsSection.Items) {
					string from = item.Name;
					string to   = item.Location;
					ISolutionFolderContainer folder = newSolution.guidDictionary[to] as ISolutionFolderContainer;
					folder.AddFolder(newSolution.guidDictionary[from]);
				}
			}
			
			if (newSolution.FixSolutionConfiguration(newSolution.Projects) || needsConversion) {
				// save in new format
				newSolution.Save();
			}
			return true;
		}
		#endregion
		
		#region Configuration/Platform management
		#region Section management
		public ProjectSection GetSolutionConfigurationsSection()
		{
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "SolutionConfigurationPlatforms")
					return sec;
			}
			ProjectSection newSec = new ProjectSection("SolutionConfigurationPlatforms", "preSolution");
			this.Sections.Insert(0, newSec);
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "SolutionConfiguration") {
					this.Sections.Remove(sec);
					foreach (SolutionItem item in sec.Items) {
						newSec.Items.Add(new SolutionItem(item.Name + "|Any CPU", item.Location + "|Any CPU"));
					}
					break;
				}
			}
			return newSec;
		}
		
		public ProjectSection GetProjectConfigurationsSection()
		{
			foreach (ProjectSection sec in Sections) {
				if (sec.Name == "ProjectConfigurationPlatforms")
					return sec;
			}
			ProjectSection newSec = new ProjectSection("ProjectConfigurationPlatforms", "postSolution");
			Sections.Add(newSec);
			foreach (ProjectSection sec in this.Sections) {
				if (sec.Name == "ProjectConfiguration") {
					this.Sections.Remove(sec);
					foreach (SolutionItem item in sec.Items) {
						string name = item.Name;
						string location = item.Location;
						if (!name.Contains("|")) {
							int pos = name.LastIndexOf('.');
							if (pos > 0) {
								string firstpart = name.Substring(0, pos);
								string lastpart = name.Substring(pos);
								if (lastpart == ".0") {
									pos = firstpart.LastIndexOf('.');
									if (pos > 0) {
										lastpart = name.Substring(pos);
										firstpart = name.Substring(0, pos);
									}
								}
								name = firstpart + "|Any CPU" + lastpart;
							}
							
							pos = location.LastIndexOf('|');
							if (pos < 0) {
								location += "|Any CPU";
							} else {
								string platform = location.Substring(pos+1);
								bool found = false;
								foreach (IProject p in this.Projects) {
									if (p.PlatformNames.Contains(platform)) {
										found = true;
										break;
									}
								}
								if (!found) {
									location = location.Substring(0, pos) + "|Any CPU";
								}
							}
						}
						newSec.Items.Add(new SolutionItem(name, location));
					}
					break;
				}
			}
			return newSec;
		}
		
		public bool FixSolutionConfiguration(IEnumerable<IProject> projects)
		{
			ProjectSection solSec = GetSolutionConfigurationsSection();
			ProjectSection prjSec = GetProjectConfigurationsSection();
			bool changed = false;
			if (solSec.Items.Count == 0) {
				solSec.Items.Add(new SolutionItem("Debug|Any CPU", "Debug|Any CPU"));
				solSec.Items.Add(new SolutionItem("Release|Any CPU", "Release|Any CPU"));
				LoggingService.Warn("!! Inserted default SolutionConfigurationPlatforms !!");
				changed = true;
			}
			foreach (IProject project in projects) {
				string guid = project.IdGuid.ToUpperInvariant();
				foreach (SolutionItem configuration in solSec.Items) {
					string searchKey = guid + "." + configuration.Name + ".Build.0";
					if (!prjSec.Items.Exists(delegate (SolutionItem item) {
					                         	return item.Name == searchKey;
					                         }))
					{
						prjSec.Items.Add(new SolutionItem(searchKey, configuration.Location));
						changed = true;
					}
					searchKey = guid + "." + configuration.Name + ".ActiveCfg";
					if (!prjSec.Items.Exists(delegate (SolutionItem item) {
					                         	return item.Name == searchKey;
					                         }))
					{
						prjSec.Items.Add(new SolutionItem(searchKey, configuration.Location));
						changed = true;
					}
				}
			}
			return changed;
		}
		#endregion
		
		#region GetProjectConfigurationsSection/GetPlatformNames
		public IList<string> GetConfigurationNames()
		{
			List<string> configurationNames = new List<string>();
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				string name = AbstractProject.GetConfigurationNameFromKey(item.Name);
				if (!configurationNames.Contains(name))
					configurationNames.Add(name);
			}
			return configurationNames;
		}
		
		public IList<string> GetPlatformNames()
		{
			List<string> platformNames = new List<string>();
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				string name = AbstractProject.GetPlatformNameFromKey(item.Name);
				if (!platformNames.Contains(name))
					platformNames.Add(name);
			}
			return platformNames;
		}
		#endregion
		
		#region Solution - project configuration matching
		public void ApplySolutionConfigurationAndPlatformToProjects()
		{
			foreach (ProjectConfigurationPlatformMatching l in
			         GetActiveConfigurationsAndPlatformsForProjects(preferences.ActiveConfiguration,
			                                                        preferences.ActivePlatform))
			{
				l.Project.ActiveConfiguration = l.Configuration;
				l.Project.ActivePlatform = FixPlatformNameForProject(l.Platform);
			}
		}
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// </summary>
		static string FixPlatformNameForProject(string platformName)
		{
			if (platformName == "Any CPU") {
				return "AnyCPU";
			} else {
				return platformName;
			}
		}
		
		/// <summary>
		/// This is a special case in MSBuild we need to take care of.
		/// Opposite of FixPlatformNameForProject
		/// </summary>
		static string FixPlatformNameForSolution(string platformName)
		{
			if (platformName == "AnyCPU") {
				return "Any CPU";
			} else {
				return platformName;
			}
		}
		
		internal class ProjectConfigurationPlatformMatching
		{
			public readonly IProject Project;
			public string Configuration;
			public string Platform;
			public SolutionItem SolutionItem;
			
			public ProjectConfigurationPlatformMatching(IProject project, string configuration, string platform, SolutionItem solutionItem)
			{
				this.Project = project;
				this.Configuration = configuration;
				this.Platform = platform;
				this.SolutionItem = solutionItem;
			}
			
			public void SetSolutionConfigurationPlatform(ProjectSection section, string newConfiguration, string newPlatform)
			{
				if (this.SolutionItem == null)
					return;
				string oldName = this.SolutionItem.Name;
				this.SolutionItem.Name = this.Project.IdGuid + "." + newConfiguration + "|" + newPlatform + ".Build.0";
				string newName = this.SolutionItem.Name;
				if (StripBuild0(ref oldName) && StripBuild0(ref newName)) {
					oldName += ".ActiveCfg";
					newName += ".ActiveCfg";
					foreach (SolutionItem item in section.Items) {
						if (item.Name == oldName)
							item.Name = newName;
					}
				}
			}
			
			public void SetProjectConfigurationPlatform(ProjectSection section, string newConfiguration, string newPlatform)
			{
				this.Configuration = newConfiguration;
				this.Platform = newPlatform;
				if (this.SolutionItem == null)
					return;
				this.SolutionItem.Location = newConfiguration + "|" + newPlatform;
				string thisName = this.SolutionItem.Name;
				if (StripBuild0(ref thisName)) {
					thisName += ".ActiveCfg";
					foreach (SolutionItem item in section.Items) {
						if (item.Name == thisName)
							item.Location = this.SolutionItem.Location;
					}
				}
			}
			
			internal static bool StripBuild0(ref string s)
			{
				if (s.EndsWith(".Build.0")) {
					s = s.Substring(0, s.Length - ".Build.0".Length);
					return true;
				} else {
					return false;
				}
			}
		}
		
		internal List<ProjectConfigurationPlatformMatching>
			GetActiveConfigurationsAndPlatformsForProjects(string solutionConfiguration, string solutionPlatform)
		{
			List<ProjectConfigurationPlatformMatching> results = new List<ProjectConfigurationPlatformMatching>();
			ProjectSection prjSec = GetProjectConfigurationsSection();
			Dictionary<string, SolutionItem> dict = new Dictionary<string, SolutionItem>(StringComparer.InvariantCultureIgnoreCase);
			foreach (SolutionItem item in prjSec.Items) {
				dict[item.Name] = item;
			}
			string searchKeyPostFix = "." + solutionConfiguration + "|" + solutionPlatform + ".Build.0";
			foreach (IProject p in Projects) {
				string searchKey = p.IdGuid + searchKeyPostFix;
				SolutionItem solutionItem;
				if (dict.TryGetValue(searchKey, out solutionItem)) {
					string targetConfPlat = solutionItem.Location;
					if (targetConfPlat.IndexOf('|') > 0) {
						string conf = AbstractProject.GetConfigurationNameFromKey(targetConfPlat);
						string plat = AbstractProject.GetPlatformNameFromKey(targetConfPlat);
						results.Add(new ProjectConfigurationPlatformMatching(p, conf, plat, solutionItem));
					} else {
						results.Add(new ProjectConfigurationPlatformMatching(p, targetConfPlat, solutionPlatform, solutionItem));
					}
				} else {
					results.Add(new ProjectConfigurationPlatformMatching(p, solutionConfiguration, solutionPlatform, null));
				}
			}
			return results;
		}
		
		internal SolutionItem CreateMatchingItem(string solutionConfiguration, string solutionPlatform, IProject project, string initialLocation)
		{
			SolutionItem item = new SolutionItem(project.IdGuid + "." + solutionConfiguration + "|"
			                                     + solutionPlatform + ".Build.0", initialLocation);
			GetProjectConfigurationsSection().Items.Add(item);
			GetProjectConfigurationsSection().Items.Add(new SolutionItem(project.IdGuid + "." + solutionConfiguration + "|"
			                                                             + solutionPlatform + ".ActiveCfg", initialLocation));
			return item;
		}
		#endregion
		
		#region Rename Solution Configuration/Platform
		public void RenameSolutionConfiguration(string oldName, string newName)
		{
			foreach (string platform in GetPlatformNames()) {
				foreach (ProjectConfigurationPlatformMatching m
				         in GetActiveConfigurationsAndPlatformsForProjects(oldName, platform))
				{
					m.SetSolutionConfigurationPlatform(GetProjectConfigurationsSection(), newName, platform);
				}
			}
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				if (AbstractProject.GetConfigurationNameFromKey(item.Name) == oldName) {
					item.Name = newName + "|" + AbstractProject.GetPlatformNameFromKey(item.Name);
					item.Location = item.Name;
				}
			}
		}
		
		public void RenameSolutionPlatform(string oldName, string newName)
		{
			foreach (string configuration in GetConfigurationNames()) {
				foreach (ProjectConfigurationPlatformMatching m
				         in GetActiveConfigurationsAndPlatformsForProjects(configuration, oldName))
				{
					m.SetSolutionConfigurationPlatform(GetProjectConfigurationsSection(), configuration, newName);
				}
			}
			foreach (SolutionItem item in GetSolutionConfigurationsSection().Items) {
				if (AbstractProject.GetPlatformNameFromKey(item.Name) == oldName) {
					item.Name = AbstractProject.GetConfigurationNameFromKey(item.Name) + "|" + newName;
					item.Location = item.Name;
				}
			}
		}
		#endregion
		
		#region Rename Project Configuration/Platform
		public bool RenameProjectConfiguration(IProject project, string oldName, string newName)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RenameProjectConfiguration(oldName, newName))
				return false;
			// project configuration was renamed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetConfigurationNameFromKey(item.Location) == oldName) {
						item.Location = newName + "|" + AbstractProject.GetPlatformNameFromKey(item.Location);
					}
				}
			}
			return true;
		}
		
		public bool RenameProjectPlatform(IProject project, string oldName, string newName)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RenameProjectPlatform(FixPlatformNameForProject(oldName), FixPlatformNameForProject(newName)))
				return false;
			// project configuration was renamed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetPlatformNameFromKey(item.Location) == oldName) {
						item.Location = AbstractProject.GetConfigurationNameFromKey(item.Location) + "|" + newName;
					}
				}
			}
			return true;
		}
		#endregion
		
		#region Add Solution Configuration/Platform
		/// <summary>
		/// Creates a new solution configuration.
		/// </summary>
		/// <param name="newName">Name of the new configuration</param>
		/// <param name="copyFrom">Name of existing configuration to copy values from, or null</param>
		/// <param name="createInProjects">true to also create the new configuration in all projects</param>
		public void AddSolutionConfiguration(string newName, string copyFrom, bool createInProjects)
		{
			foreach (string platform in this.GetPlatformNames()) {
				AddSolutionConfigurationPlatform(newName, platform, copyFrom, createInProjects, false);
			}
		}
		
		public void AddSolutionPlatform(string newName, string copyFrom, bool createInProjects)
		{
			foreach (string configuration in this.GetConfigurationNames()) {
				AddSolutionConfigurationPlatform(configuration, newName, copyFrom, createInProjects, true);
			}
		}
		
		void AddSolutionConfigurationPlatform(string newConfiguration, string newPlatform,
		                                      string copyFrom, bool createInProjects, bool addPlatform)
		{
			List<ProjectConfigurationPlatformMatching> matchings;
			if (string.IsNullOrEmpty(copyFrom)) {
				matchings = new List<ProjectConfigurationPlatformMatching>();
			} else {
				if (addPlatform) {
					matchings = GetActiveConfigurationsAndPlatformsForProjects(newConfiguration, copyFrom);
				} else {
					matchings = GetActiveConfigurationsAndPlatformsForProjects(copyFrom, newPlatform);
				}
			}
			GetSolutionConfigurationsSection().Items.Add(new SolutionItem(newConfiguration + "|" + newPlatform,
			                                                              newConfiguration + "|" + newPlatform));
			foreach (IProject project in this.Projects) {
				// get old project configuration and platform
				string projectConfiguration, projectPlatform;
				
				ProjectConfigurationPlatformMatching matching = matchings.Find(
					delegate(ProjectConfigurationPlatformMatching m) { return m.Project == project; });
				if (matching != null) {
					projectConfiguration = matching.Configuration;
					projectPlatform = matching.Platform;
				} else {
					projectConfiguration = Linq.ToArray(project.ConfigurationNames)[0];
					projectPlatform = FixPlatformNameForSolution(Linq.ToArray(project.PlatformNames)[0]);
				}
				if (createInProjects) {
					ICollection<string> existingInProject = addPlatform ? project.PlatformNames : project.ConfigurationNames;
					if (existingInProject.Contains(addPlatform ? newPlatform : newConfiguration)) {
						// target platform/configuration already exists, so reference it
						if (addPlatform) {
							projectPlatform = newPlatform;
						} else {
							projectConfiguration = newConfiguration;
						}
					} else {
						// create in project
						IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
						if (pacc != null) {
							if (addPlatform) {
								if (pacc.AddProjectPlatform(FixPlatformNameForProject(newPlatform),
								                            FixPlatformNameForProject(projectPlatform))) {
									projectPlatform = newPlatform;
								}
							} else {
								if (pacc.AddProjectConfiguration(newConfiguration, projectConfiguration)) {
									projectConfiguration = newConfiguration;
								}
							}
						}
					}
				}
				
				// create item combining solution configuration with project configuration
				CreateMatchingItem(newConfiguration, newPlatform, project, projectConfiguration + "|" + projectPlatform);
			}
		}
		#endregion
		
		#region Remove Solution Configuration/Platform
		/// <summary>
		/// Gets the configuration|platform name from a conf item, e.g.
		/// "Release|Any CPU" from
		/// "{7115F3A9-781C-4A95-90AE-B5AB53C4C588}.Release|Any CPU.Build.0"
		/// </summary>
		static string GetKeyFromProjectConfItem(string name)
		{
			int pos = name.IndexOf('.');
			if (pos < 0) return null;
			name = name.Substring(pos + 1);
			if (!ProjectConfigurationPlatformMatching.StripBuild0(ref name)) {
				pos = name.LastIndexOf('.');
				if (pos < 0) return null;
				name = name.Substring(0, pos);
			}
			return name;
		}
		
		public void RemoveSolutionConfiguration(string name)
		{
			GetSolutionConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetConfigurationNameFromKey(item.Name) == name;
				});
			GetProjectConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetConfigurationNameFromKey(GetKeyFromProjectConfItem(item.Name)) == name;
				});
		}
		
		public void RemoveSolutionPlatform(string name)
		{
			GetSolutionConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetPlatformNameFromKey(item.Name) == name;
				});
			GetProjectConfigurationsSection().Items.RemoveAll(
				delegate(SolutionItem item) {
					return AbstractProject.GetPlatformNameFromKey(GetKeyFromProjectConfItem(item.Name)) == name;
				});
		}
		#endregion
		
		#region Remove Project Configuration/Platform
		public bool RemoveProjectConfiguration(IProject project, string name)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RemoveProjectConfiguration(name))
				return false;
			string otherConfigurationName = "other";
			foreach (string configName in project.ConfigurationNames) {
				otherConfigurationName = configName;
			}
			// project configuration was removed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetConfigurationNameFromKey(item.Location) == name) {
						// removed configuration was in use here, replace with other configuration
						item.Location = otherConfigurationName + "|" + AbstractProject.GetPlatformNameFromKey(item.Location);
					}
				}
			}
			return true;
		}
		
		public bool RemoveProjectPlatform(IProject project, string name)
		{
			IProjectAllowChangeConfigurations pacc = project as IProjectAllowChangeConfigurations;
			if (pacc == null)
				return false;
			if (!pacc.RemoveProjectPlatform(name))
				return false;
			string otherPlatformName = "other";
			foreach (string platformName in project.PlatformNames) {
				otherPlatformName = platformName;
			}
			// project configuration was removed successfully, adjust solution:
			foreach (SolutionItem item in GetProjectConfigurationsSection().Items) {
				if (item.Name.ToLowerInvariant().StartsWith(project.IdGuid.ToLowerInvariant())) {
					if (AbstractProject.GetPlatformNameFromKey(item.Location) == name) {
						// removed configuration was in use here, replace with other configuration
						item.Location = AbstractProject.GetConfigurationNameFromKey(item.Location) + "|" + otherPlatformName;
					}
				}
			}
			return true;
		}
		#endregion
		#endregion
		
		#region Load
		static Solution solutionBeingLoaded;
		
		public static Solution SolutionBeingLoaded {
			get {
				return solutionBeingLoaded;
			}
		}
		
		public static Solution Load(string fileName)
		{
			Solution newSolution = new Solution();
			solutionBeingLoaded = newSolution;
			newSolution.Name     = Path.GetFileNameWithoutExtension(fileName);
			
			string extension = Path.GetExtension(fileName).ToUpperInvariant();
			if (extension == ".CMBX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportCmbx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertSolution(newSolution, fileName);
				if (newSolution.FixSolutionConfiguration(newSolution.Projects)) {
					newSolution.Save();
				}
			} else if (extension == ".PRJX") {
				if (!MessageService.AskQuestion("${res:SharpDevelop.Solution.ImportPrjx}")) {
					return null;
				}
				newSolution.fileName = Path.ChangeExtension(fileName, ".sln");
				ICSharpCode.SharpDevelop.Project.Converter.CombineToSolution.ConvertProject(newSolution, fileName);
				if (newSolution.FixSolutionConfiguration(newSolution.Projects)) {
					newSolution.Save();
				}
			} else {
				newSolution.fileName = fileName;
				if (!SetupSolution(newSolution, fileName)) {
					return null;
				}
			}
			
			solutionBeingLoaded = null;
			return newSolution;
		}
		#endregion
		
		#region System.IDisposable interface implementation
		public void Dispose()
		{
			foreach (IProject project in Projects) {
				project.Dispose();
			}
			if (buildEngine != null) {
				buildEngine.UnloadAllProjects();
				buildEngine = null;
			}
		}
		#endregion
		
		public void StartBuild(BuildOptions options)
		{
			MSBuildBasedProject.RunMSBuild(this, null,
			                               this.Preferences.ActiveConfiguration,
			                               this.Preferences.ActivePlatform,
			                               options);
		}
	}
}
