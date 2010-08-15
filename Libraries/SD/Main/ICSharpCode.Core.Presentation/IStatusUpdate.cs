// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 3298 $</version>
// </file>

using System;

namespace ICSharpCode.Core.Presentation
{
	public interface IStatusUpdate
	{
		void UpdateText();
		void UpdateStatus();
	}
}
