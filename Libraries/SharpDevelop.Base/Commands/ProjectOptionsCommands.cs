// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core.AddIns;
using ICSharpCode.Core.AddIns.Codons;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;

using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Gui.Components;
using ICSharpCode.SharpDevelop.Gui.Dialogs;
using ICSharpCode.SharpDevelop.Internal.Project;

using ICSharpCode.SharpDevelop.Internal.ExternalTool;
using ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser;

namespace ICSharpCode.SharpDevelop.Commands
{
	public class AddProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.AddProjectConfiguration();
			}
		}
	}
	
	public class RenameProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.RenameProjectConfiguration();
			}
		}
	}
	
	public class RemoveProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.RemoveProjectConfiguration();
			}
		}
	}
	
	public class SetActiveProjectConfiguration : AbstractMenuCommand
	{
		public override void Run()
		{
			ProjectOptionsDialog optionsDialog = Owner as ProjectOptionsDialog;
			
			if (optionsDialog != null) {
				optionsDialog.SetSelectedConfigurationAsStartup();
			}
		}
	}
	
	
	
}
