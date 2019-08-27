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
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="WpfMath.Atoms.Radical"/> objects (radicals like square root).
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.Atoms.Radical}" />
  internal class RadicalAtomRenderer : OpenXMLAtomRenderer<WpfMath.Radical>
  {
    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, WpfMath.Radical item)
    {

      var radicalEle = renderer.Push(new Radical());

      var radicalProperties = new RadicalProperties(
        new ControlProperties(
          new W.RunProperties(
            new W.RunFonts() { Ascii = "Cambria Math", HighAnsi = "Cambria Math" },
            new W.Italic()
            )
          )
        );

      if (null == item.DegreeAtom)
      {
        radicalProperties.PrependChild(new HideDegree() { Val = BooleanValues.One });
      }
      radicalEle.AppendChild(radicalProperties);

      // note we need a Degree element even if no degree text is present
      // (but note that in this case we have a HideDegree element in RadicalProperties)
      var degreeEle = renderer.Push(new Degree());
      if (null != item.DegreeAtom)
        renderer.Write(item.DegreeAtom);
      renderer.PopTo(degreeEle);


      if (null != item.BaseAtom)
      {
        var baseEle = renderer.Push(new Base());
        renderer.Write(item.BaseAtom);
        renderer.PopTo(baseEle);
      }

      renderer.PopTo(radicalEle);

      return WriteResult.Completed;
    }
  }
}
