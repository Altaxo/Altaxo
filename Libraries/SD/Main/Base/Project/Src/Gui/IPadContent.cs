﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 5498 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// The IPadContent interface is the basic interface to all "tool" windows
	/// in SharpDevelop.
	/// </summary>
	public interface IPadContent : IDisposable
	{
		/// <summary>
		/// This is the UI element for the view.
		/// You can use both Windows.Forms and WPF controls.
		/// </summary>
		object Control {
			get;
		}
		
		/// <summary>
		/// Gets the control which has focus initially.
		/// </summary>
		object InitiallyFocusedControl {
			get;
		}
	}
}
