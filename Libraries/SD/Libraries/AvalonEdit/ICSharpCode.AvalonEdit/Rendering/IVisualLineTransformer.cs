// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 4142 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.AvalonEdit.Rendering
{
	/// <summary>
	/// Allows transforming visual line elements.
	/// </summary>
	public interface IVisualLineTransformer
	{
		/// <summary>
		/// Applies the transformation to the specified list of visual line elements.
		/// </summary>
		void Transform(ITextRunConstructionContext context, IList<VisualLineElement> elements);
	}
}
