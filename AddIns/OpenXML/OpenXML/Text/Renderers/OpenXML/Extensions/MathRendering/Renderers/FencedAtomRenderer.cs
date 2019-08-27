#region Copyright

/////////////////////////////////////////////////////////////////////////////
//    Altaxo:  a data processing and data plotting program
//    Copyright (C) 2002-2018 Dr. Dirk Lellinger
//
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation; either version 2 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
//
/////////////////////////////////////////////////////////////////////////////

#endregion Copyright

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Math;
using WpfMath.Atoms;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="FencedAtom"/> objects, i.e. elements that are embedded in delimiters,
  /// like braces.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.FencedAtom}" />
  internal class FencedAtomRenderer : OpenXMLAtomRenderer<FencedAtom>
  {
    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, FencedAtom item)
    {

      var delimiter = renderer.Push(new Delimiter());


      string leftDelimiterString = "|";
      string rightDelimiterString = "|";
      if (item.LeftDelimeter is SymbolAtom symLeft)
      {
        if (SymbolAtomRenderer.TryConvert(symLeft.Name, out var text))
          leftDelimiterString = text;
      }

      if (item.RightDelimeter is SymbolAtom symRight)
      {
        if (SymbolAtomRenderer.TryConvert(symRight.Name, out var text))
          rightDelimiterString = text;
      }

      var delimiterProp = new DelimiterProperties(
        new BeginChar { Val = leftDelimiterString },
        new EndChar { Val = rightDelimiterString },
        new ControlProperties(
          new W.RunProperties(
            new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
            new W.Italic()
          )
        )
      );

      delimiter.AppendChild(delimiterProp);

      var baseEle = renderer.Push(new Base());

      renderer.Write(item.BaseAtom);

      renderer.PopTo(delimiter);

      return WriteResult.Completed;
    }
  }
}
