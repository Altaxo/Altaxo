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
	public class ActiveViewContentUntitled : AbstractCondition
	{
		[XmlMemberAttribute("activewindowuntitled", IsRequired = true)]
		bool activewindowuntitled;
		
		public bool Activewindowuntitled {
			get {
				return activewindowuntitled;
			}
			set {
				activewindowuntitled = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow == null || WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent == null) {
				return false;
			}
			
			return WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled && activewindowuntitled ||
			       !WorkbenchSingleton.Workbench.ActiveWorkbenchWindow.ViewContent.IsUntitled && !activewindowuntitled ;
		}
	}
}
