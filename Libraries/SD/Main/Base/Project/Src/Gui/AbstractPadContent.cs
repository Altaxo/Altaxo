﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 5948 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	public abstract class AbstractPadContent : IPadContent
	{
		/// <inheritdoc/>
		public abstract object Control {
			get;
		}
		
		/// <inheritdoc/>
		public virtual object InitiallyFocusedControl {
			get {
				return null;
			}
		}
		
		public virtual void Dispose()
		{
		}
		
		public void BringToFront()
		{
			PadDescriptor d = this.PadDescriptor;
			if (d != null)
				d.BringPadToFront();
		}
		
		protected virtual PadDescriptor PadDescriptor {
			get {
				if (WorkbenchSingleton.Workbench == null || WorkbenchSingleton.Workbench.WorkbenchLayout == null)
					return null;
				return WorkbenchSingleton.Workbench.GetPad(GetType());
			}
		}
	}
}
