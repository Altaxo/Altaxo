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
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class CombineDescriptor
	{
		ArrayList projectDescriptors = new ArrayList();
		ArrayList combineDescriptors = new ArrayList();
		
		string name;
		string startupProject    = null;
		string relativeDirectory = null;
	
		#region public properties
		public string StartupProject {
			get {
				return startupProject;
			}
		}

		public ArrayList ProjectDescriptors {
			get {
				return projectDescriptors;
			}
		}

		public ArrayList CombineDescriptors {
			get {
				return projectDescriptors;
			}
		}
		#endregion

		protected CombineDescriptor(string name)
		{
			this.name = name;
		}
		
		public string CreateCombine(ProjectCreateInformation projectCreateInformation, string defaultLanguage)
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			Combine newCombine     = new Combine();
			string  newCombineName = stringParserService.Parse(name, new string[,] { 
				{"ProjectName", projectCreateInformation.ProjectName}
			});
			
			newCombine.Name = newCombineName;
			
			string oldCombinePath = projectCreateInformation.CombinePath;
			string oldProjectPath = projectCreateInformation.ProjectBasePath;
			if (relativeDirectory != null && relativeDirectory.Length > 0 && relativeDirectory != ".") {
				projectCreateInformation.CombinePath     = projectCreateInformation.CombinePath + Path.DirectorySeparatorChar + relativeDirectory;
				projectCreateInformation.ProjectBasePath = projectCreateInformation.CombinePath + Path.DirectorySeparatorChar + relativeDirectory;
				if (!Directory.Exists(projectCreateInformation.CombinePath)) {
					Directory.CreateDirectory(projectCreateInformation.CombinePath);
				}
				if (!Directory.Exists(projectCreateInformation.ProjectBasePath)) {
					Directory.CreateDirectory(projectCreateInformation.ProjectBasePath);
				}
			}

			// Create sub projects
			foreach (ProjectDescriptor projectDescriptor in projectDescriptors) {
				newCombine.AddEntry(projectDescriptor.CreateProject(projectCreateInformation, defaultLanguage));
			}
			
			// Create sub combines
			foreach (CombineDescriptor combineDescriptor in combineDescriptors) {
				newCombine.AddEntry(combineDescriptor.CreateCombine(projectCreateInformation, defaultLanguage));
			}
			
			projectCreateInformation.CombinePath = oldCombinePath;
			projectCreateInformation.ProjectBasePath = oldProjectPath;
			
			// Save combine
			string combineLocation = fileUtilityService.GetDirectoryNameWithSeparator(projectCreateInformation.CombinePath) + newCombineName + ".cmbx";
			if (File.Exists(combineLocation)) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				if (messageService.AskQuestion("Combine file " + combineLocation + " already exists, do you want to overwrite\nthe existing file ?")) {
					newCombine.SaveCombine(combineLocation);
				}
			} else {
				newCombine.SaveCombine(combineLocation);
			}
			newCombine.Dispose();
			return combineLocation;
		}
		
		public static CombineDescriptor CreateCombineDescriptor(XmlElement element)
		{
			CombineDescriptor combineDescriptor = new CombineDescriptor(element.Attributes["name"].InnerText);
			
			if (element.Attributes["directory"] != null) {
				combineDescriptor.relativeDirectory = element.Attributes["directory"].InnerText;
			}
			
			if (element["Options"] != null && element["Options"]["StartupProject"] != null) {
				combineDescriptor.startupProject = element["Options"]["StartupProject"].InnerText;
			}
			
			foreach (XmlNode node in element.ChildNodes) {
				if (node != null) {
					switch (node.Name) {
						case "Project":
							combineDescriptor.projectDescriptors.Add(ProjectDescriptor.CreateProjectDescriptor((XmlElement)node));
							break;
						case "Combine":
							combineDescriptor.combineDescriptors.Add(CreateCombineDescriptor((XmlElement)node));
							break;
					}
				}
			}
			return combineDescriptor;
		}
	}
}
