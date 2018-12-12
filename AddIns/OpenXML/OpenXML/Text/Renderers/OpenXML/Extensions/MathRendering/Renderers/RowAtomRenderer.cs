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
using WpfMath;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="RowAtom"/> objects, containing children that are horizontally aligned.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{WpfMath.RowAtom}" />
  internal class RowAtomRenderer : OpenXMLAtomRenderer<RowAtom>
  {
    protected override void Write(OpenXMLWpfMathRenderer renderer, RowAtom item)
    {
      foreach (var subItems in item.Elements)
        renderer.Write(subItems);
    }
  }
}
