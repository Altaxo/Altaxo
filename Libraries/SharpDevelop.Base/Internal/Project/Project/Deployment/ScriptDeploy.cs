// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Diagnostics;

using ICSharpCode.Core.Services;

namespace ICSharpCode.SharpDevelop.Internal.Project
{
	public class ScriptDeploy : IDeploymentStrategy
	{
		public void DeployProject(IProject project)
		{
			IResourceService resourceService = (IResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			if (project.DeployInformation.DeployScript.Length == 0) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(resourceService.GetString("Internal.Project.Deploy.ScriptDeploy.DeployWithoutScriptError"));
				return;
			}
			try {
				FileUtilityService fileUtilityService = (FileUtilityService)ServiceManager.Services.GetService(typeof(FileUtilityService));
				if (fileUtilityService.TestFileExists(project.DeployInformation.DeployScript)) {
					ProcessStartInfo pInfo = new ProcessStartInfo(project.DeployInformation.DeployScript);
					pInfo.WorkingDirectory = Path.GetDirectoryName(project.DeployInformation.DeployScript);
					Process.Start(pInfo);
				}
			} catch (Exception e) {
				IMessageService messageService =(IMessageService)ServiceManager.Services.GetService(typeof(IMessageService));
				messageService.ShowError(e, resourceService.GetString("Internal.Project.Deploy.ScriptDeploy.ErrorWhileExecuteScript"));
			}
		}
	}
}
