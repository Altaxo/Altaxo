// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Xml;


using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.Core.AddIns
{
	[ConditionAttribute()]
	public class ProjectOpenCondition : AbstractCondition
	{
		[XmlMemberAttribute("openproject", IsRequired = true)]
		string openproject;
		
		public string OpenProject {
			get {
				return openproject;
			}
			set {
				openproject = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IProject project = projectService.CurrentSelectedProject;
			
			if (project == null && projectService.CurrentOpenCombine != null) {
				ArrayList projects = Combine.GetAllProjects(projectService.CurrentOpenCombine);
				if (projects.Count > 0) {
					project = ((ProjectCombineEntry)projects[0]).Project;
				}
			}
			
			if (openproject == "*") {
				return project != null;
			}
			return project != null && project.ProjectType == openproject;
		}
	}

}
