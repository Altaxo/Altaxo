// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2455 $</version>
// </file>

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	public class BuildEvents : AbstractProjectOptionPanel
	{
		public override void LoadPanelContents()
		{
			SetupFromXmlResource("ProjectOptions.BuildEvents.xfrm");
			
			InitializeHelper();
			baseDirectory = Path.GetDirectoryName(project.OutputAssemblyFullPath);
			
			ConnectBrowseButton("preBuildEventBrowseButton",
			                    "preBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*",
			                    TextBoxEditMode.EditRawProperty);
			ConnectBrowseButton("postBuildEventBrowseButton",
			                    "postBuildEventTextBox",
			                    "${res:SharpDevelop.FileFilter.AllFiles}|*.*",
			                    TextBoxEditMode.EditRawProperty);
			
			ConfigurationGuiBinding b;
			
			b = helper.BindString("preBuildEventTextBox", "PreBuildEvent", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("preBuildEventTextBox");
			
			b = helper.BindString("postBuildEventTextBox", "PostBuildEvent", TextBoxEditMode.EditRawProperty);
			b.CreateLocationButton("postBuildEventTextBox");
			
			b = helper.BindEnum<RunPostBuildEvent>("runPostBuildEventComboBox", "RunPostBuildEvent");
			b.CreateLocationButton("runPostBuildEventComboBox");
			
			helper.AddConfigurationSelector(this);
		}
	}
}
