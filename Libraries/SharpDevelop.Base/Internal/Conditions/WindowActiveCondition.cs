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
		Type prevType      = null;
		bool prevValidFlag = false;
		
		public override bool IsValid(object owner)
		{
			if (WorkbenchSingleton.Workbench == null) {
				return false;
			}
			
			if (activewindow == "*") {
				return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow != null;
			}
			
			if (WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent == null) {
				return false;
			}
			
			Type currentType = WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ActiveViewContent.GetType();
			
			
			if (currentType.Equals(prevType)) {
				return prevValidFlag;
			} else {
				prevType = currentType;
				if (currentType.ToString() == activewindow) {
					prevValidFlag = true;
					return true;
				}
				foreach (Type i in currentType.GetInterfaces()) {
					if (i.ToString() == activewindow) {
						prevValidFlag = true;
						return true;
					}
				}
			}
			prevValidFlag = false;
			return false;			
		}
	}
}
