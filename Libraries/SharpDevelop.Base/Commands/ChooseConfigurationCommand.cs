// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns.Codons;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Commands
{
	class ChooseConfigurationCommand : AbstractComboBoxCommand
	{
		IProjectService projectService = (IProjectService)ServiceManager.Services.GetService(typeof(IProjectService));
		
		public ChooseConfigurationCommand()
		{
			projectService.CurrentProjectChanged += new ProjectEventHandler(currentProjectChanged);
			projectService.ActiveConfigurationChanged += new ConfigurationEventHandler(activeConfigurationChanged);
			projectService.ConfigurationAdded += new EventHandler(configurationAdded);
			projectService.ConfigurationRemoved += new EventHandler(configurationRemoved);
		}
		
		void configurationAdded(object sender, EventArgs e)
		{
			refresh();
		}
		
		void configurationRemoved(object sender, EventArgs e)
		{
			refresh();
		}
		
		void activeConfigurationChanged(object sender, ConfigurationEventArgs e)
		{
			refresh();
		}
		
		void currentProjectChanged(object sender, ProjectEventArgs e)
		{
			refresh();
		}
		
		void refresh()
		{
			SdMenuComboBox toolbarItem = (SdMenuComboBox)Owner;
			ComboBox combo = toolbarItem.ComboBox;
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IProject project = projectService.CurrentSelectedProject;
			
			combo.Items.Clear();
			
			if(project != null) {
				foreach(IConfiguration config in project.Configurations) {
					int index = combo.Items.Add(config.Name);
					if(config.Equals(project.ActiveConfiguration)) {
						combo.SelectedIndex = index;
					}
				}
			}
		}
		
		public override void Run()
		{
			SdMenuComboBox toolbarItem = (SdMenuComboBox)Owner;
			ComboBox combo = toolbarItem.ComboBox;
			
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IProject project = projectService.CurrentSelectedProject;
			project.ActiveConfiguration = project.Configurations[combo.SelectedIndex];
		}
	}
}
