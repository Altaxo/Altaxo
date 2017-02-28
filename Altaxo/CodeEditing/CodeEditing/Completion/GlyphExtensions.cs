// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn.Windows, GlyphExtensions.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Altaxo.CodeEditing.Completion
{
	public static class GlyphExtensions
	{
		public static ImageSource ToImageSource(this Glyph glyph)
		{
			try
			{
				var obj = Application.Current.TryFindResource(new ComponentResourceKey(typeof(Glyph), glyph));
				return obj as ImageSource;
			}
			catch (Exception ex)
			{
			}
			return null;
		}

		public static Completion.Glyph GetGlyph(this Microsoft.CodeAnalysis.ISymbol symbol)
		{
			return (Completion.Glyph)Microsoft.CodeAnalysis.Shared.Extensions.ISymbolExtensions2.GetGlyph(symbol);
		}
	}
}