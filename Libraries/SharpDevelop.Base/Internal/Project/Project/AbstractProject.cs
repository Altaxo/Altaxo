// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Globalization;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;
using System.Xml;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns;

using ICSharpCode.SharpDevelop.Internal.Project.Collections;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	
	
	/// <summary>
	/// External language bindings must extend this class
	/// </summary>
	[XmlNodeName("Project")]
	public abstract class AbstractProject : LocalizedObject, IProject
	{
		readonly static string currentProjectFileVersion = "1.1";
		readonly static string configurationNodeName     = "Configuration";
		
		protected string basedirectory   = String.Empty;

		[XmlAttribute("name")]
		protected string projectname     = "New Project";

		[XmlAttribute("description")]
		protected string description     = "";

		[XmlAttribute("newfilesearch")]
		protected NewFileSearch newFileSearch  = NewFileSearch.None;

		[XmlAttribute("enableviewstate")]
		protected bool          enableViewState = true;

		[XmlSetAttribute(typeof(ProjectFile),      "Contents")]
		protected ProjectFileCollection      projectFiles       = new ProjectFileCollection();

		[XmlSetAttribute(typeof(ProjectReference), "References")]
		protected ProjectReferenceCollection projectReferencess = new ProjectReferenceCollection();
		
		protected DeployInformation deployInformation = new DeployInformation();
		FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		
		[Browsable(false)]
		public string BaseDirectory {
			get {
				return basedirectory;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.Project.Name}",
		                   Description ="${res:ICSharpCode.SharpDevelop.Internal.Project.Project.Description}")]
		public string Name {
			get {
				return projectname;
			}
			set {
				if (projectname != value && value != null && value.Length > 0) {
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					if (!projectService.ExistsEntryWithName(value)) {
						string oldName = projectname;
						projectname = value;
						projectService.OnRenameProject(new ProjectRenameEventArgs(this, oldName, value));
						OnNameChanged(EventArgs.Empty);
					}
				}
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectClass.Description}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectClass.Description.Description}")]
		[DefaultValue("")]
		public string Description {
			get {
				return description;
			}
			set {
				description = value;
			}
		}
		
		[Browsable(false)]
		public ProjectFileCollection ProjectFiles {
			get {
				return projectFiles;
			}
		}
		
		[Browsable(false)]
		public ProjectReferenceCollection ProjectReferences {
			get {
				return projectReferencess;
			}
		}
		
		protected ArrayList configurations = new ArrayList();
		protected IConfiguration activeConfiguration = null;
		
		[Browsable(false)]
		public ArrayList Configurations {
			get {
				return configurations;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.Project.ActiveConfiguration}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.Project.ActiveConfiguration.Description}")]
		[TypeConverter(typeof(ProjectActiveConfigurationTypeConverter))]
		public IConfiguration ActiveConfiguration {
			get {
				if (activeConfiguration == null && configurations.Count > 0) {
					return (IConfiguration)configurations[0];
				}
				return activeConfiguration;
			}
			set {
				activeConfiguration = value;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.Project.NewFileSearch}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.Project.NewFileSearch.Description}")]
		[DefaultValue(NewFileSearch.None)]
		public NewFileSearch NewFileSearch {
			get {
				return newFileSearch;
			}

			set {
				newFileSearch = value;
			}
		}
		
		[Browsable(false)]
		public bool EnableViewState {
			get {
				return enableViewState;
			}
			set {
				enableViewState = value;
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.Project.ProjectType}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.Project.ProjectType.Description}")]
		public abstract string ProjectType {
			get;
		}
		
		[Browsable(false)]
		public DeployInformation DeployInformation {
			get {
				return deployInformation;
			}
		}

		public AbstractProject()
		{
		}

		public bool IsFileInProject(string filename)
		{
			foreach (ProjectFile file in projectFiles) {
				// WINDOWS DEPENDENCY:
				if (file.Name.ToUpper() == filename.ToUpper()) {
					return true;
				}
			}
			return false;
		}

		public bool IsCompileable(string fileName)
		{
			LanguageBindingService languageBindingService = (LanguageBindingService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(LanguageBindingService));
			return languageBindingService.GetBindingPerLanguageName(ProjectType).CanCompile(fileName);
		}
		
		public void SearchNewFiles()
		{
			if (newFileSearch == NewFileSearch.None) {
				return;
			}

			StringCollection newFiles   = new StringCollection();
			StringCollection collection = fileUtilityService.SearchDirectory(basedirectory, "*");

			foreach (string file in collection) {
				string extension = Path.GetExtension(file).ToUpper();

				if (!IsFileInProject(file) &&
				     extension != ".SCC" &&  // source safe control files -- Svante Lidmans
				     extension != ".DLL" &&
				     extension != ".PDB" &&
				     extension != ".EXE" &&
				     extension != ".CMBX" &&
				     extension != ".PRJX" &&
				     !Path.GetDirectoryName(file).EndsWith("CVS") &&
				     !Path.GetDirectoryName(file).EndsWith(".svn") &&
				     !Path.GetDirectoryName(file).EndsWith("ProjectDocumentation")) {

					newFiles.Add(file);
				}
			}
			
			if (newFiles.Count > 0) {
				if (newFileSearch == NewFileSearch.OnLoadAutoInsert) {
					foreach (string file in newFiles) {
						ProjectFile newFile = new ProjectFile(file);
						newFile.BuildAction = IsCompileable(file) ? BuildAction.Compile : BuildAction.Nothing;
						projectFiles.Add(newFile);
					}
				} else {
#if !LINUX
					new IncludeFilesDialog(this, newFiles).ShowDialog();
#endif
				}
			}
		}
		
		public virtual void LoadProject(string fileName)
		{
			basedirectory = Path.GetDirectoryName(fileName);
			XmlDocument doc = new XmlDocument();
			doc.Load(fileName);
			
			string version = null;
			if (doc.DocumentElement.Attributes["version"] == null) {
				if (doc.DocumentElement.Attributes["fileversion"] != null) {
					version = doc.DocumentElement.Attributes["fileversion"].InnerText;
				}
			} else {
				version = doc.DocumentElement.Attributes["version"].InnerText;
			}
			
			if (version != "1.0" && version != currentProjectFileVersion) {
				throw new UnknownProjectVersionException(version);
			}
			if (version == "1.0") {
				string tempFile = Path.GetTempFileName();
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowMessage("Old project file format found.\n It will be automatically converted to " + currentProjectFileVersion,
				                           "Information");
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				
				ConvertXml.Convert(fileName,
				                   propertyService.DataDirectory + Path.DirectorySeparatorChar +
				                   "ConversionStyleSheets" + Path.DirectorySeparatorChar +
				                   "ConvertPrjx10to11.xsl",
				                   tempFile);
				try {
					File.Delete(fileName);
					File.Copy(tempFile, fileName);
					LoadProject(fileName);
					File.Delete(tempFile);
					return;
				} catch (Exception) {
					messageService.ShowError("Error writing the old project file.\nCheck if you have write permission on the project file (.prjx).\n A non persistent proxy project will be created but no changes will be saved.\nIt is better if you close SharpDevelop and correct the problem.");
					if (File.Exists(tempFile)) {
						doc.Load(tempFile);
						File.Delete(tempFile);
					} else {
						messageService.ShowError("damn! (should never happen)");
					}
				}
			}
			
			GetXmlAttributes(doc, doc.DocumentElement, this);
			
			// add the configurations
			XmlNode configurationElement = doc.DocumentElement.SelectSingleNode("Configurations");
			
			string activeConfigurationName = configurationElement.Attributes["active"].InnerText;
			
			foreach (XmlNode configuration in configurationElement.ChildNodes) {
				if (configuration.Name == configurationNodeName) {
					IConfiguration newConfiguration = CreateConfiguration();
					GetXmlAttributes(doc, (XmlElement)configuration, newConfiguration);
					if (newConfiguration.Name == activeConfigurationName) {
						activeConfiguration = newConfiguration;
					}
					Configurations.Add(newConfiguration);
				}
			}
			
			SearchNewFiles();
		}

		void GetXmlAttributes(XmlDocument doc, XmlElement element, object o)
		{
			FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.FlattenHierarchy |
			                                               BindingFlags.Public           |
			                                               BindingFlags.Instance         |
			                                               BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fieldInfos) {
				// set the xml attributes for this object
				XmlAttributeAttribute[]           xmlAttributes = (XmlAttributeAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
				ConvertToRelativePathAttribute[]  convertToRelPath = (ConvertToRelativePathAttribute[])fieldInfo.GetCustomAttributes(typeof(ConvertToRelativePathAttribute), true);
				bool convertRel = convertToRelPath != null && convertToRelPath.Length > 0;
				
				if (xmlAttributes != null && xmlAttributes.Length > 0) {
					XmlAttribute xmlAttribute = element.Attributes[xmlAttributes[0].Name];
					if (xmlAttribute != null) {
						if (convertRel && convertToRelPath[0].PredicatePropertyName != null && convertToRelPath[0].PredicatePropertyName.Length > 0) {
							PropertyInfo myPropInfo = o.GetType().GetProperty(convertToRelPath[0].PredicatePropertyName, 
							                                                          BindingFlags.Public |
							                                                          BindingFlags.NonPublic |
							                                                          BindingFlags.Instance);
							if (myPropInfo != null) {
								convertRel = (bool)myPropInfo.GetValue(o, null);
							}
						}
						
						string val = null;
						if (convertRel) {
							if (xmlAttribute.InnerText.Length == 0) {
								val = String.Empty;
							} else {
								val = fileUtilityService.RelativeToAbsolutePath(basedirectory, xmlAttribute.InnerText);
							}
						} else {
							val = xmlAttribute.InnerText;
						}
						
						if (fieldInfo.FieldType.IsEnum) {
							fieldInfo.SetValue(o, Enum.Parse(fieldInfo.FieldType,val));
						} else {
							fieldInfo.SetValue(o, Convert.ChangeType(val, fieldInfo.FieldType));
						}
					}
				} else { // add sets to the xmlElement
					XmlSetAttribute[] xmlSetAttributes = (XmlSetAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlSetAttribute), true);
					if (xmlSetAttributes != null && xmlSetAttributes.Length > 0) {
						XmlElement setElement;
						if (xmlSetAttributes[0].Name == null) {
							setElement = element;
						} else {
							setElement = (XmlElement)element.SelectSingleNode("descendant::" + xmlSetAttributes[0].Name);
						}
						
						IList collection = (IList)fieldInfo.GetValue(o);
						foreach (XmlNode childNode in setElement.ChildNodes) {
							object instance = xmlSetAttributes[0].Type.Assembly.CreateInstance(xmlSetAttributes[0].Type.FullName);
							GetXmlAttributes(doc, (XmlElement)childNode, instance);
							collection.Add(instance);
						}
					} else { // finally try, if the field is from a type which has a XmlNodeName attribute attached
						
						XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])fieldInfo.FieldType.GetCustomAttributes(typeof(XmlNodeNameAttribute), true);
						
						if (xmlNodeNames != null && xmlNodeNames.Length == 1) {
							XmlElement el = (XmlElement)element.SelectSingleNode("descendant::" + xmlNodeNames[0].Name);
							object instance = fieldInfo.FieldType.Assembly.CreateInstance(fieldInfo.FieldType.FullName);
							if (el != null) {
								GetXmlAttributes(doc, el, instance);
							}
							fieldInfo.SetValue(o, instance);
						}
					}
				}
			}
		}
		
		void SetXmlAttributes(XmlDocument doc, XmlElement element, object o)
		{
			FieldInfo[] fieldInfos = o.GetType().GetFields(BindingFlags.FlattenHierarchy |
			                                               BindingFlags.Public           |
			                                               BindingFlags.Instance         |
			                                               BindingFlags.NonPublic);
			foreach (FieldInfo fieldInfo in fieldInfos) {
				// set the xml attributes for this object
				XmlAttributeAttribute[] xmlAttributes = (XmlAttributeAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlAttributeAttribute), true);
				
				ConvertToRelativePathAttribute[]  convertToRelPath = (ConvertToRelativePathAttribute[])fieldInfo.GetCustomAttributes(typeof(ConvertToRelativePathAttribute), true);
				bool convertRel = convertToRelPath != null && convertToRelPath.Length > 0;
								
				if (xmlAttributes != null && xmlAttributes.Length > 0) {
					XmlAttribute xmlAttribute = doc.CreateAttribute(xmlAttributes[0].Name);
					object fieldValue = fieldInfo.GetValue(o);
					
					if (convertRel && convertToRelPath[0].PredicatePropertyName != null && convertToRelPath[0].PredicatePropertyName.Length > 0) {
						PropertyInfo myPropInfo = o.GetType().GetProperty(convertToRelPath[0].PredicatePropertyName,
						                                                          BindingFlags.Public |
						                                                          BindingFlags.NonPublic |
						                                                          BindingFlags.Instance);
						if (myPropInfo != null) {
							convertRel = (bool)myPropInfo.GetValue(o, null);
						}
					}
					
					if (convertRel) {
						string val = fieldValue == null ? String.Empty : fieldValue.ToString();
						if (val.Length == 0) {
							fieldValue = String.Empty;
						} else {
							fieldValue = fileUtilityService.AbsoluteToRelativePath(basedirectory, val);
						}
					}
					xmlAttribute.InnerText = fieldValue == null ? String.Empty : fieldValue.ToString();
					element.Attributes.Append(xmlAttribute);
				} else { // add sets to the xmlElement
					XmlSetAttribute[] xmlSetAttributes = (XmlSetAttribute[])fieldInfo.GetCustomAttributes(typeof(XmlSetAttribute), true);
					if (xmlSetAttributes != null && xmlSetAttributes.Length > 0) {
						XmlElement setElement;
						if (xmlSetAttributes[0].Name == null) {
							setElement = element;
						} else {
							setElement = doc.CreateElement(xmlSetAttributes[0].Name);
						}

						// A set must always be a collection
						ICollection collection = (ICollection)fieldInfo.GetValue(o);
						foreach (object collectionObject in collection) {
							XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])collectionObject.GetType().GetCustomAttributes(typeof(XmlNodeNameAttribute), true);
							if (xmlNodeNames == null || xmlNodeNames.Length != 1) {
								throw new Exception("XmlNodeNames mismatch");
							}
							XmlElement collectionElement = doc.CreateElement(xmlNodeNames[0].Name);
							SetXmlAttributes(doc, collectionElement, collectionObject);
							setElement.AppendChild(collectionElement);
						}
						if (element != setElement) {
							element.AppendChild(setElement);
						}
					} else { // finally try, if the field is from a type which has a XmlNodeName attribute attached
						object fieldValue = fieldInfo.GetValue(o);
						if (fieldValue != null) {
							XmlNodeNameAttribute[] xmlNodeNames = (XmlNodeNameAttribute[])fieldValue.GetType().GetCustomAttributes(typeof(XmlNodeNameAttribute), true);

							if (xmlNodeNames != null && xmlNodeNames.Length == 1) {
								XmlElement setElement = doc.CreateElement(xmlNodeNames[0].Name);
								SetXmlAttributes(doc, setElement, fieldValue);
								element.AppendChild(setElement);
							}
						}
					}
				}
			}
		}
		
		public virtual void SaveProject(string fileName)
		{
			basedirectory = Path.GetDirectoryName(fileName);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml("<Project/>");

			SetXmlAttributes(doc, doc.DocumentElement, this);
			
			// set version attribute to the root node
			XmlAttribute versionAttribute = doc.CreateAttribute("version");
			versionAttribute.InnerText    = currentProjectFileVersion;
			doc.DocumentElement.Attributes.Append(versionAttribute);
			
			
			// set projecttype attribute to the root node
			XmlAttribute projecttypeAttribute = doc.CreateAttribute("projecttype");
			projecttypeAttribute.InnerText    = ProjectType;
			doc.DocumentElement.Attributes.Append(projecttypeAttribute);
			
			// create the configuration nodes
			// I choosed to add the configuration nodes 'per hand' instead of using the automated
			// version, because it is more cleaner for the language binding implementors to just 
			// creating a factory method for their configurations.
			XmlElement configurationElement = doc.CreateElement("Configurations");
			XmlAttribute activeConfigAttribute = doc.CreateAttribute("active");
			activeConfigAttribute.InnerText = ActiveConfiguration == null ? String.Empty : ActiveConfiguration.Name;
			configurationElement.Attributes.Append(activeConfigAttribute);
			
			foreach (IConfiguration configuration in Configurations) {
				XmlElement newConfig = doc.CreateElement(configurationNodeName);
				SetXmlAttributes(doc, newConfig, configuration);
				configurationElement.AppendChild(newConfig);
			}
			
			doc.DocumentElement.AppendChild(configurationElement);
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			fileUtilityService.ObservedSave(new NamedFileOperationDelegate(doc.Save), 
			                                fileName, 
			                                resourceService.GetString("Internal.Project.Project.CantSaveProjectErrorText"),
			                                FileErrorPolicy.ProvideAlternative);
		}
		
		public virtual string GetParseableFileContent(string fileName)
		{
			StreamReader sr = File.OpenText(fileName);
			string content = sr.ReadToEnd();
			sr.Close();
			return content;
		}
		
		public void SaveProjectAs()
		{
			SaveFileDialog fdiag = new SaveFileDialog();
			fdiag.OverwritePrompt = true;
			fdiag.AddExtension    = true;

			fdiag.Filter          = String.Join("|", (string[])(AddInTreeSingleton.AddInTree.GetTreeNode("/SharpDevelop/Workbench/Combine/FileFilter").BuildChildItems(this)).ToArray(typeof(string)));

			if (fdiag.ShowDialog() == DialogResult.OK) {
				string filename = fdiag.FileName;
				SaveProject(filename);
				IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowMessage(filename, resourceService.GetString("Internal.Project.DefaultProject.ProjectSavedMessage"));
			}
		}
		
		public void CopyReferencesToOutputPath(bool force)
		{
			AbstractProjectConfiguration config = ActiveConfiguration as AbstractProjectConfiguration;
			if (config == null) {
				return;
			}
			foreach (ProjectReference projectReference in ProjectReferences) {
				if ((projectReference.LocalCopy || force) && projectReference.ReferenceType != ReferenceType.Gac) {
					string referenceFileName   = projectReference.GetReferencedFileName(this);
					string destinationFileName = fileUtilityService.GetDirectoryNameWithSeparator(config.OutputDirectory) + Path.GetFileName(referenceFileName);
					try {
						if (destinationFileName != referenceFileName) {
							File.Copy(referenceFileName, destinationFileName, true);
						}
					} catch (Exception e) {
						Console.WriteLine("Can't copy reference file from {0} to {1} reason {2}", referenceFileName, destinationFileName, e);
					}
				}
			}
		}
		
		public virtual void Dispose()
		{
		}
		
		public abstract IConfiguration CreateConfiguration();
		
		public virtual  IConfiguration CreateConfiguration(string name)
		{
			IConfiguration config = CreateConfiguration();
			config.Name = name;
			
			return config;
		}
		
		protected virtual void OnNameChanged(EventArgs e)
		{
			if (NameChanged != null) {
				NameChanged(this, e);
			}
		}
		
		public event EventHandler NameChanged;
	}
	
	public class ProjectActiveConfigurationTypeConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context,Type sourceType)
		{
			return true;
		}
		
		public override  bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return true;
		}
		
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture,  object value)
		{
			IProject project = (IProject)context.Instance;
			foreach (IConfiguration configuration in project.Configurations) {
				if (configuration.Name == value.ToString()) {
					return configuration;
				}
			}
			return null;
		}
		
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
		{
			IConfiguration config = value as IConfiguration;
			Debug.Assert(config != null, String.Format("Tried to convert {0} to IConfiguration", config));
			if (config != null) {
				return config.Name;
			}
			return String.Empty;
		}
		
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return true;
		}
		
		public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
		{
			return new TypeConverter.StandardValuesCollection(((IProject)context.Instance).Configurations);
		}
	}
}
