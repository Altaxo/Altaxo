﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;
using System.Windows.Forms;

namespace ICSharpCode.SharpDevelop
{
	public interface IOptionPanel
	{
		/// <summary>
		/// Gets/sets the owner (the context object used when building the option panels
		/// from the addin-tree). This is null for IDE options or the IProject instance for project options.
		/// </summary>
		object Owner { get; set; }
		
		object Control {
			get;
		}
		
		void LoadOptions();
		bool SaveOptions();
	}
}
