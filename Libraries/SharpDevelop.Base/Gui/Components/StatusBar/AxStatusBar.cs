// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui.Components
{
	public class AxStatusBar : System.Windows.Forms.StatusBar
	{
		public AxStatusBar()
		{
		}
		protected override void OnDrawItem(StatusBarDrawItemEventArgs sbdievent)
		{
			if (sbdievent.Panel is AxStatusBarPanel) {
				((AxStatusBarPanel)sbdievent.Panel).DrawPanel(sbdievent);
			} else {
				base.OnDrawItem(sbdievent);
			}
		}
	}
	
}
