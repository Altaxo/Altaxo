// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.IO;
using System.Xml;


using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core.AddIns
{
	[ConditionAttribute()]
	public class ActiveContentExtensionCondition : AbstractCondition
	{
		[XmlMemberAttribute("activeextension", IsRequired = true)]
		string activeextension;
		
		public string Activeextension {
			get {
				return activeextension;
			}
			set {
				activeextension = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return false;
			}
			try {
				string name = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled ?
				              WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.UntitledName : WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.FileName;
				
				if (name == null) {
					return false;
				}
				string extension = Path.GetExtension(name);
				return extension.ToUpper() == activeextension.ToUpper();
			} catch (Exception) {
				return false;
			}
		}
	}
}
