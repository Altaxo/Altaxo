// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Xml;
using ICSharpCode.SharpDevelop.Internal.Templates;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class handles the code templates
	/// </summary>
	public class CodeTemplateLoader
	{
		static string TemplateFileName = "SharpDevelop-templates.xml";
		static string TemplateVersion  = "2.0";
		
		static ArrayList templateGroups = new ArrayList();
		
		public static ArrayList TemplateGroups {
			get {
				return templateGroups;
			}
			set {
				templateGroups = value;
				Debug.Assert(templateGroups != null);
			}
		}
		
		public static CodeTemplateGroup GetTemplateGroupPerFilename(string fileName)
		{
			return GetTemplateGroupPerExtension(Path.GetExtension(fileName));
		}
		public static CodeTemplateGroup GetTemplateGroupPerExtension(string extension)
		{
			foreach (CodeTemplateGroup group in templateGroups) {
				foreach (string groupExtension in group.Extensions) {
					if (groupExtension == extension) {
						return group;
					}
				}
			}
			return null;
		}
		
		static bool LoadTemplatesFromStream(string filename)
		{
			XmlDocument doc = new XmlDocument();
			try {
				doc.Load(filename);
				
				templateGroups = new ArrayList();
				
				if (doc.DocumentElement.GetAttribute("version") != TemplateVersion) {
					return false;
				}
				
				foreach (XmlElement el in doc.DocumentElement.ChildNodes) {
					templateGroups.Add(new CodeTemplateGroup(el));
				}
			} catch (Exception) {
				return false;
			}
			return true;
		}
		
		static void WriteTemplatesToFile(string fileName)
		{
			XmlDocument doc    = new XmlDocument();
			
			doc.LoadXml("<CodeTemplates version = \"" + TemplateVersion + "\" />");
			
			foreach (CodeTemplateGroup codeTemplateGroup in templateGroups) {
				doc.DocumentElement.AppendChild(codeTemplateGroup.ToXmlElement(doc));
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), fileName, FileErrorPolicy.ProvideAlternative);
		}
		
		/// <summary>
		/// This method loads the code templates from a XML based
		/// configuration file.
		/// </summary>
		static CodeTemplateLoader()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			
			if (!LoadTemplatesFromStream(Path.Combine(propertyService.ConfigDirectory, TemplateFileName))) {
				Console.WriteLine("Templates: can't load user defaults, reading system defaults");
				if (!LoadTemplatesFromStream(propertyService.DataDirectory + 
				                             Path.DirectorySeparatorChar   + "options" +
				                             Path.DirectorySeparatorChar   + TemplateFileName)) {
					IMessageService messageService = (IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowWarning("${res:Internal.Templates.CodeTemplateLoader.CantLoadTemplatesWarning}");
				}
			}
		}
		
		/// <summary>
		/// This method saves the code templates to a XML based
		/// configuration file in the current user's own files directory
		/// </summary>
		public static void SaveTemplates()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			WriteTemplatesToFile(Path.Combine(propertyService.ConfigDirectory, TemplateFileName));
		}
	}
}
