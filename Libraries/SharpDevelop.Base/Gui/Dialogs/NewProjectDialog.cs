// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;

using ICSharpCode.Core.AddIns;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui.XmlForms;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs
{
	/// <summary>
	/// This class displays a new project dialog and sets up and creates a a new project,
	/// the project types are described in an XML options file
	/// </summary>
	public class NewProjectDialog : BaseSharpDevelopForm
	{
		Container components = new System.ComponentModel.Container();
		
		ArrayList alltemplates = new ArrayList();
		ArrayList categories   = new ArrayList();
		Hashtable icons        = new Hashtable();
		
		ResourceService     resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
		FileUtilityService  fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
		StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
		PropertyService     propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
		IconService         iconService = (IconService)ServiceManager.Services.GetService(typeof(IconService));
		bool openCombine;
		
		public NewProjectDialog(bool openCombine)
		{
			this.openCombine = openCombine;
			InitializeComponents();
			
			InitializeTemplates();
			InitializeView();
			
			((TreeView)ControlDictionary["categoryTreeView"]).Select();
			((TextBox)ControlDictionary["locationTextBox"]).Text = propertyService.GetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.DefaultPath", fileUtilityService.GetDirectoryNameWithSeparator(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal)) + "SharpDevelop Projects").ToString();
			StartPosition = FormStartPosition.CenterParent;
			Icon = null;
		}
		
		void InitializeView()
		{
			ImageList smalllist = new ImageList();
			ImageList imglist = new ImageList();
			
			smalllist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ColorDepth   = ColorDepth.Depth32Bit;
			imglist.ImageSize    = new Size(32, 32);
			smalllist.ImageSize  = new Size(16, 16);
			
			smalllist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			imglist.Images.Add(resourceService.GetBitmap("Icons.32x32.EmptyProjectIcon"));
			
			// load the icons and set their index from the image list in the hashtable
			int i = 0;
			Hashtable tmp = new Hashtable(icons);
			
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
			
			// set the correct imageindex for all templates
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
			for (int j = 0; j < categories.Count; ++j) {
				if (((Category)categories[j]).Name == propertyService.GetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", "C#")) {
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
				if (category.Text == categoryname)
					return category;
			}
			Category newcategory = new Category(categoryname);
			categories.Add(newcategory);
			return newcategory;
		}
		
		void InitializeTemplates()
		{
			foreach (ProjectTemplate template in ProjectTemplate.ProjectTemplates) {
				TemplateItem titem = new TemplateItem(template);
				if (titem.Template.Icon != null)
					icons[titem.Template.Icon] = 0; // "create template icon"
				Category cat = GetCategory(titem.Template.Category);
				cat.Templates.Add(titem);
				if (cat.Templates.Count == 1)
					titem.Selected = true;
				alltemplates.Add(titem);
			}
		}
		
		void CategoryChange(object sender, TreeViewEventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).Items.Clear();
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				foreach (TemplateItem item in ((Category)((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode).Templates) {
					((ListView)ControlDictionary["templateListView"]).Items.Add(item);
				}
			}
			this.SelectedIndexChange(sender, e);
		}
		
		void OnBeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 1;
		}
		
		void OnBeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.ImageIndex = 0;
		}
		
		void CheckedChange(object sender, EventArgs e)
		{
			((TextBox)ControlDictionary["solutionNameTextBox"]).ReadOnly = !((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked;
			
			if (((TextBox)ControlDictionary["solutionNameTextBox"]).ReadOnly) { // unchecked created own directory for solution
				NameTextChanged(null, null);    // set the value of the ((TextBox)ControlDictionary["solutionNameTextBox"]) to ((TextBox)ControlDictionary["nameTextBox"])
			}
		}
		
		void NameTextChanged(object sender, EventArgs e)
		{
			if (!((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked) {
				((TextBox)ControlDictionary["solutionNameTextBox"]).Text = ((TextBox)ControlDictionary["nameTextBox"]).Text;
			}
		}
		
		string ProjectSolution {
			get {
				string name = String.Empty;
				if (((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).Checked) {
					name += Path.DirectorySeparatorChar + ((TextBox)ControlDictionary["solutionNameTextBox"]).Text;
				}
				return ProjectLocation + name;
			}
		}
		
		string ProjectLocation {
			get {
				string location = ((TextBox)ControlDictionary["locationTextBox"]).Text.TrimEnd('\\', '/', Path.DirectorySeparatorChar);
				string name     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
				return location + (((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).Checked ? Path.DirectorySeparatorChar + name : "");
			}
		}
		
		// TODO : Format the text
		void PathChanged(object sender, EventArgs e)
		{
			ControlDictionary["createInLabel"].Text = resourceService.GetString("Dialog.NewProject.ProjectAtDescription")+ " " + ProjectSolution;
		}
		
		void IconSizeChange(object sender, EventArgs e)
		{
			((ListView)ControlDictionary["templateListView"]).View = ((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked ? View.List : View.LargeIcon;
		}
		
		public bool IsFilenameAvailable(string fileName)
		{
			return true;
		}
		
		public void SaveFile(IProject project, string filename, string content, bool showFile)
		{
			project.ProjectFiles.Add(new ProjectFile(filename));
			
			StreamWriter sr = File.CreateText(filename);
			sr.Write(stringParserService.Parse(content, new string[,] { {"PROJECT", ((TextBox)ControlDictionary["nameTextBox"]).Text}, {"FILE", Path.GetFileName(filename)}}));
			sr.Close();
			
			if (showFile) {
				string longfilename = fileUtilityService.GetDirectoryNameWithSeparator(ProjectSolution) + stringParserService.Parse(filename, new string[,] { {"PROJECT", ((TextBox)ControlDictionary["nameTextBox"]).Text}});
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(longfilename);
			}
		}
		
		public string NewProjectLocation;
		public string NewCombineLocation;
		
		void OpenEvent(object sender, EventArgs e)
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			if (((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode != null) {
				propertyService.SetProperty("Dialogs.NewProjectDialog.LastSelectedCategory", ((TreeView)ControlDictionary["categoryTreeView"]).SelectedNode.Text);
				propertyService.SetProperty("Dialogs.NewProjectDialog.LargeImages", ((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked);
			}
			
			FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
			string solution = ((TextBox)ControlDictionary["solutionNameTextBox"]).Text;
			string name     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
			string location = ((TextBox)ControlDictionary["locationTextBox"]).Text;
			if (!fileUtilityService.IsValidFileName(solution) || solution.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(name)     || name.IndexOf(Path.DirectorySeparatorChar) >= 0 ||
			    !fileUtilityService.IsValidFileName(location)) {
				MessageService.ShowError("Illegal project name.\nOnly use letters, digits, space, '.' or '_'.");
				return;
			}
			
			propertyService.SetProperty("ICSharpCode.SharpDevelop.Gui.Dialogs.NewProjectDialog.AutoCreateProjectSubdir", ((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).Checked);
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1 && ((TextBox)ControlDictionary["locationTextBox"]).Text.Length > 0 && ((TextBox)ControlDictionary["solutionNameTextBox"]).Text.Length > 0) {
					TemplateItem item = (TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0];
					
					System.IO.Directory.CreateDirectory(ProjectSolution);
					
					
					ProjectCreateInformation cinfo = new ProjectCreateInformation();
					
					cinfo.CombinePath     = ProjectLocation;
					cinfo.ProjectBasePath = ProjectSolution;
//					cinfo.Description     = stringParserService.Parse(item.Template.Description);
					
					cinfo.ProjectName     = ((TextBox)ControlDictionary["nameTextBox"]).Text;
//					cinfo.ProjectTemplate = item.Template;
					
					NewCombineLocation = item.Template.CreateProject(cinfo);
					if (NewCombineLocation == null || NewCombineLocation.Length == 0) {
						return;
					}
					if (openCombine) {
						item.Template.OpenCreatedCombine();
					}
					
					// TODO :: THIS DOESN'T WORK !!!
					NewProjectLocation = Path.ChangeExtension(NewCombineLocation, ".prjx");
					
					DialogResult = DialogResult.OK;
					/*
					if (item.Template.LanguageName != null && item.Template.LanguageName.Length > 0)  {
						
					}
					
					if (item.Template.WizardPath != null) {
						IProperties customizer = new DefaultProperties();
						customizer.SetProperty("Template", item.Template);
						customizer.SetProperty("Creator",  this);
						WizardDialog wizard = new WizardDialog("Project Wizard", customizer, item.Template.WizardPath);
						if (wizard.ShowDialog() == DialogResult.OK) {
							DialogResult = DialogResult.OK;
						}
					}
					
					NewCombineLocation = fileUtilityService.GetDirectoryNameWithSeparator(ProjectLocation) + ((TextBox)ControlDictionary["nameTextBox"]).Text + ".cmbx";
					
					if (File.Exists(NewCombineLocation)) {
						DialogResult result = MessageBox.Show("Combine file " + NewCombineLocation + " already exists, do you want to overwrite\nthe existing file ?", "File already exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
						switch(result) {
							case DialogResult.Yes:
								cmb.SaveCombine(NewCombineLocation);
								break;
							case DialogResult.No:
								break;
						}
					} else {
						cmb.SaveCombine(NewCombineLocation);
					}
				} else {
					MessageBox.Show(resourceService.GetString("Dialog.NewProject.EmptyProjectFieldWarning"), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
					*/
			}
		}
		
		void BrowseDirectories(object sender, EventArgs e)
		{
			// Changes Shankar
			FolderDialog fd = new FolderDialog();
			if (fd.DisplayDialog() == DialogResult.OK) {
				((TextBox)ControlDictionary["locationTextBox"]).Text = fd.Path;
			}
			// End
		}
		
		// list view event handlers
		void SelectedIndexChange(object sender, EventArgs e)
		{
			if (((ListView)ControlDictionary["templateListView"]).SelectedItems.Count == 1) {
				ControlDictionary["descriptionLabel"].Text = stringParserService.Parse(((TemplateItem)((ListView)ControlDictionary["templateListView"]).SelectedItems[0]).Template.Description);
				ControlDictionary["openButton"].Enabled = true;
			} else {
				ControlDictionary["descriptionLabel"].Text = String.Empty;
				ControlDictionary["openButton"].Enabled = false;
			}
		}
		
		void InitializeComponents()
		{
			base.SetupFromXml(Path.Combine(PropertyService.DataDirectory, @"resources\dialogs\NewProjectDialog.xfrm"));
			
			ImageList imglist = new ImageList();
			imglist.ColorDepth = ColorDepth.Depth32Bit;
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.OpenFolderBitmap"));
			imglist.Images.Add(IconService.GetBitmap("Icons.16x16.ClosedFolderBitmap"));
			((TreeView)ControlDictionary["categoryTreeView"]).ImageList = imglist;
			
			((ListView)ControlDictionary["templateListView"]).DoubleClick += new EventHandler(OpenEvent);
			((ListView)ControlDictionary["templateListView"]).SelectedIndexChanged += new EventHandler(SelectedIndexChange);
			((TreeView)ControlDictionary["categoryTreeView"]).AfterSelect    += new TreeViewEventHandler(CategoryChange);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeSelect   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeExpand   += new TreeViewCancelEventHandler(OnBeforeExpand);
			((TreeView)ControlDictionary["categoryTreeView"]).BeforeCollapse += new TreeViewCancelEventHandler(OnBeforeCollapse);
			((TextBox)ControlDictionary["solutionNameTextBox"]).TextChanged += new EventHandler(PathChanged);
			((TextBox)ControlDictionary["nameTextBox"]).TextChanged += new EventHandler(NameTextChanged);
			((TextBox)ControlDictionary["nameTextBox"]).TextChanged += new EventHandler(PathChanged);
			((TextBox)ControlDictionary["locationTextBox"]).TextChanged += new EventHandler(PathChanged);
			
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Checked = propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).CheckedChanged += new EventHandler(IconSizeChange);
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["largeIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.LargeIconsIcon");
			
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Checked = !propertyService.GetProperty("Dialogs.NewProjectDialog.LargeImages", true);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).CheckedChanged += new EventHandler(IconSizeChange);
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).FlatStyle = FlatStyle.Standard;
			((RadioButton)ControlDictionary["smallIconsRadioButton"]).Image  = IconService.GetBitmap("Icons.16x16.SmallIconsIcon");
			
			ControlDictionary["openButton"] .Click += new EventHandler(OpenEvent);
			ControlDictionary["browseButton"].Click += new EventHandler(BrowseDirectories);
			((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).CheckedChanged += new EventHandler(CheckedChange);
			((CheckBox)ControlDictionary["createSeparateDirCheckBox"]).CheckedChanged += new EventHandler(PathChanged);
			((CheckBox)ControlDictionary["autoCreateSubDirCheckBox"]).CheckedChanged  += new EventHandler(PathChanged);
			
			ToolTip tooltip = new ToolTip();
			tooltip.SetToolTip(ControlDictionary["largeIconsRadioButton"], StringParserService.Parse("${res:Global.LargeIconToolTip}"));
			tooltip.SetToolTip(ControlDictionary["smallIconsRadioButton"], StringParserService.Parse("${res:Global.SmallIconToolTip}"));
			tooltip.Active = true;
			Owner         = (Form)WorkbenchSingleton.Workbench;
			StartPosition = FormStartPosition.CenterParent;
			Icon          = null;
			
			CheckedChange(this, EventArgs.Empty);
			IconSizeChange(this, EventArgs.Empty);
		}
		
		/// <summary>
		///  Represents a category
		/// </summary>
		internal class Category : TreeNode
		{
			ArrayList categories = new ArrayList();
			ArrayList templates  = new ArrayList();
			string name;
			
			public Category(string name) : base(name)
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
		/// Holds a new file template
		/// </summary>
		internal class TemplateItem : ListViewItem
		{
			ProjectTemplate template;
			
			public TemplateItem(ProjectTemplate template) : base(((StringParserService)ServiceManager.Services.GetService(typeof(StringParserService))).Parse(template.Name))
			{
				this.template = template;
				ImageIndex = 0;
			}
			
			public ProjectTemplate Template {
				get {
					return template;
				}
			}
		}
	}
}
