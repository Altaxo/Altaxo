// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike KrÃ¼ger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using Reflector.UserInterface;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.Services;
using ICSharpCode.Core.Properties;
using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Services;
namespace ICSharpCode.SharpDevelop.Gui.Dialogs {
	/// <summary>
	/// Dialog for viewing the project options (plain treeview isn't good enough :/)
	/// </summary>
	public class ProjectOptionsDialog : TreeViewOptions
	{
		IProject  project;
		TreeNode configurationTreeNode;
		
		IAddInTreeNode configurationNode;
		
		public ProjectOptionsDialog(IProject project, IAddInTreeNode node, IAddInTreeNode configurationNode) : base(null, null)
		{
			this.project = project;
			this.configurationNode = configurationNode;
			this.Text = StringParserService.Parse("${res:Dialog.Options.ProjectOptions.DialogName}");
			
			((TreeView)ControlDictionary["optionsTreeView"]).MouseUp        += new MouseEventHandler(TreeViewMouseUp);
			((TreeView)ControlDictionary["optionsTreeView"]).AfterLabelEdit += new NodeLabelEditEventHandler(AfterLabelEdit);
			
			((TreeView)ControlDictionary["optionsTreeView"]).Font = boldFont;
			
			properties = new DefaultProperties();
			properties.SetProperty("Project", project);
			
			AddNodes(properties, ((TreeView)ControlDictionary["optionsTreeView"]).Nodes, node.BuildChildItems(this));
			
			configurationTreeNode = new TreeNode(StringParserService.Parse("${res:Dialog.Options.ProjectOptions.ConfigurationsNodeName}"));
			configurationTreeNode.NodeFont = plainFont;
			
			foreach (IConfiguration config in project.Configurations) {
				TreeNode newNode = new TreeNode(config.Name);
				newNode.Tag = config;
				if (config == project.ActiveConfiguration) {
					newNode.NodeFont = boldFont;
				} else {
					newNode.NodeFont = plainFont;
				}
				DefaultProperties configNodeProperties = new DefaultProperties();
				configNodeProperties.SetProperty("Project", project);
				configNodeProperties.SetProperty("Config", config);
				AddNodes(configNodeProperties, newNode.Nodes, configurationNode.BuildChildItems(this));
				configurationTreeNode.Nodes.Add(newNode);
			} 
			((TreeView)ControlDictionary["optionsTreeView"]).Nodes.Add(configurationTreeNode);
			
		}
		
		public void AddProjectConfiguration()
		{
			int    number  = -1;
			string name    = "New Configuration"; // don't localize this project configs should have per default an english name
			string newName = name;
			bool duplicateNumber;
			do {
				duplicateNumber = false;
				foreach (IConfiguration config in project.Configurations) {
					newName = number >= 0 ? name + number : name;
					if (newName == config.Name) {
						++number;
						duplicateNumber = true;
						break;
					}
				}
			} while (duplicateNumber);
			
			TreeNode newNode = new TreeNode(newName);
			IConfiguration newConfig = (IConfiguration)project.ActiveConfiguration.Clone();
			newConfig.Name = newName;
			newNode.Tag  = newConfig;
			newNode.NodeFont = plainFont;
			project.Configurations.Add(newConfig);
			properties.SetProperty("Config", newConfig);
			AddNodes(properties, newNode.Nodes, configurationNode.BuildChildItems(newConfig));
			configurationTreeNode.Nodes.Add(newNode);
			((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = newNode;
			((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit    = true;
			newNode.BeginEdit();
		}
		
		public void RemoveProjectConfiguration()
		{
			IConfiguration config = (IConfiguration)((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode.Tag;
			if (project.Configurations.Count > 1) {
				bool newActiveConfig = project.ActiveConfiguration == config;
				project.Configurations.Remove(config);
				project.ActiveConfiguration = (IConfiguration)project.Configurations[0];
				((TreeView)ControlDictionary["optionsTreeView"]).Nodes.Remove(((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode);
				UpdateBoldConfigurationNode();
				configurationTreeNode.Expand();
			}
		}
		
		void UpdateBoldConfigurationNode()
		{
			foreach (TreeNode node in configurationTreeNode.Nodes) {
				if (node == ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode) {
					node.NodeFont = boldFont;
				} else {
					node.NodeFont = plainFont;
				}
			}
		}
		
		public void SetSelectedConfigurationAsStartup()
		{
			IConfiguration config = ((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode.Tag as IConfiguration;
			if (config != null) {
				project.ActiveConfiguration = config;
				UpdateBoldConfigurationNode();
			}
		}
		
		public void RenameProjectConfiguration()
		{
			((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit    = true;
			((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode.BeginEdit();
		}
		
		void AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			((TreeView)ControlDictionary["optionsTreeView"]).LabelEdit = false;
			
			// canceled edit (or empty name)
			if (e.Label == null || e.Label.Length == 0) {
				return;
			}
			
			bool duplicateLabel       = false;
			foreach (IConfiguration config in project.Configurations) {
				if (e.Label == config.Name) {
					duplicateLabel = true;
					break;
				}
			}
			e.CancelEdit = true;
			
			if (!duplicateLabel) {
				e.Node.Text = e.Label;
				((IConfiguration)e.Node.Tag).Name = e.Label;
			}
		}
		
		static string configNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/ConfigNodeMenu";
		static string selectConfigNodeMenu = "/SharpDevelop/Workbench/ProjectOptions/SelectedConfigMenu";
		
		void TreeViewMouseUp(object sender, MouseEventArgs e)
		{
			TreeNode clickedNode = ((TreeView)ControlDictionary["optionsTreeView"]).GetNodeAt(e.X, e.Y);
			
			if (e.Button == MouseButtons.Right) {
				MenuService menuService = (MenuService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(MenuService));
				if (clickedNode == configurationTreeNode) {
					b = false;
					((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = clickedNode;
					b = true;
					menuService.ShowContextMenu(this, configNodeMenu, this, e.X, e.Y);
				}
				if (clickedNode.Parent == configurationTreeNode) {
					b = false;
					((TreeView)ControlDictionary["optionsTreeView"]).SelectedNode = clickedNode;
					b = true;
					menuService.ShowContextMenu(this, selectConfigNodeMenu, this, e.X, e.Y);
				}
			}
		}
	}
}
