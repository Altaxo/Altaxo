// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class defines and holds the new file templates.
	/// </summary>
	public class FileTemplate
	{
		public static ArrayList FileTemplates = new ArrayList();
		
		string    originator   = null;
		string    created      = null;
		string    lastmodified = null;
		string    name         = null;
		string    category     = null;
		string    languagename = null;
		string    description  = null;
		string    icon         = null;
		
		string    wizardpath   = null;
		
		ArrayList files        = new ArrayList(); // contains FileDescriptionTemplate classes
		
		XmlElement fileoptions = null;
		
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
		
		public XmlElement FileOptions {
			get {
				return fileoptions;
			}
		}
		
		public ArrayList Files {
			get {
				return files;
			}
		}
		
		public FileTemplate(string filename)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			XmlElement config = doc.DocumentElement["TemplateConfiguration"];
			
			originator   = doc.DocumentElement.Attributes["Originator"].InnerText;
			created      = doc.DocumentElement.Attributes["Created"].InnerText;
			lastmodified = doc.DocumentElement.Attributes["LastModified"].InnerText;
			
			name         = config["Name"].InnerText;
			category     = config["Category"].InnerText;
			languagename = config["LanguageName"].InnerText;
			
			if (config["Description"] != null) {
				description  = config["Description"].InnerText;
			}
			
			if (config["Icon"] != null) {
				icon         = config["Icon"].InnerText;
			}
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].Attributes["path"].InnerText;
			}
			
			fileoptions = doc.DocumentElement["FileOptions"];
			
			// load the files
			XmlElement files  = doc.DocumentElement["TemplateFiles"];
			XmlNodeList nodes = files.ChildNodes;
			foreach (XmlElement filenode in nodes) {
				FileDescriptionTemplate template = new FileDescriptionTemplate(filenode.Attributes["DefaultName"].InnerText + filenode.Attributes["DefaultExtension"].InnerText, filenode.InnerText);
				this.files.Add(template);
			}
		}
		
		static void LoadFileTemplate(string filename)
		{
			FileTemplates.Add(new FileTemplate(filename));
		}
		
		static FileTemplate()
		{
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			PropertyService    propertyService    = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			IMessageService    messageService     = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
			
			StringCollection files = fileUtilityService.SearchDirectory(propertyService.DataDirectory + 
			                            Path.DirectorySeparatorChar + "templates" + 
			                            Path.DirectorySeparatorChar + "file", "*.xft");
			foreach (string file in files) {
				try {
					LoadFileTemplate(file);
				} catch(Exception e) {
					messageService.ShowError(e, "Error loading template file " + file + ".");
				}
			}
		}
	}
}
