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

using DocumentFormat.OpenXml.Math;
using XamlMath.Atoms;

namespace Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.Renderers
{
  /// <summary>
  /// Renderer for <see cref="ScriptsAtom"/> objects, like subscript and superscript.
  /// </summary>
  /// <seealso cref="Altaxo.Text.Renderers.OpenXML.Extensions.MathRendering.OpenXMLAtomRenderer{XamlMath.Atoms.ScriptsAtom}" />
  internal class ScriptsAtomRenderer : OpenXMLAtomRenderer<ScriptsAtom>
  {
    protected override WriteResult Write(OpenXMLWpfMathRenderer renderer, ScriptsAtom item)
    {

      if (item.SubscriptAtom is not null && item.SuperscriptAtom is not null)
      {
        var subscript = renderer.Push(new SubSuperscript());

        var baseM = renderer.Push(new Base());

        renderer.Write(item.BaseAtom);

        renderer.PopTo(baseM);

        var subArgument = renderer.Push(new SubArgument());

        renderer.Write(item.SubscriptAtom);

        renderer.PopTo(subArgument);

        var superArgument = renderer.Push(new SuperArgument());

        renderer.Write(item.SuperscriptAtom);

        renderer.PopTo(superArgument);

        renderer.PopTo(subscript);
      }
      else if (item.SubscriptAtom is not null)
      {
        var subscript = renderer.Push(new Subscript());

        var baseM = renderer.Push(new Base());

        renderer.Write(item.BaseAtom);

        renderer.PopTo(baseM);

        var subArgument = renderer.Push(new SubArgument());

        renderer.Write(item.SubscriptAtom);

        renderer.PopTo(subArgument);

        renderer.PopTo(subscript);
      }
      else if (item.SuperscriptAtom is not null)
      {
        var subscript = renderer.Push(new Superscript());

        var baseM = renderer.Push(new Base());

        renderer.Write(item.BaseAtom);

        renderer.PopTo(baseM);

        var superArgument = renderer.Push(new SuperArgument());

        renderer.Write(item.SuperscriptAtom);

        renderer.PopTo(superArgument);

        renderer.PopTo(subscript);
      }

      return WriteResult.Completed;
    }
  }
}
