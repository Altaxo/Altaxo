﻿#region Copyright

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

using DocumentFormat.OpenXml.Math;
using XamlMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="FractionAtom"/> objects (mathematical fractions).
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{XamlMath.Atoms.FractionAtom}" />
  internal class FractionAtomRenderer : OpenXMLAtomRenderer<FractionAtom>
  {
    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, FractionAtom item)
    {

      var frac = renderer.Push(new Fraction());

      var numerator = renderer.Push(new Numerator());
      renderer.Write(item.Numerator);
      renderer.PopTo(numerator);

      var denominator = renderer.Push(new Denominator());
      renderer.Write(item.Denominator);
      renderer.PopTo(denominator);

      renderer.PopTo(frac);

      return WriteResult.Completed;
    }
  }
}
