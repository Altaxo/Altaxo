// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Project;

namespace ICSharpCode.SharpDevelop.Services 
{
	public delegate void ProjectRenameEventHandler(object sender, ProjectRenameEventArgs e);
	
	public class ProjectRenameEventArgs : EventArgs
	{ 
		IProject project;
		string   oldName;
		string   newName;
		
		public IProject Project {
			get {
				return project;
			}
		}
		
		public string OldName {
			get {
				return oldName;
			}
		}
		
		public string NewName {
			get {
				return newName;
			}
		}
		
		public ProjectRenameEventArgs(IProject project, string oldName, string newName)
		{
			this.project = project;
			this.oldName = oldName;
			this.newName = newName;
		}
	}
}
