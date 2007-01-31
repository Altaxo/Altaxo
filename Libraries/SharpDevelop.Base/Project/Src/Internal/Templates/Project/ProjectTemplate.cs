﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2043 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	public class OpenFileAction
	{
		string fileName;
		
		public OpenFileAction(string fileName)
		{
			this.fileName = fileName;
		}
		
		public void Run(ProjectCreateInformation projectCreateInformation)
		{
			string parsedFileName = StringParser.Parse(fileName, new string[,] { {"ProjectName", projectCreateInformation.ProjectName} });
			string path = FileUtility.Combine(projectCreateInformation.ProjectBasePath, parsedFileName);
			FileService.OpenFile(path);
		}
	}
	
	/// <summary>
	/// This class defines and holds the new project templates.
	/// </summary>
	public class ProjectTemplate : IComparable
	{
		static List<ProjectTemplate> projectTemplates;
		
		/// <summary>
		/// Gets the list of project templates. Not thread-safe!
		/// </summary>
		public static ReadOnlyCollection<ProjectTemplate> ProjectTemplates {
			get {
				if (projectTemplates == null) {
					LoadProjectTemplates();
				}
				return projectTemplates.AsReadOnly();
			}
		}
		
		string    originator    = null;
		string    created       = null;
		string    lastmodified  = null;
		string    name          = null;
		string    category      = null;
		string    languagename  = null;
		string    description   = null;
		string    icon          = null;
		string    wizardpath    = null;
		string    subcategory   = null;
		
		int IComparable.CompareTo(object other)
		{
			ProjectTemplate pt = other as ProjectTemplate;
			if (pt == null) return -1;
			int res = category.CompareTo(pt.category);
			if (res != 0) return res;
			return name.CompareTo(pt.name);
		}
		
		bool   newProjectDialogVisible = true;
		
		ArrayList actions      = new ArrayList();
		
		CombineDescriptor combineDescriptor = null;
		ProjectDescriptor projectDescriptor = null;
		
		#region Template Properties
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		
		public string Originator {
			get {
				return originator;
			}
		}
		
		public string Created {
			get {
				return created;
			}
		}
		
		public string LastModified {
			get {
				return lastmodified;
			}
		}
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string Subcategory {
			get {
				return subcategory;
			}
		}
		
		public string LanguageName {
			get {
				return languagename;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string Icon {
			get {
				return icon;
			}
		}
		
		public bool NewProjectDialogVisible {
			get {
				return newProjectDialogVisible;
			}
		}
		
		
		[Browsable(false)]
		public CombineDescriptor CombineDescriptor {
			get {
				return combineDescriptor;
			}
		}
		
		[Browsable(false)]
		public ProjectDescriptor ProjectDescriptor {
			get {
				return projectDescriptor;
			}
		}
		#endregion
		
		protected ProjectTemplate(string fileName)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			LoadFromXml(doc.DocumentElement, fileName);
		}
		
		void LoadFromXml(XmlElement templateElement, string xmlFileName)
		{
			// required for warning messages for unknown elements
			templateElement.SetAttribute("fileName", xmlFileName);
			
			originator   = templateElement.GetAttribute("originator");
			created      = templateElement.GetAttribute("created");
			lastmodified = templateElement.GetAttribute("lastModified");
			
			string newProjectDialogVisibleAttr  = templateElement.GetAttribute("newprojectdialogvisible");
			if (string.Equals(newProjectDialogVisibleAttr, "false", StringComparison.OrdinalIgnoreCase))
				newProjectDialogVisible = false;
			
			XmlElement config = templateElement["TemplateConfiguration"];
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].InnerText;
			}
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			
			if (config["LanguageName"] != null) {
				languagename = config["LanguageName"].InnerText;
				WarnObsoleteNode(config["LanguageName"], "use language attribute on the project node instead");
			}
			
			if (config["Subcategory"] != null) {
				subcategory = config["Subcategory"].InnerText;
			}
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon = config["Icon"].InnerText;
			}
			
			string hintPath = Path.GetDirectoryName(xmlFileName);
			if (templateElement["Solution"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(templateElement["Solution"], hintPath);
			} else if (templateElement["Combine"] != null) {
				combineDescriptor = CombineDescriptor.CreateCombineDescriptor(templateElement["Combine"], hintPath);
				WarnObsoleteNode(templateElement["Combine"], "Use <Solution> instead!");
			}
			
			if (templateElement["Project"] != null) {
				projectDescriptor = new ProjectDescriptor(templateElement["Project"], hintPath);
			}
			
			if (combineDescriptor == null && projectDescriptor == null
			    || combineDescriptor != null && projectDescriptor != null)
			{
				throw new TemplateLoadException("Template must contain either Project or Solution node!");
			}
			
			// Read Actions;
			if (templateElement["Actions"] != null) {
				foreach (XmlElement el in templateElement["Actions"]) {
					actions.Add(new OpenFileAction(el.Attributes["filename"].InnerText));
				}
			}
		}
		
		[Conditional("DEBUG")]
		internal static void WarnObsoleteNode(XmlElement element, string message)
		{
			MessageService.ShowWarning("Obsolete node <" + element.Name +
			                           "> used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		[Conditional("DEBUG")]
		internal static void WarnObsoleteAttribute(XmlElement element, string attribute, string message)
		{
			MessageService.ShowWarning("Obsolete attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           "used in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "':\n" + message);
		}
		
		[Conditional("DEBUG")]
		internal static void WarnAttributeMissing(XmlElement element, string attribute)
		{
			MessageService.ShowWarning("Missing attribute <" + element.Name +
			                           " " + attribute + "=...>" +
			                           " in '" + element.OwnerDocument.DocumentElement.GetAttribute("fileName") +
			                           "'");
		}
		
//		string startupProject = null;

		public string CreateProject(ProjectCreateInformation projectCreateInformation)
		{
			if (wizardpath != null) {
				Properties customizer = new Properties();
				customizer.Set("ProjectCreateInformation", projectCreateInformation);
				customizer.Set("ProjectTemplate", this);
				WizardDialog wizard = new WizardDialog("Project Wizard", customizer, wizardpath);
				if (wizard.ShowDialog(ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) != DialogResult.OK) {
					return null;
				}
			}
			if (combineDescriptor != null) {
				return combineDescriptor.CreateSolution(projectCreateInformation, this.languagename);
			} else if (projectDescriptor != null) {
				projectCreateInformation.Solution = new Solution();
				return projectDescriptor.CreateProject(projectCreateInformation, this.languagename).FileName;
			} else {
				return null;
			}
		}
		
		public void RunOpenActions(ProjectCreateInformation projectCreateInformation)
		{
			foreach (OpenFileAction action in actions) {
				action.Run(projectCreateInformation);
			}
		}
		
		public const string TemplatePath = "/SharpDevelop/BackendBindings/Templates";
		
		static void LoadProjectTemplates()
		{
			projectTemplates = new List<ProjectTemplate>();
			string dataTemplateDir = FileUtility.Combine(PropertyService.DataDirectory, "templates", "project");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xpt");
			foreach (string templateDirectory in AddInTree.BuildItems(TemplatePath, null, false)) {
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xpt"));
			}
			foreach (string fileName in files) {
				try {
					projectTemplates.Add(new ProjectTemplate(fileName));
				} catch (XmlException e) {
					MessageService.ShowError(ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n" + e.Message);
				} catch (TemplateLoadException e) {
					MessageService.ShowError(ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n" + e.ToString());
				} catch (Exception e) {
					MessageService.ShowError(e, ResourceService.GetString("Internal.Templates.ProjectTemplate.LoadingError") + "\n(" + fileName + ")\n");
				}
			}
			projectTemplates.Sort();
		}
	}
}
