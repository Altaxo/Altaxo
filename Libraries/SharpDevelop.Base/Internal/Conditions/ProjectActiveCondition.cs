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
	public class ProjectActiveCondition : AbstractCondition
	{
		[XmlMemberAttribute("activeproject", IsRequired = true)]
		string activeproject;
		
		public string ActiveProject {
			get {
				return activeproject;
			}
			set {
				activeproject = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			IProjectService projectService = (IProjectService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IProjectService));
			IProject project = projectService.CurrentSelectedProject;
			
			if (activeproject == "*") {
				return project != null;
			}
			return project != null && project.ProjectType == activeproject;
		}
	}

}
