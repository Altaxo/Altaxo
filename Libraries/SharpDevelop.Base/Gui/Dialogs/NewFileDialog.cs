// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.Resources;
using System.Windows.Forms;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.CodeDom.Compiler;

using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core.Properties;

using ICSharpCode.Core.AddIns;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	///  This class is for creating a new "empty" file
	/// </summar
	public class NewFileDialog : BaseSharpDevelopForm, INewFileCreator
	{
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		bool allowUntitledFiles;
		string basePath;
		StringCollection createdFiles = new StringCollection();
		
		public StringCollection CreatedFiles {
			get {
				return createdFiles;
			}
		}
		
		public NewFileDialog(string basePath)
		{
			StandardHeader.SetHeaders();
			this.basePath = basePath;
			this.allowUntitledFiles = basePath == null;
			try {
				InitializeComponents();
				InitializeTemplates();
				InitializeView();
				
				((TreeView)ControlDictionary["categoryTreeView"]).Select();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}
		
		void InitializeView()
		{
			ImageList smalllist  = new ImageList();
			ImageList imglist    = new ImageList();
			smalllist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.ColorDepth   = ColorDepth.Depth32Bit;
			
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(IconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			imglist.Images.Add(IconService.GetBitmap("Icons.32x32.EmptyFileIcon"));
			
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			IconService iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
			foreach (DictionaryEntry entry in icons) {
				Bitmap bitmap = iconService.GetBitmap(entry.Key.ToString());
				if (bitmap != null) {
					smalllist.Images.Add(bitmap);
					imglist.Images.Add(bitmap);
					tmp[entry.Key] = ++i;
				} else {
					Console.WriteLine("can't load bitmap " + entry.Key.ToString() + " using default");
				}
			}
			
			icons = tmp;
			foreach (TemplateItem item in alltemplates) {
				if (item.Template.Icon == null) {
					item.ImageIndex = 0;
				} else {
					item.ImageIndex = (int)icons[item.Template.Icon];
				}
			}
			
			((ListView)ControlDictionary["templateListView"]).LargeImageList = imglist;
			((ListView)ControlDictionary["templateListView"]).SmallImageList = smalllist;
			
			InsertCategories(null, categories);
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			for (int j = 0; j < categories.Count; ++j) {
				if (((Category)categories[j]).Name == propertyService.GetProperty("Dialogs.NewFileDialog.LastSelectedCategory", "C#")) {
					((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode = (TreeNode)((TreeView)ControlDictionary["categoryTreeView"]).Nodes[j];
					break;
				}
			}
		}
		
		void InsertCategories(TreeNode node, ArrayList catarray)
		{
			foreach (Category cat in catarray) {
				if (node == null) {
					((TreeView)ControlDictionary["categoryTreeView"]).Nodes.Add(cat);
				} else {
					node.Nodes.Add(cat);
				}
				InsertCategories(cat, cat.Categories);
			}
		}
		
		// TODO : insert sub categories
		Category GetCategory(string categoryname)
		{
			foreach (Category category in categories) {
				if (category.Name == categoryname) {
					return category;
				}
			}
			Category newcategory = new Category(categoryname);
			categories.Add(newcategory);
			return newcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (FileTemplate template in FileTemplate.FileTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null) {
					icons[titem.Template.Icon] = 0; // "create template icon"
				}
				if (template.NewFileDialogVisible == true) {
					Category cat = GetCategory(titem.Template.Category);
					cat.Templates.Add(titem); 
				
					if (cat.Selected == false && template.WizardPath == null) {
						cat.Selected = true;
					}
					if (!cat.HasSelectedTemplate && titem.Template.FileDescriptionTemplates.Count == 1) {
						if (((FileDescriptionTemplate)titem.Template.FileDescriptionTemplates[0]).Name.StartsWith("Empty")) {
							titem.Selected = true;
							cat.HasSelectedTemplate = true;
						}
					}
				}
				alltemplates.Add(titem);
			}
		}
		
		// tree view event handlers
		void CategoryChange(object sender, TreeViewEventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).Items.Clear();
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				foreach (TemplateItem item in ((Category)((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode).Templates) {
					((ListView)ControlDictionary["templateListView"]).Items.Add(item);
				}
			}
		}
		
		void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 0;
		}
		
		const int GridWidth = 256;
		const int GridMargin = 8;
		PropertyGrid propertyGrid = new PropertyGrid();
		LocalizedTypeDescriptor localizedTypeDescriptor = null;
		
		bool AllPropertiesHaveAValue {
			get {
				foreach (TemplateProperty property in SelectedTemplate.Properties) {
					string val = StringParserService.Properties["Properties." + property.Name];
					if (val == null || val.Length == 0) {
						return false;
					}
				}
				return true;
			}
		}
		
		void ShowPropertyGrid()
		{
			if (localizedTypeDescriptor == null) {
				localizedTypeDescriptor = new LocalizedTypeDescriptor();
			}
				
			if (!Controls.Contains(propertyGrid)) {
				this.SuspendLayout();
				propertyGrid.Location = new Point(Width - GridMargin, GridMargin);
				localizedTypeDescriptor.Properties.Clear();
				foreach (TemplateProperty property in SelectedTemplate.Properties) {
					LocalizedProperty localizedProperty;
					if (property.Type.StartsWith("Types:")) {
						localizedProperty = new LocalizedProperty(property.Name, "System.Enum", property.Category, property.Description);
						TemplateType type = null;
						foreach (TemplateType templateType in SelectedTemplate.CustomTypes) {
							if (templateType.Name == property.Type.Substring("Types:".Length)) {
								type = templateType;
								break;
							}
						}
						if (type == null) {
							throw new Exception("type : " + property.Type + " not found.");
						}
						localizedProperty.TypeConverterObject = new CustomTypeConverter(type);
						StringParserService.Properties["Properties." + localizedProperty.Name] = property.DefaultValue;
						localizedProperty.DefaultValue = property.DefaultValue; // localizedProperty.TypeConverterObject.ConvertFrom();
					} else {
						localizedProperty = new LocalizedProperty(property.Name, property.Type, property.Category, property.Description);
						if (property.Type == "System.Boolean") {
							localizedProperty.TypeConverterObject = new BooleanTypeConverter();
							string defVal = property.DefaultValue == null ? null : property.DefaultValue.ToString();
							if (defVal == null || defVal.Length == 0) {
								defVal = "True";
							}
							StringParserService.Properties["Properties." + localizedProperty.Name] = defVal;
							localizedProperty.DefaultValue = Boolean.Parse(defVal);
						}
					}
					localizedProperty.LocalizedName = property.LocalizedName;
					localizedTypeDescriptor.Properties.Add(localizedProperty);
				}
				propertyGrid.ToolbarVisible = false;
				propertyGrid.SelectedObject = localizedTypeDescriptor;
				propertyGrid.Size     = new Size(GridWidth, Height - GridMargin * 4);
				
				Width = Width + GridWidth;
				Controls.Add(propertyGrid);
				this.ResumeLayout(false);
			}
		}
		
		void HidePropertyGrid()
		{
			if (Controls.Contains(propertyGrid)) {
				this.SuspendLayout();
				Controls.Remove(propertyGrid);
				Width = Width - GridWidth;
				this.ResumeLayout(false);
			}
		}
		
		FileTemplate SelectedTemplate {
			get {
				if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
					return ((TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0]).Template;
				}
				return null;
			}
		}
		string GenerateCurrentFileName()
		{
			if (SelectedTemplate.DefaultName.IndexOf("${Number}") >= 0) {
				try {
					int curNumber = 1;
					IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
					while (true) {
						StringParserService.Properties["Number"] = curNumber.ToString();
						string fileName = StringParserService.Parse(SelectedTemplate.DefaultName);
						if (allowUntitledFiles) {
							if (!fileService.IsOpen(fileName)) {
								break;
							}
						} else if (!File.Exists(Path.Combine(basePath, fileName))) {
							break;
						}
						++curNumber;
					}
				} catch (Exception e) {
					Console.WriteLine(e);
				}
			}
			return StringParserService.Parse(SelectedTemplate.DefaultName);
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				ControlDictionary["descriptionLabel"].Text = StringParserService.Parse(SelectedTemplate.Description);
				ControlDictionary["openButton"].Enabled = true;
				if (SelectedTemplate.HasProperties) {
					ShowPropertyGrid();
				}
				if (!this.allowUntitledFiles) {
					ControlDictionary["fileNameTextBox"].Text = GenerateCurrentFileName();
				}
			} else {
				ControlDictionary["descriptionLabel"].Text = String.Empty;
				ControlDictionary["openButton"].Enabled = false;
				HidePropertyGrid();
			}
		}
		
		// button events
		
		void CheckedChange(object sender, EventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(string filename, string content, string languageName, bool showFile)
		{
			IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
			string parsedFileName = StringParserService.Parse(filename);
			createdFiles.Add(parsedFileName);
			fileService.NewFile(parsedFileName, StringParserService.Parse(languageName), StringParserService.Parse(content));
			DialogResult = DialogResult.OK;
		}
		
		string GenerateValidClassName(string className)
		{
			int idx = 0;
			while (idx < className.Length && className[idx] != '_' && !Char.IsLetter(className[idx])) {
				++idx;
			}
			StringBuilder nameBuilder = new StringBuilder();
			for (; idx < className.Length; ++idx) {
				if (Char.IsLetterOrDigit(className[idx]) || className[idx] == '_') {
					nameBuilder.Append(className[idx]);
				}
				if (className[idx] == ' ' || className[idx] == '-' ) {
					nameBuilder.Append('_');
				}
			}
			
			return nameBuilder.ToString();
		}
		
		void OpenEvent(object sender, EventArgs e)
		{
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
				propertyService.SetProperty("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
				propertyService.SetProperty("Dialogs.NewFileDialog.LastSelectedCategory", ((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode.Text);
			}
			createdFiles.Clear();
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				if (!AllPropertiesHaveAValue) {
					IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
					messageService.ShowMessage("${res:Dialog.NewFile.FillOutFirstMessage}", "${res:Dialog.NewFile.FillOutFirstCaption}");
					return;
				}
				TemplateItem item = (TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0];
				string fileName;
				StringParserService.Properties["StandardNamespace"] = "DefaultNamespace";
				if (allowUntitledFiles) {
					fileName = GenerateCurrentFileName();
				} else {
					IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
					if (projectService.CurrentSelectedProject != null) {
						StringParserService.Properties["StandardNamespace"] = projectService.CurrentSelectedProject.StandardNamespace;
					}
					fileName = ControlDictionary["fileNameTextBox"].Text;
				}
				
				StringParserService.Properties["FullName"]                 = fileName;
				StringParserService.Properties["FileName"]                 = Path.GetFileName(fileName);
				StringParserService.Properties["FileNameWithoutExtension"] = Path.GetFileNameWithoutExtension(fileName);
				StringParserService.Properties["Extension"]                = Path.GetExtension(fileName);
				StringParserService.Properties["Path"]                     = Path.GetDirectoryName(fileName);
				
				StringParserService.Properties["ClassName"] = GenerateValidClassName(Path.GetFileNameWithoutExtension(fileName));
				
				
				if (item.Template.WizardPath != null) {
					IProperties customizer = new DefaultProperties();
					customizer.SetProperty("Template", item.Template);
					customizer.SetProperty("Creator",  this);
					WizardDialog wizard = new WizardDialog("File Wizard", customizer, item.Template.WizardPath);
					if (wizard.ShowDialog() == DialogResult.OK) {
						DialogResult = DialogResult.OK;
					}
				} else {
					ScriptRunner scriptRunner = new ScriptRunner();
					foreach (FileDescriptionTemplate newfile in item.Template.FileDescriptionTemplates) {
						SaveFile(newfile.Name, scriptRunner.CompileScript(item.Template, newfile), newfile.Language, true);
					}
					DialogResult = DialogResult.OK;
				}
			}
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category : TreeNode
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			public bool Selected = false;
			public bool HasSelectedTemplate = false;
			static StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			public Category(string name) : base(stringParserService.Parse(name))
			{
				this.name = name;
				ImageIndex = 1;
			}
			
			public string Name {
				get {
					return name;
				}
			}
			public ArrayList Categories {
				get {
					return categories;
				}
			}
			public ArrayList Templates {
				get {
					return templates;
				}
			}
		}
		
		/// <summary>
		///  Represents a new file template
		/// </summary>
		class TemplateItem : ListViewItem
		{
			FileTemplate template;
			
			public TemplateItem(FileTemplate template) : base(((StringParserService)ServiceManager.Services.GetService(typeof(StringParserService))).Parse(template.Name))
			{
				this.template = template;
				ImageIndex    = 0;
			}
			
			public FileTemplate Template {
				get {
					return template;
				}
			}
		}
		
		void InitializeComponents()
		{
			if (allowUntitledFiles) {
				base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\NewFileDialog.xfrm"));
			} else {
				base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\NewFileWithNameDialog.xfrm"));
			}
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			((TreeView)ControlDictionary["categoryTreeView"]).ImageList = imglist;
		
			((TreeView)ControlDictionary["categoryTreeView"]).AfterSelect    += new TreeViewEventHandler(CategoryChange);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			
			((ListView)ControlDictionary["templateListView"]).SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			((ListView)ControlDictionary["templateListView"]).DoubleClick          += new EventHandler(OpenEvent);
			
			ControlDictionary["openButton"].Click += new EventHandler(OpenEvent);
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked = propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked = !propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).CheckedChanged += new EventHandler(CheckedChange);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(ControlDictionary["largeIconsRadioButton"], StringParserService.Parse("${res:Global.LargeIconToolTip}"));
			tooltip.SetToolTip(ControlDictionary["smallIconsRadioButton"], StringParserService.Parse("${res:Global.SmallIconToolTip}"));
			tooltip.Active = true;
			Owner         = (Form)WorkbenchSingleton.Workbench;
			StartPosition = FormStartPosition.CenterParent;
			Icon          = null;
			
			CheckedChange(this, EventArgs.Empty);
		}
	}
}
