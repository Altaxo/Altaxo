// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing;
using System.Collections.Specialized;
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;
using ICSharpCode.Core.Properties;

using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui.Components;

namespace ICSharpCode.SharpDevelop.Gui.Pads.ProjectBrowser
{
	public class ReferenceNode : AbstractBrowserNode, IDisposable
	{		
		readonly static string defaultContextMenuPath = "/SharpDevelop/Views/ProjectBrowser/ContextMenu/ReferenceNode";
		
		public ProjectReference ProjectReference {
			get {
				return (ProjectReference)UserData;
			}
		}
		
		void SetNodeName(object sender, EventArgs e)
		{
			string name = null;
			switch (ProjectReference.ReferenceType) {
				case ReferenceType.Typelib:
					int index = ProjectReference.Reference.IndexOf("|");
					if (index > 0) {
						name = ProjectReference.Reference.Substring(0, index);
					} else {
						name = ProjectReference.Reference;
					}
					break;
				case ReferenceType.Project:
					name = ProjectReference.Reference;
					break;
				case ReferenceType.Assembly:
					name = Path.GetFileName(ProjectReference.Reference);
					break;
				case ReferenceType.Gac:
					name = ProjectReference.Reference.Split(',')[0];
					break;
				default:
					throw new NotImplementedException("reference type : " + ProjectReference.ReferenceType);
			}
			Text = name;
		}
		
		public override void Dispose()
		{
			base.Dispose();
			this.ProjectReference.ReferenceChanged -= new EventHandler(SetNodeName);
		}
		
		public ReferenceNode(ProjectReference projectReference)
		{
			UserData  = projectReference;
			
			canLabelEdited = false;
			
			contextmenuAddinTreePath = defaultContextMenuPath;
			SetNodeName(this, EventArgs.Empty);
			this.ProjectReference.ReferenceChanged += new EventHandler(SetNodeName);
		}
		
		public override void ActivateItem()
		{
			if (userData != null && userData is ProjectReference) {
				IFileService fileService = (IFileService)ICSharpCode.Core.Services.ServiceManager.Services.GetService(typeof(IFileService));
				fileService.OpenFile(((ProjectReference)userData).GetReferencedFileName(Project));
			}
		}
		
		/// <summary>
		/// Removes a reference from a project
		/// NOTE : This method assumes that its parent is 
		/// from the type 'ProejctBrowserNode'.
		/// </summary>
		public override bool RemoveNode()
		{
			string question = String.Empty;
			ProjectReference referenceInformation = (ProjectReference)UserData;
			ResourceService resourceService = (ResourceService)ServiceManager.Services.GetService(typeof(IResourceService));
			StringParserService stringParserService = (StringParserService)ServiceManager.Services.GetService(typeof(StringParserService));
			
			switch(referenceInformation.ReferenceType) {
				case ReferenceType.Typelib:
					question = resourceService.GetString("ProjectComponent.RemoveReference.RemoveAssemblyQuestion");
					break;
				case ReferenceType.Assembly:
					question = resourceService.GetString("ProjectComponent.RemoveReference.RemoveAssemblyQuestion");
					break;
				case ReferenceType.Project:
					question = resourceService.GetString("ProjectComponent.RemoveReference.RemoveProjectQuestion");
					break;
				case ReferenceType.Gac:
					question = resourceService.GetString("ProjectComponent.RemoveReference.RemoveGacAssembyQuestion");
					break;
				default:
					Debug.Assert(false, "Unknown reference type");
					break;
			}
			
			int ret = new SharpMessageBox(resourceService.GetString("ProjectComponent.RemoveReference.Title"),
			                              stringParserService.Parse(question, new string[,] { {"REFERENCE", Text}}),
								          resourceService.GetString("Global.RemoveButtonText"), 
									      resourceService.GetString("Global.CancelButtonText")).ShowMessageBox();
			
			if (ret == 1 || ret == -1) {
				return false;
			}
		
			Project.ProjectReferences.Remove(referenceInformation);
			
			return true;
		}
	}
}
