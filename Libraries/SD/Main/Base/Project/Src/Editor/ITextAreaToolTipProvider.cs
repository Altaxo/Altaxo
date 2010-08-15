﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Christian Hornung"/>
//     <version>$Revision: 5529 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Editor
{
	/// <summary>
	/// Describes an object being able to provide tooltip information for
	/// the text editor.
	/// </summary>
	public interface ITextAreaToolTipProvider
	{
		/// <summary>
		/// Gets tooltip information for the specified position in the text area,
		/// if available.
		/// </summary>
		/// <returns><c>null</c>, if no tooltip information is available at this position, otherwise a ToolTipInfo object containing the tooltip information to be displayed.</returns>
		void HandleToolTipRequest(ToolTipRequestEventArgs e);
	}
}
