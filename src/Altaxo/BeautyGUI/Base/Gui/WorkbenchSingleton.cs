// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.CodeDom.Compiler;
using System.Windows.Forms;

using ICSharpCode.Core.Properties;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class WorkbenchSingleton
	{
		const string workbenchMemento        = "SharpDevelop.Workbench.WorkbenchMemento";
		const string uiIconStyle             = "IconMenuItem.IconMenuStyle";
		const string workbenchLayoutProperty = "SharpDevelop.Workbench.WorkbenchLayout";
		const string uiLanguageProperty      = "CoreProperties.UILanguage";
		const string layoutManagerPath       = "/SharpDevelop/Workbench/LayoutManager";
		
		static IWorkbench workbench    = null;
		static string currentLayout          = String.Empty;
		
		public static IWorkbench Workbench {
			get {
				return workbench;
			}
			set {
				workbench = value;
			}
		}
		
		static WorkbenchSingleton()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			propertyService.PropertyChanged += new PropertyEventHandler(TrackPropertyChanges);
		}
		
		static void SetWbLayout()
		{
			PropertyService propertyService = (PropertyService)ServiceManager.Services.GetService(typeof(PropertyService));
			string layoutManager = propertyService.GetProperty(workbenchLayoutProperty, "MDI");
			if (layoutManager != currentLayout) {
				currentLayout = layoutManager;
				workbench.WorkbenchLayout = (IWorkbenchLayout)(ICSharpCode.Core.AddIns.AddInTreeSingleton.AddInTree.GetTreeNode(layoutManagerPath)).BuildChildItem(layoutManager, null);
			}
		}
		
		/// <remarks>
		/// This method handles the redraw all event for specific changed IDE properties
		/// </remarks>
		static void TrackPropertyChanges(object sender, ICSharpCode.Core.Properties.PropertyEventArgs e)
		{
			if (e.OldValue != e.NewValue) {
				switch (e.Key) {
					case workbenchLayoutProperty:
						SetWbLayout();
						break;
					
					case "ICSharpCode.SharpDevelop.Gui.VisualStyle":
					case "CoreProperties.UILanguage":
						workbench.RedrawAllComponents();
						break;
				}
			}
		}
		
		public static void CreateWorkspace()
		{
			SetWbLayout();
			workbench.RedrawAllComponents();
		}
	}
}
