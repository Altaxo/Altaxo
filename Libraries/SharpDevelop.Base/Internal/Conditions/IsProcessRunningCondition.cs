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
using ICSharpCode.Core.Services;
using ICSharpCode.SharpDevelop.Services;


namespace ICSharpCode.Core.AddIns
{
	[ConditionAttribute()]
	public class IsProcessRunningCondition : AbstractCondition
	{
		[XmlMemberAttribute("isprocessrunning", IsRequired = true)]
		bool isprocessrunning;
		
		public bool IsProcessRunning {
			get {
				return isprocessrunning;
			}
			set {
				isprocessrunning = value;
			}
		}
		
		public override bool IsValid(object owner)
		{
			DebuggerService debuggerService = (DebuggerService)ServiceManager.Services.GetService(typeof(DebuggerService));
			return isprocessrunning && debuggerService.IsProcessRuning || 
			      !isprocessrunning && !debuggerService.IsProcessRuning;
		}
	}
}
