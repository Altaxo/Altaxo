// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ICSharpCode.Core.AddIns.Conditions;
using ICSharpCode.Core.AddIns.Codons;
using Reflector.UserInterface;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class SdMenuSeparator : CommandBarSeparator, IStatusUpdate
	{
		object caller;
		ConditionCollection conditionCollection;
		
		public SdMenuSeparator()
		{
		}
		
		public SdMenuSeparator(ConditionCollection conditionCollection, object caller)
		{
			this.caller              = caller;
			this.conditionCollection = conditionCollection;
		}
		
		public virtual void UpdateStatus()
		{
			if (conditionCollection != null) {
				ConditionFailedAction failedAction = conditionCollection.GetCurrentConditionFailedAction(caller);
				this.IsEnabled = failedAction != ConditionFailedAction.Disable;
				this.IsVisible = failedAction != ConditionFailedAction.Exclude;
			}
		}
	}
}
