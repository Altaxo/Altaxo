// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2043 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
	/// <summary>
	/// Base class for project option panels that are using the <see cref="ConfigurationGuiHelper"/>.
	/// </summary>
	public abstract class AbstractProjectOptionPanel : AbstractOptionPanel, ICanBeDirty
	{
		protected ConfigurationGuiHelper helper;
		protected MSBuildBasedProject project;
		
		protected void InitializeHelper()
		{
			project = (MSBuildBasedProject)((Properties)CustomizationObject).Get("Project");
			baseDirectory = project.Directory;
			helper = new ConfigurationGuiHelper(project, this.ControlDictionary);
		}
		
		public bool IsDirty {
			get { return helper.IsDirty; }
			set { helper.IsDirty = value; }
		}
		
		public event EventHandler DirtyChanged {
			add    { helper.DirtyChanged += value; }
			remove { helper.DirtyChanged -= value; }
		}
		
		public override bool StorePanelContents()
		{
			return helper.Save();
		}
	}
}
