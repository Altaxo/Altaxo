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
  /// Renderer for <see cref="UnderlinedAtom"/> objects, i.e. elements that have a line below.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.UnderlinedAtom}" />
  internal class UnderlinedAtomRenderer : OpenXMLAtomRenderer<UnderlinedAtom>
  {
    protected override void Write(OpenXMLWpfMathRenderer renderer, UnderlinedAtom item)
    {
      var bar = renderer.Push(new Bar());
      var barProperties = new BarProperties();

      barProperties.AppendChild(
          new Position() { Val = VerticalJustificationValues.Bottom }
        );

      barProperties.AppendChild(
        new ControlProperties(
          new W.RunProperties(
            new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
            new W.Italic()
          )
        )
      );

      bar.AppendChild(barProperties);

      renderer.Push(new Base());

      renderer.Write(item.BaseAtom);

      renderer.PopTo(bar);
    }
  }
}
