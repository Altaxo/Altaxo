// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class is used inside the combine templates for projects.
	/// </summary>
	public class ProjectDescriptor
	{
		string name;
		string relativePath;
		string languageName = null;
		
		ArrayList files      = new ArrayList(); // contains FileTemplate classes
		ArrayList references = new ArrayList(); 
		
		XmlElement projectOptions = null;
		
		#region public properties
		public string LanguageName {
			get {
				return languageName;
			}
		}

		public ArrayList Files {
			get {
				return files;
			}
		}

		public ArrayList References {
			get {
				return references;
			}
		}

		public XmlElement ProjectOptions {
			get {
				return projectOptions;
			}
		}
		#endregion

		protected ProjectDescriptor(string name, string relativePath)
		{
			this.name = name;
			this.relativePath = relativePath;
		}
		
		public string CreateProject(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			string language = languageName != null && languageName.Length > 0 ? languageName : defaultLanguage;
			
			ILanguageBinding languageinfo = languageBindingService.GetBindingPerLanguageName(language);
			
			if (languageinfo == null) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError("Can't create project with type :" + language);
				return String.Empty;
			}
			
			IProject project = languageinfo.CreateProject(projectCreateInformation, projectOptions);
			
			string newProjectName = stringParserService.Parse(name, new string[,] { 
				{"ProjectName", projectCreateInformation.ProjectName}
			});
			
			project.Name = newProjectName;
			
			// Add References
			foreach (ProjectReference projectReference in references) {
				project.ProjectReferences.Add(projectReference);
			}
			
			// Add Files
			foreach (FileDescriptionTemplate file in files) {
				string fileName = fileUtilityService.GetDirectoryNameWithSeparator(projectCreateInformation.ProjectBasePath) + stringParserService.Parse(file.Name, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} });
				
				project.ProjectFiles.Add(new ProjectFile(fileName));
				
				if (File.Exists(fileName)) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					if (!messageService.AskQuestion("File " + fileName + " already exists, do you want to overwrite\nthe existing file ?", "File already exists")) {
						continue;
					}
				}
				
				try {
					if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
						Directory.CreateDirectory(Path.GetDirectoryName(fileName));
					}
					StreamWriter sr = File.CreateText(fileName);
					sr.Write(stringParserService.Parse(file.Content, new string[,] { {"ProjectName", projectCreateInformation.ProjectName}, {"FileName", fileName}}));
					sr.Close();
				} catch (Exception ex) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowError(ex, "File " + fileName + " could not be written.");
				}
			}
			
			// Save project
			string projectLocation = fileUtilityService.GetDirectoryNameWithSeparator(projectCreateInformation.ProjectBasePath) + newProjectName + ".prjx";
			
			if (File.Exists(projectLocation)) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				if (messageService.AskQuestion("Project file " + projectLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists")) {
					project.SaveProject(projectLocation);
				}
			} else {
				project.SaveProject(projectLocation);
			}
			
			return projectLocation;
		}
		
		public static ProjectDescriptor CreateProjectDescriptor(XmlElement element)
		{
			ProjectDescriptor projectDescriptor = new ProjectDescriptor(element.Attributes["name"].InnerText, element.Attributes["directory"].InnerText);
			
			projectDescriptor.projectOptions = element["Options"];
			if (element.Attributes["language"] != null) {
				projectDescriptor.languageName = element.Attributes["language"].InnerText;
			}
			
			if (element["Files"] != null) {
				foreach (XmlNode node in element["Files"].ChildNodes) {
					if (node != null && node.Name == "File") {
						projectDescriptor.files.Add(new FileDescriptionTemplate(node.Attributes["name"].InnerText, node.InnerText));
					}
				}
			}
			if (element["References"] != null) {
				foreach (XmlNode node in element["References"].ChildNodes) {
					if (node != null && node.Name == "Reference") {
						ProjectReference projectReference = new ProjectReference();
						
						projectReference.ReferenceType = (ReferenceType)Enum.Parse(typeof(ReferenceType), node.Attributes["type"].InnerXml);
						projectReference.Reference     = node.Attributes["refto"].InnerXml;
						projectDescriptor.references.Add(projectReference);
					}
				}
			}
			return projectDescriptor;
		}
	}
}
