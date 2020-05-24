// Copyright Eli Arbel (no explicit copyright notice in original file)

// Originated from: RoslynPad, RoslynPad.Roslyn.Windows, GlyphExtensions.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.CodeAnalysis;

namespace Altaxo.CodeEditing.Common
{

  public static class GlyphExtensions
  {
    internal static ImageSource ToImageSource(this Microsoft.CodeAnalysis.Glyph glyph)
    {
      try
      {
        var obj = Application.Current.TryFindResource(new ComponentResourceKey(typeof(Altaxo.CodeEditing.Common.Glyph), (Altaxo.CodeEditing.Common.Glyph)glyph));
        return obj as ImageSource;
      }
      catch (Exception ex)
      {
      }
      return null;
    }

    internal static Microsoft.CodeAnalysis.Glyph GetGlyph(this Microsoft.CodeAnalysis.ISymbol symbol)
    {
      return Microsoft.CodeAnalysis.Shared.Extensions.ISymbolExtensions2.GetGlyph(symbol);
    }
  }
}
