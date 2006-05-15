﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace ICSharpCode.Core
{
	public class MenuSeparator : ToolStripSeparator, IStatusUpdate
	{
		object caller;
		Codon codon;
		
		public MenuSeparator()
		{
			this.RightToLeft = RightToLeft.Inherit;
		}
		
		public MenuSeparator(Codon codon, object caller)
		{
			this.RightToLeft = RightToLeft.Inherit;
			this.caller              = caller;
			this.codon = codon;
		}
		
		public virtual void UpdateStatus()
		{
			if (codon != null) {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				this.Enabled = failedAction != ConditionFailedAction.Disable;
				this.Visible = failedAction != ConditionFailedAction.Exclude;
			}
		}
		
		public virtual void UpdateText()
		{
		}
	}
}
