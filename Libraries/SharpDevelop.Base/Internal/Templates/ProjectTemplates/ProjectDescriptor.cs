// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using ICSharpCode.Core.Properties;
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
			// remember old outerProjectBasePath
			string outerProjectBasePath = projectCreateInformation.ProjectBasePath;
			try
			{
				projectCreateInformation.ProjectBasePath = Path.Combine(projectCreateInformation.ProjectBasePath, this.relativePath);
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath))
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				
				LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
				StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				
				string language = languageName != null && languageName.Length > 0 ? languageName : defaultLanguage;
				
				ILanguageBinding languageinfo = languageBindingService.GetBindingPerLanguageName(language);
				
				if (languageinfo == null) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					stringParserService.Properties["type"] = language;
					messageService.ShowError("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.CantCreateProjectWithTypeError}");
					return String.Empty;
				}
				
				IProject project = languageinfo.CreateProject(projectCreateInformation, projectOptions);
				
				string newProjectName = stringParserService.Parse(name, new string[,] {
					{"ProjectName", projectCreateInformation.ProjectName}
				});
				
				project.Name                     = newProjectName;
				StringBuilder standardNamespace  = new StringBuilder();
				
				// filter 'illegal' chars from standard namespace
				if (newProjectName != null && newProjectName.Length > 0) {
					char ch = newProjectName[0];
					// can only begin with a letter or '_'
					if (!Char.IsLetter(ch)) {
						standardNamespace.Append('_');
					} else {
						standardNamespace.Append(ch);
					}
					for (int i = 1; i < newProjectName.Length; ++i) {
						ch = newProjectName[i];
						// can only contain letters, digits or '_'
						if (!Char.IsLetterOrDigit(ch) && ch != '.') {
							standardNamespace.Append('_');
						} else {
							standardNamespace.Append(ch);
							
						}
					}
				}
				project.StandardNamespace = standardNamespace.ToString();
				stringParserService.Properties["StandardNamespace"] = project.StandardNamespace;
				
				// Add References
				foreach (ProjectReference projectReference in references) {
					project.ProjectReferences.Add(projectReference);
				}
				
				// Add Files
				foreach (FileDescriptionTemplate file in files) {
					string fileName = fileUtilityService.GetDirectoryNameWithSeparator(projectCreateInformation.ProjectBasePath) + stringParserService.Parse(file.Name, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} });
					ProjectFile projectFile = new ProjectFile(fileName);
					if (!project.IsCompileable(fileName)) {
						projectFile.BuildAction = BuildAction.Nothing;
					}
					project.ProjectFiles.Add(projectFile);
					
					
					if (File.Exists(fileName)) {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						stringParserService.Properties["fileName"] = fileName;
						if (!messageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
							continue;
						}
					}
					
					try {
						if (!Directory.Exists(Path.GetDirectoryName(fileName))) {
							Directory.CreateDirectory(Path.GetDirectoryName(fileName));
						}
						PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
						IProperties properties = ((IProperties)propertyService.GetProperty("ICSharpCode.TextEditor.Document.Document.DefaultDocumentAggregatorProperties", new DefaultProperties()));
						
						StreamWriter sr = new StreamWriter(File.Create(fileName), Encoding.GetEncoding(properties.GetProperty("Encoding", 1252)));
						sr.Write(stringParserService.Parse(stringParserService.Parse(file.Content, new string[,] { {"ProjectName", projectCreateInformation.ProjectName}, {"FileName", fileName}})));
						sr.Close();
					} catch (Exception ex) {
						IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
						stringParserService.Properties["fileName"] = fileName;
						messageService.ShowError(ex, "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.FileCouldntBeWrittenError}");
					}
				}
				
				// Save project
				string projectLocation = fileUtilityService.GetDirectoryNameWithSeparator(projectCreateInformation.ProjectBasePath) + newProjectName + ".prjx";
				
				if (File.Exists(projectLocation)) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					stringParserService.Properties["projectLocation"] = projectLocation;
					if (messageService.AskQuestion("${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteProjectQuestion}", "${res:ICSharpCode.SharpDevelop.Internal.Templates.ProjectDescriptor.OverwriteQuestion.InfoName}")) {
						project.SaveProject(projectLocation);
					}
				} else {
					project.SaveProject(projectLocation);
				}
				
				return projectLocation;
			}
			finally
			{
				// set back outerProjectBasePath
				projectCreateInformation.ProjectBasePath = outerProjectBasePath;
			}
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
						XmlElement filenode = (XmlElement)node;
						FileDescriptionTemplate template = new FileDescriptionTemplate(filenode.GetAttribute("name"),
						                                                               filenode.GetAttribute("language"),
						                                                               filenode.InnerText);
						projectDescriptor.files.Add(template);
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
