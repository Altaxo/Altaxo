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
  /// Renderer for <see cref="AccentedAtom"/> objects, i.e. elements that have an accent.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.AccentedAtom}" />
  internal class AccentedAtomRenderer : OpenXMLAtomRenderer<AccentedAtom>
  {
    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, AccentedAtom item)
    {

      string accentString = string.Empty;

      if (item.AccentAtom is SymbolAtom symAtom)
      {
        if (SymbolAtomRenderer.TryConvert(symAtom.Name, out var text))
          accentString = text;

        if (symAtom.Name == "widehat")
          accentString = null;
      }

      var accent = new Accent();
      renderer.Push(accent);

      var accentProperties = new AccentProperties();

      if (accentString != null) // for widehat, it seems that Word don't even have an accent char (and for widetilde, it is an empty string !)
        accentProperties.AppendChild(new AccentChar() { Val = accentString });

      accentProperties.AppendChild(
        new ControlProperties(
          new W.RunProperties(
            new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
            new W.Italic()
          )
        )
      );

      accent.AppendChild(accentProperties);

      renderer.Push(new Base());

      renderer.Write(item.BaseAtom);

      renderer.PopTo(accent);

      return WriteResult.Completed;
    }
  }
}
