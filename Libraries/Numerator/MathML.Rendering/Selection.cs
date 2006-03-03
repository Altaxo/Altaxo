using System;
using System.Xml;

namespace MathML.Rendering
{
	// where to look for the caret position
	internal enum SelectionType
	{
		Area, Prev, Next, Mouse
	}

	// selection type for a terminal node. This indicates if we should place the cursor at either the start
	// or end of a terminal node
	internal enum TerminalSelectionType
	{
		Start, End
	}

	public class Selection
	{
		// the currently selected element
		public MathMLElement Element;
		
		// used for string areas, the index of the currently selected char
		public int CharIndex = 0;
	}
}
