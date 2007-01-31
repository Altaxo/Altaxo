﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2146 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

using MSBuild = Microsoft.Build.BuildEngine;
using Import = System.Collections.Generic.KeyValuePair<string, string>;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class is used inside the combine templates for projects.
	/// </summary>
	public sealed class ProjectDescriptor
	{
		private class ProjectProperty
		{
			public string Name, Value;
			public string Configuration, Platform;
			public PropertyStorageLocations Location;
			/// <summary>specifies whether to MSBuild-escape the value</summary>
			public bool ValueIsLiteral;
			
			public ProjectProperty(string name, string value, string configuration, string platform, PropertyStorageLocations location)
			{
				this.Name = name;
				this.Value = value;
				this.Configuration = configuration;
				this.Platform = platform;
				this.Location = location;
			}
		}
		
		string name;
		string relativePath;
		
		/// <summary>
		/// The language of the project.
		/// </summary>
		string languageName;
		
		bool clearExistingImports;
		List<Import> projectImports = new List<Import>();
		
		List<FileDescriptionTemplate> files = new List<FileDescriptionTemplate>();
		List<ProjectItem> projectItems = new List<ProjectItem>();
		List<ProjectProperty> projectProperties = new List<ProjectProperty>();
		
		/// <summary>
		/// Creates a project descriptor for the project node specified by the xml element.
		/// </summary>
		/// <param name="element">The &lt;Project&gt; node of the xml template file.</param>
		/// <param name="xmlFileName">The name of the xml file. Used to display warning/error messages</param>
		public ProjectDescriptor(XmlElement element, string hintPath)
		{
			if (element == null)
				throw new ArgumentNullException("element");
			if (hintPath == null)
				throw new ArgumentNullException("hintPath");
			
			if (element.HasAttribute("name")) {
				name = element.GetAttribute("name");
			} else {
				name = "${ProjectName}";
			}
			if (element.HasAttribute("directory")) {
				relativePath = element.GetAttribute("directory");
			} else {
				relativePath = ".";
			}
			languageName = element.GetAttribute("language");
			if (string.IsNullOrEmpty(languageName)) {
				ProjectTemplate.WarnAttributeMissing(element, "language");
			}
			
			LoadElementChildren(element, hintPath);
		}
		
		#region Loading XML template
		void LoadElementChildren(XmlElement parentElement, string hintPath)
		{
			foreach (XmlElement node in ChildElements(parentElement)) {
				LoadElement(node, hintPath);
			}
		}
		
		static IEnumerable<XmlElement> ChildElements(XmlElement parentElement)
		{
			return Linq.OfType<XmlElement>(parentElement.ChildNodes);
		}
		
		void LoadElement(XmlElement node, string hintPath)
		{
			switch (node.Name) {
				case "Options":
					ProjectTemplate.WarnObsoleteNode(node, "Options are no longer supported, use properties instead.");
					break;
				case "ProjectItems":
					LoadProjectItems(node);
					break;
				case "Files":
					LoadFiles(node, hintPath);
					break;
				case "Imports":
					LoadImports(node);
					break;
				case "PropertyGroup":
					LoadPropertyGroup(node);
					break;
				case "Include":
					TemplateLoadException.AssertAttributeExists(node, "src");
					string includeFileName = Path.Combine(hintPath, node.GetAttribute("src"));
					try {
						XmlDocument doc = new XmlDocument();
						doc.Load(includeFileName);
						doc.DocumentElement.SetAttribute("fileName", includeFileName);
						if (doc.DocumentElement.Name == "Include") {
							LoadElementChildren(doc.DocumentElement, Path.GetDirectoryName(includeFileName));
						} else {
							LoadElement(doc.DocumentElement, Path.GetDirectoryName(includeFileName));
						}
					} catch (XmlException ex) {
						throw new TemplateLoadException("Error loading include file " + includeFileName, ex);
					}
					break;
				default:
					throw new TemplateLoadException("Unknown node in <Project>: " + node.Name);
			}
		}
		
		void LoadProjectItems(XmlElement projectItemsElement)
		{
			foreach (XmlElement projectItemElement in ChildElements(projectItemsElement)) {
				ProjectItem item = new UnknownProjectItem(null,
				                                          projectItemElement.Name,
				                                          projectItemElement.GetAttribute("Include"));
				foreach (XmlElement metadataElement in ChildElements(projectItemElement)) {
					item.SetMetadata(metadataElement.Name, metadataElement.InnerText);
				}
				projectItems.Add(item);
			}
		}
		
		void LoadPropertyGroup(XmlElement propertyGroupElement)
		{
			string configuration = propertyGroupElement.GetAttribute("configuration");
			string platform = propertyGroupElement.GetAttribute("platform");
			PropertyStorageLocations storageLocation;
			if (string.IsNullOrEmpty(configuration) && string.IsNullOrEmpty(platform)) {
				storageLocation = PropertyStorageLocations.Base;
			} else {
				storageLocation = 0;
				if (!string.IsNullOrEmpty(configuration))
					storageLocation |= PropertyStorageLocations.ConfigurationSpecific;
				if (!string.IsNullOrEmpty(platform))
					storageLocation |= PropertyStorageLocations.PlatformSpecific;
			}
			if (string.Equals(propertyGroupElement.GetAttribute("userFile"), "true", StringComparison.OrdinalIgnoreCase)) {
				storageLocation |= PropertyStorageLocations.UserFile;
			}
			
			foreach (XmlElement propertyElement in ChildElements(propertyGroupElement)) {
				ProjectProperty p = new ProjectProperty(propertyElement.Name,
				                                        propertyElement.InnerText,
				                                        configuration, platform, storageLocation);
				if (string.Equals(propertyGroupElement.GetAttribute("escapeValue"), "false", StringComparison.OrdinalIgnoreCase)) {
					p.ValueIsLiteral = false;
				} else {
					p.ValueIsLiteral = true;
				}
				projectProperties.Add(p);
			}
		}
		
		void LoadImports(XmlElement importsElement)
		{
			if (string.Equals(importsElement.GetAttribute("clear"), "true", StringComparison.OrdinalIgnoreCase)) {
				clearExistingImports = true;
			}
			foreach (XmlElement importElement in ChildElements(importsElement)) {
				TemplateLoadException.AssertAttributeExists(importElement, "Project");
				projectImports.Add(new Import(
					importElement.GetAttribute("Project"),
					importElement.HasAttribute("Condition") ? importElement.GetAttribute("Condition") : null
				));
			}
		}
		
		void LoadFiles(XmlElement filesElement, string hintPath)
		{
			foreach (XmlElement fileElement in ChildElements(filesElement)) {
				files.Add(new FileDescriptionTemplate(fileElement, hintPath));
			}
		}
		#endregion
		
		#region Create new project from template
		public IProject CreateProject(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			// remember old outerProjectBasePath
			string outerProjectBasePath = projectCreateInformation.ProjectBasePath;
			string outerProjectName = projectCreateInformation.ProjectName;
			try
			{
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.ProjectBasePath, this.relativePath);
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
				
				string language = string.IsNullOrEmpty(languageName) ? defaultLanguage : languageName;
				LanguageBindingDescriptor descriptor = LanguageBindingService.GetCodonPerLanguageName(language);
				ILanguageBinding languageinfo = (descriptor != null) ? descriptor.Binding : null;
				
				if (languageinfo == null) {
					StringParser.Properties["type"] = language;
					MessageService.ShowError("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.CantCreateProjectWithTypeError}");
					return null;
				}
				
				string newProjectName = StringParser.Parse(name, new string[,] {
				                                           	{"ProjectName", projectCreateInformation.ProjectName}
				                                           });
				string projectLocation = Path.GetFullPath(Path.Combine(projectCreateInformation.ProjectBasePath,
				                                                       newProjectName + LanguageBindingService.GetProjectFileExtension(language)));
				
				
				StringBuilder standardNamespace = new StringBuilder();
				// filter 'illegal' chars from standard namespace
				if (newProjectName != null && newProjectName.Length > 0) {
					char ch = '.';
					for (int i = 0; i < newProjectName.Length; ++i) {
						if (ch == '.') {
							// at beginning or after '.', only a letter or '_' is allowed
							ch = newProjectName[i];
							if (!Char.IsLetter(ch)) {
								standardNamespace.Append('_');
							} else {
								standardNamespace.Append(ch);
							}
						} else {
							ch = newProjectName[i];
							// can only contain letters, digits or '_'
							if (!Char.IsLetterOrDigit(ch) && ch != '.') {
								standardNamespace.Append('_');
							} else {
								standardNamespace.Append(ch);
							}
						}
					}
				}
				
				projectCreateInformation.OutputProjectFileName = projectLocation;
				projectCreateInformation.RootNamespace = standardNamespace.ToString();
				projectCreateInformation.ProjectName = newProjectName;
				
				StringParser.Properties["StandardNamespace"] = projectCreateInformation.RootNamespace;
				
				IProject project = languageinfo.CreateProject(projectCreateInformation);
				
				// Add Project items
				foreach (ProjectItem projectItem in projectItems) {
					ProjectItem newProjectItem = new UnknownProjectItem(
						project,
						StringParser.Parse(projectItem.ItemType.ItemName),
						StringParser.Parse(projectItem.Include)
					);
					foreach (string metadataName in projectItem.MetadataNames) {
						newProjectItem.SetEvaluatedMetadata(
							StringParser.Parse(metadataName),
							StringParser.Parse(projectItem.GetMetadata(metadataName))
						);
					}
					((IProjectItemListProvider)project).AddProjectItem(newProjectItem);
				}
				
				// Add Imports
				if (clearExistingImports || projectImports.Count > 0) {
					if (!(project is MSBuildBasedProject))
						throw new Exception("<Imports> may be only used in project templates for MSBuildBasedProjects");
					
					if (clearExistingImports) {
						MSBuildInternals.ClearImports(((MSBuildBasedProject)project).MSBuildProject);
					}
					foreach (Import projectImport in projectImports) {
						((MSBuildBasedProject)project).MSBuildProject.AddNewImport(projectImport.Key, projectImport.Value);
					}
					((MSBuildBasedProject)project).CreateItemsListFromMSBuild();
				}
				
				if (projectProperties.Count > 0) {
					if (!(project is MSBuildBasedProject))
						throw new Exception("<PropertyGroup> may be only used in project templates for MSBuildBasedProjects");
					
					foreach (ProjectProperty p in projectProperties) {
						((MSBuildBasedProject)project).SetProperty(
							StringParser.Parse(p.Configuration),
							StringParser.Parse(p.Platform),
							StringParser.Parse(p.Name),
							StringParser.Parse(p.Value),
							p.Location,
							p.ValueIsLiteral
						);
					}
				}
				
				// Add Files
				foreach (FileDescriptionTemplate file in files) {
					string fileName = Path.Combine(projectCreateInformation.ProjectBasePath, StringParser.Parse(file.Name, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} }));
					FileProjectItem projectFile = new FileProjectItem(project, project.GetDefaultItemType(fileName));
					
					projectFile.Include = FileUtility.GetRelativePath(project.Directory, fileName);
					
					file.SetProjectItemProperties(projectFile);
					
					((IProjectItemListProvider)project).AddProjectItem(projectFile);
					
					if (File.Exists(fileName)) {
						StringParser.Properties["fileName"] = fileName;
						if (!MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
							continue;
						}
					}
					
					try {
						if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(fileName));
						}
						if (file.ContentData != null) {
							// Binary content
							File.WriteAllBytes(fileName, file.ContentData);
						} else {
							// Textual content
							StreamWriter sr = new StreamWriter(File.Create(fileName), ParserService.DefaultFileEncoding);
							sr.Write(StringParser.Parse(StringParser.Parse(file.Content, new string[,] { {"ProjectName", projectCreateInformation.ProjectName}, {"FileName", fileName}})));
							sr.Close();
						}
					} catch (Exception ex) {
						StringParser.Properties["fileName"] = fileName;
						MessageService.ShowError(ex, "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.FileCouldntBeWrittenError}");
					}
				}
				
				// Save project
				if (File.Exists(projectLocation)) {
					StringParser.Properties["projectLocation"] = projectLocation;
					if (MessageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteProjectQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
						project.Save();
					}
				} else {
					project.Save();
				}
				
				projectCreateInformation.CreatedProjects.Add(project.FileName);
				return project;
			}
			finally
			{
				// set back outerProjectBasePath
				projectCreateInformation.ProjectBasePath = outerProjectBasePath;
				projectCreateInformation.ProjectName = outerProjectName;
			}
		}
		#endregion
	}
}
