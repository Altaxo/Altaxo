// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class AssemblyDeploy  : IDeploymentStrategy
	{
		static string[] extensions = {
			"",
			".exe",
			".dll"
		};
		
		public void DeployProject(IProject project)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (project.DeployInformation.DeployTarget.Length == 0) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(resourceService.GetString("Internal.Project.Deploy.AssemblyDeploy.DeployTargetNotSet"));
				return;
			}
			try {
				AbstractProjectConfiguration config = (AbstractProjectConfiguration)project.ActiveConfiguration;
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				string assembly = fileUtilityService.GetDirectoryNameWithSeparator(config.OutputDirectory) + config.OutputAssembly;
				
				foreach (string  ext in extensions) {
					if (File.Exists(assembly + ext)) {
						File.Copy(assembly + ext, fileUtilityService.GetDirectoryNameWithSeparator(project.DeployInformation.DeployTarget) + config.OutputAssembly + ext, true);
						return;
					}
				}
				throw new Exception("Assembly not found.");
			} catch (Exception e) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e);
			}
		}
	}
}
