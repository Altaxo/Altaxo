// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Xml;


using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.Core.AddIns
{
	[ConditionAttribute()]
	public class WindowActiveCondition : AbstractCondition
	{
		[XmlMemberAttribute("activewindow", IsRequired = true)]
		string activewindow;
		
		public string ActiveWindow {
			get {
				return activewindow;
			}
			set {
				activewindow = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			if (activewindow == "*") {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null;
			}
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null) {
				return false;
			}
			Type currentType = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.GetType();
			if (currentType.ToString() == activewindow) {
				return true;
			}
			foreach (Type i in currentType.GetInterfaces()) {
				if (i.ToString() == activewindow) {
					return true;
				}
			}
			return false;
		}
	}
}
