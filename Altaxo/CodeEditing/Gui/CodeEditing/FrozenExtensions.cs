using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altaxo.Gui.CodeEditing
{
	public static class FrozenExtensions
	{
		public static HighlightingColor AsFrozen(this HighlightingColor color)
		{
			if (!color.IsFrozen)
			{
				color.Freeze();
			}
			return color;
		}
	}
}